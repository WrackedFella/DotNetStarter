using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace SafeBaby.Web.Core.PageModels
{
    public abstract class HistoryModelBase<TDomainObject> : PageModelBase
        where TDomainObject : DomainObject
    {
        public TDomainObject Record { get; set; }

        public Guid RecordId { get; set; }

        private string TableName { get; }

        private readonly List<string> _excludedFields = new List<string>
        {
            "LastModifiedBy",
            "LastModifiedDate",
            "CreatedBy",
            "CreatedDate",
            "FullName"
        };

        protected HistoryModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context, contextAccessor)
        {
            this.TableName = typeof(TDomainObject).Name switch
            {
                "Address" => "Addresses",
                "Person" => "Persons",
                _ => "Unknown",
            };
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            this.Record = await this.Context.Set<TDomainObject>().FindAsync(id);
            if (this.Record == null)
            {
                return NotFound();
            }
            this.RecordId = id;
            return Page();
        }

        public virtual async Task<JsonResult> OnGetGridDataAsync(Guid id, Guid logId)
        {
            var resultSet = new List<object>();
            foreach (var ah in await this.Context.AuditHistory.Where(x => x.TableName == this.TableName && x.RowId.ToUpper() == id.ToString().ToUpper()).ToListAsync())
            {
                var changeSet = JsonConvert.DeserializeObject<(TDomainObject, TDomainObject)>(ah.Changed);
                var afterProps = changeSet.Item1.GetType().GetProperties().Where(x => !this._excludedFields.Contains(x.Name)).ToArray();
                var beforeProps = changeSet.Item2.GetType().GetProperties().Where(x => !this._excludedFields.Contains(x.Name)).ToArray();
                resultSet.AddRange(afterProps.Join(beforeProps,
                        aProp => aProp.Name,
                        bProp => bProp.Name,
                        (aProp, bProp) =>
                        {
                            var before = (bProp.GetValue(changeSet.Item1)?.ToString() ?? "-").Split(' ')[0];
                            var after = (aProp.GetValue(changeSet.Item2)?.ToString() ?? "-").Split(' ')[0];
                            return new
                            {
                                PropertyName = aProp.Name,
                                Before = before,
                                After = after,
                                ChangedBy = ah.LastModifiedBy,
                                ChangedOn = ah.Created
                            };
                        })
                    .Where(x => x.After != x.Before)
                    .ToList());
            }

            //return new JsonResult(await this.Context.FormatLogData(resultSet));
            return new JsonResult(resultSet);
        }
    }
}
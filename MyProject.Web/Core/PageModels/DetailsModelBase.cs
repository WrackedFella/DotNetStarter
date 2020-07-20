using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace MyProject.Web.Core.PageModels
{
    public abstract class DetailsModelBase<TDomainObject> : PageModelBase
        where TDomainObject : DomainObject
    {
        protected DetailsModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context, contextAccessor)
        {
        }

        [BindProperty]
        public TDomainObject Record { get; set; }

        [BindProperty]
        public Guid RecordId { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(Guid id)
        {
            this.RecordId = id;
            this.Record = await this.Context.Set<TDomainObject>().FindAsync(id);

            if (this.Record == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
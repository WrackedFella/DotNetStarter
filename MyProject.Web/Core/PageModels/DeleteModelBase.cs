using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace MyProject.Web.Core.PageModels
{
    public abstract class DeleteModelBase<TDomainObject> : PageModelBase
        where TDomainObject : DomainObject
    {
        protected DeleteModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context, contextAccessor)
        {
        }

        [BindProperty]
        public TDomainObject Record { get; set; }

        [BindProperty]
        public Guid RecordId { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.RecordId = (Guid)id;
            this.Record = await this.Context.Set<TDomainObject>().FindAsync((Guid)id);

            if (this.Record == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            this.Record = await this.Context.Set<TDomainObject>().FindAsync(id);
            this.Record.Deactivate();
            await this.Context.SaveChangesAsync(this.Username);
            return RedirectToPage("./Index");
        }
    }
}
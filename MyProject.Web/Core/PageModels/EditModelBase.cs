using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain;
using MyProject.Domain.Core;
using MyProject.Web.Core;

namespace SafeBaby.Web.Core.PageModels
{
    public abstract class EditModelBase<TDomainObject> : PageModelBase
        where TDomainObject : DomainObject, new()
    {
        protected EditModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context, contextAccessor)
        {
        }

        [BindProperty] public TDomainObject Record { get; set; }

        public Guid RecordId { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.RecordId = (Guid)id;
            this.Record = await this.Context.Set<TDomainObject>().FindAsync(id);

            if (this.Record == null)
            {
                return NotFound();
            }
            if (!this.Record.IsActive)
            {
                return RedirectToPage("./Details", new { id });
            }

            return await LoadPageAsync();
        }

        public virtual async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                // ToDo: Invoke a delegate here to populate view data
                this.RecordId = id;
                return await LoadPageAsync();
            }

            // ToDo: DbConcurrencyError check?
            var updateTarget = await this.Context.Set<TDomainObject>().FindAsync(id);
            if (updateTarget == null)
            {
                throw new NullReferenceException("Could not find record.");
            }

            this.Context.Entry(updateTarget).UpdateRecord(this.Record);

            await this.Context.SaveChangesAsync(this.Username);

            return RedirectToPage("./Details", new { id });
        }

        public virtual async Task<IActionResult> LoadPageAsync()
        {
            return Page();
        }
    }
}
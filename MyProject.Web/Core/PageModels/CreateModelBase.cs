using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace SafeBaby.Web.Core.PageModels
{
	public abstract class CreateModelBase<TDomainObject> : PageModelBase
		where TDomainObject : DomainObject, new()
	{
		protected CreateModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context, contextAccessor)
		{
		}

		[BindProperty] public TDomainObject Record { get; set; } = new TDomainObject();

		public virtual async Task<IActionResult> OnGetAsync()
		{
			return await LoadPageAsync();
		}

		public virtual async Task<IActionResult> OnPostAsync()
		{
			if (!this.ModelState.IsValid)
			{
				return await LoadPageAsync();
			}

			await this.Context.Set<TDomainObject>().AddAsync(this.Record);
			await this.Context.SaveChangesAsync(this.Username);

			return RedirectToPage("./Details", new { id = this.Record.GetId() });
		}

		public virtual async Task<IActionResult> LoadPageAsync()
		{
			return Page();
		}
	}
}
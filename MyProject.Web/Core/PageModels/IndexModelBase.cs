using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace SafeBaby.Web.Core.PageModels
{
	public abstract class IndexModelBase<TDomainObject> : PageModel
		where TDomainObject : DomainObject
	{
		protected readonly MyProjectContext Context;

		protected IndexModelBase(MyProjectContext context)
		{
			this.Context = context;
		}

		public virtual async Task<JsonResult> OnGetGridDataAsync()
		{
			List<TDomainObject> resultSet;
			if (bool.TryParse(this.Request.Query["isActiveFilter"], out var isActive))
			{
				resultSet = await this.Context.Set<TDomainObject>()
					.Where(x => x.IsActive == isActive)
					.AsNoTracking()
					.ToListAsync();
			}
			else
			{
				resultSet = await this.Context.Set<TDomainObject>()
					.Where(x => x.IsActive)
					.AsNoTracking()
					.ToListAsync();
			}
			// ToDo: Format and apply filters as needed to make it bind to a grid/table.
			return new JsonResult(resultSet);
		}
	}
}
using Microsoft.AspNetCore.Http;
using MyProject.Domain;
using MyProject.Domain.Core;
using MyProject.Domain.People;
using MyProject.Web.Core;
using MyProject.Web.Core.PageModels;

namespace MyProject.Web.Pages.Persons
{
    [CustomAuthorize()]
    public class EditModel : EditModelBase<Person>
    {
        public EditModel(MyProjectContext context, IHttpContextAccessor contextAccessor) : base(context,
            contextAccessor)
        {
        }
    }
}

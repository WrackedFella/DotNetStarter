using MyProject.Domain;
using MyProject.Domain.Core;
using MyProject.Domain.People;
using MyProject.Web.Core;
using MyProject.Web.Core.PageModels;

namespace MyProject.Web.Pages.Persons
{
    [CustomAuthorize(Role.User)]
    public class IndexModel : IndexModelBase<Person>
    {
        public IndexModel(MyProjectContext context) : base(context)
        {
        }
    }
}

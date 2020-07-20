using Microsoft.AspNetCore.Http;
using MyProject.Domain;
using MyProject.Web.Core;

namespace MyProject.Web.Pages
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(MyProjectContext context) : base(context) { }

        public void OnGet()
        {
        }
    }
}
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MyProject.Domain.Enums;

namespace SafeBaby.Web.Core
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(params string[] roles) : base()
        {
            this.AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
            this.Roles = string.Join(",", roles, Role.Admin.Name, Role.Developer.Name);
        }
    }
}
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MyProject.Domain.Core;

namespace MyProject.Web.Core
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(params string[] roles) : base()
        {
            this.AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
            this.Roles = string.Join(",", roles, Role.Admin, Role.Developer);
        }
    }
}
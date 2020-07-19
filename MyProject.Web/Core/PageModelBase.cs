using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using MyProject.Domain;
using MyProject.Web.Core;
using MyProject.Domain.Enums;

namespace SafeBaby.Web.Core
{
    public abstract class PageModelBase : PageModel
    {
        public bool IsUnitTest { get; set; } = false;
        protected string Username { get; }
        protected string RoleName { get; }
        protected MyProjectContext Context { get; set; }

        protected readonly CookieOptions CookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(30)
        };

        protected PageModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor)
        {
            this.Context = context;
            if (contextAccessor == null)
            {
                return;
            }
            this.Username = contextAccessor.GetUserName();
            if (contextAccessor.HttpContext.User.IsInRole(Role.Anonymous.Name))
            {
                this.RoleName = Role.Anonymous.Name;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.User.Name))
            {
                this.RoleName = Role.User.Name;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.Admin.Name))
            {
                this.RoleName = Role.Admin.Name;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.Developer.Name))
            {
                this.RoleName = Role.Developer.Name;
                return;
            }
        }

        protected T RetrieveCookie<T>(string key)
            where T : new()
        {
            return this.Request?.Cookies[key] != null
                ? JsonConvert.DeserializeObject<T>(this.Request?.Cookies[key])
                : new T();
        }

        protected void AppendCookie(string key, object value)
        {
            if (value != null)
            {
                this.Response?.Cookies.Append(
                    key,
                    JsonConvert.SerializeObject(value),
                    this.CookieOptions);
            }
        }

        protected void ResetCookie(string key)
        {
            this.Response?.Cookies.Append(key, "", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        public string GetPageTitle() => this.GetPageName().AddSpaces();

        public string GetPageName() => this.GetType().Name.Replace("Model", "");
    }
}
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using MyProject.Domain;
using MyProject.Domain.Core;

namespace MyProject.Web.Core
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

        protected PageModelBase(MyProjectContext context)
        {
            this.Context = context;
            this.Username = "User";
            
            this.RoleName = Role.Anonymous;
            return;
        }

        protected PageModelBase(MyProjectContext context, IHttpContextAccessor contextAccessor)
        {
            this.Context = context;
            if (contextAccessor == null)
            {
                return;
            }
            this.Username = contextAccessor.GetUserName();
            if (contextAccessor.HttpContext.User.IsInRole(Role.Anonymous))
            {
                this.RoleName = Role.Anonymous;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.User))
            {
                this.RoleName = Role.User;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.Admin))
            {
                this.RoleName = Role.Admin;
                return;
            }
            if (contextAccessor.HttpContext.User.IsInRole(Role.Developer))
            {
                this.RoleName = Role.Developer;
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
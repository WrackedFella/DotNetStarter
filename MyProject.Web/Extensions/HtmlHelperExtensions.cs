using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyProject.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString BackButton<TPageModel>(this IHtmlHelper<TPageModel> helper, string text, string route)
        {
            return new HtmlString($"<a class=\"btn btn-primary mb-3 nav-link\" href=\"{route}\">" +
                     $"<span class=\"d-inline-block text-center fas fa-arrow-left\"></span>&nbsp;{text}</a>");
        }

    }
}

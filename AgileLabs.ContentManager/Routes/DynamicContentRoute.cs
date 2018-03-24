using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AgileLabs.ContentManager.Repositories;
using AgileLabs.ContentManager.Models;
using Microsoft.Extensions.DependencyInjection;
using AgileLabs.ContentManager.Services;

namespace AgileLabs.ContentManager.Routes
{
    public class DynamicContentRoute : Route, IRouter
    {
        private const string SLUG = "slug";
        public DynamicContentRoute(IRouteBuilder routeBuilder, IInlineConstraintResolver resolver)
            : base(routeBuilder.DefaultHandler, "{*slug}", resolver)
        {
        }
        protected override async Task OnRouteMatched(RouteContext context)
        {
            var slugValue = context.RouteData.Values[SLUG]?.ToString();
            var _urlRecordRepository = context.HttpContext.RequestServices.GetService<UrlRecordServcie>();
            var page = _urlRecordRepository.GetUrlRecord(slugValue);
            if (page != null)
            {
                context.RouteData.Values.Add("controller", "DynamicContent");
                context.RouteData.Values.Add("action", "Preview");
                context.RouteData.Values.Add("urlRecordId", page.Id);
                await base.OnRouteMatched(context);
            }
        }

        protected override VirtualPathData OnVirtualPathGenerated(VirtualPathContext context)
        {
            return base.OnVirtualPathGenerated(context);
        }
    }
}

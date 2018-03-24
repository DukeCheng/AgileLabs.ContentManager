using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace AgileLabs.ContentManager.Routes
{
    public static class DynamicContentRouteBuilderExtensions
    {
        public static IRouteBuilder MapDynamicContentRoute(this IRouteBuilder routeCollectionBuilder)
        {
            var inlineConstraintResolver = routeCollectionBuilder.ServiceProvider.GetService<IInlineConstraintResolver>();

            routeCollectionBuilder.Routes.Add(new DynamicContentRoute(routeCollectionBuilder, inlineConstraintResolver));
            return routeCollectionBuilder;
        }
    }
}

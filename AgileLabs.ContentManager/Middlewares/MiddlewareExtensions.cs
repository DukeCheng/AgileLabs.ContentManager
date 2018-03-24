using Microsoft.AspNetCore.Builder;

namespace AgileLabs.ContentManager.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }

        public static IApplicationBuilder UseUeditor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UEditorMiddleware>();
        }

        public static IApplicationBuilder UseUploadResource(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReadUploadResourceMiddleware>();
        }

        public static IApplicationBuilder UseSubmitForm(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FormMiddleware>();
        }
    }
}

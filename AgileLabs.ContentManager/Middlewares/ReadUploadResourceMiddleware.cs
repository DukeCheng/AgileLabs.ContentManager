using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using AgileLabs.ContentManager.UEditor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.IO;
using AgileLabs.ContentManager.Entities;

namespace AgileLabs.ContentManager.Middlewares
{
    public class ReadUploadResourceMiddleware
    {
        private readonly RequestDelegate _next;

        public ReadUploadResourceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHostingEnvironment env, IOptions<UEditorConfig> uEditorConfig)
        {
            if (context.Request.Path.Value.StartsWith("/upload", StringComparison.OrdinalIgnoreCase))
            {
                var requestPath = context.Request.Path.Value.Trim('/');
                var resourceRepository = context.RequestServices.GetService<MongoDbBaseRepository<Resource>>();
                var resource = resourceRepository.SearchOneAsync(Builders<Resource>.Filter.Where(x => x.Path == requestPath)).Result;
                if (resource == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                var stream = resourceRepository.DownloadFile(resource.FileId);
                var response = context.Response;
                response.ContentType = resource.ContentType;
                context.Response.Headers.Add("Content-Disposition", new[] { "attachment; filename=" + resource.FileName });

                await stream.CopyToAsync(context.Response.Body);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await this._next(context);
            }
        }
    }
}

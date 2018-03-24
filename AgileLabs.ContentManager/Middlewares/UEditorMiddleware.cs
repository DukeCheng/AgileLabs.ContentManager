using AgileLabs.ContentManager.UEditor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Middlewares
{
    public class UEditorMiddleware
    {
        private readonly RequestDelegate _next;

        public UEditorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHostingEnvironment env, IOptions<UEditorConfig> uEditorConfig)
        {
            if (context.Request.Path.Value.StartsWith("/ueditor", StringComparison.OrdinalIgnoreCase))
            {
                //RenderMainLogPage(context);
                Handler action = null;
                switch (context.Request.Query["action"])
                {
                    case "config":
                        action = new ConfigHandler(env);
                        break;
                    case "uploadimage":
                        var config = uEditorConfig.Value;
                        action = new UploadHandler(new UploadConfig()
                        {
                            AllowExtensions = config.imageAllowFiles.ToArray(),// Config.GetStringList("imageAllowFiles"),
                            PathFormat = config.imagePathFormat,// Config.GetString("imagePathFormat"),
                            SizeLimit = config.imageMaxSize,// Config.GetInt("imageMaxSize"),
                            UploadFieldName = config.imageFieldName // Config.GetString("imageFieldName")
                        });
                        break;
                    //case "uploadscrawl":
                    //    action = new UploadHandler(context, new UploadConfig()
                    //    {
                    //        AllowExtensions = new string[] { ".png" },
                    //        PathFormat = Config.GetString("scrawlPathFormat"),
                    //        SizeLimit = Config.GetInt("scrawlMaxSize"),
                    //        UploadFieldName = Config.GetString("scrawlFieldName"),
                    //        Base64 = true,
                    //        Base64Filename = "scrawl.png"
                    //    });
                    //    break;
                    //case "uploadvideo":
                    //    action = new UploadHandler(context, new UploadConfig()
                    //    {
                    //        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                    //        PathFormat = Config.GetString("videoPathFormat"),
                    //        SizeLimit = Config.GetInt("videoMaxSize"),
                    //        UploadFieldName = Config.GetString("videoFieldName")
                    //    });
                    //    break;
                    //case "uploadfile":
                    //    action = new UploadHandler(context, new UploadConfig()
                    //    {
                    //        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                    //        PathFormat = Config.GetString("filePathFormat"),
                    //        SizeLimit = Config.GetInt("fileMaxSize"),
                    //        UploadFieldName = Config.GetString("fileFieldName")
                    //    });
                    //    break;
                    //case "listimage":
                    //    action = new ListFileManager(context, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"));
                    //    break;
                    //case "listfile":
                    //    action = new ListFileManager(context, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"));
                    //    break;
                    //case "catchimage":
                    //    action = new CrawlerHandler(context);
                    //    break;
                    default:
                        action = new NotSupportedHandler();
                        break;
                }
                await action.ExecuteAsync(context);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await this._next(context);
            }
        }
    }
}

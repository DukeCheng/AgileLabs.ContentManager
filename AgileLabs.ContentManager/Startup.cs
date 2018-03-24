using AgileLabs.ContentManager.Middlewares;
using AgileLabs.ContentManager.Repositories;
using AgileLabs.ContentManager.Services;
using AgileLabs.ContentManager.UEditor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.IO.Compression;
using System.Linq;
using AgileLabs.ContentManager.Filters;
using AgileLabs.ContentManager.Routes;
using AgileLabs.ContentManager.Common;

namespace AgileLabs.ContentManager
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSingleton(factory => services);
            services.AddSingleton(factory => Configuration);
            services.AddOptions();
            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddMemoryCache();
            services.AddDistributedMemoryCache(options =>
            {

            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(o => o.SessionStore = new MemoryCacheTicketStore());

            services.AddMvc(options =>
            {
                if (!Environment.IsDevelopment())
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(new ErrorExceptionFilterAttribute());
                }
                options.Filters.Add(new ErrorExceptionFilterAttribute());
            });

            //services.AddWebMarkupMin(options =>
            //{
            //    options.AllowMinificationInDevelopmentEnvironment = true;
            //    options.DisablePoweredByHttpHeaders = true;
            //    options.DisableCompression = true;
            //})
            //.AddHtmlMinification(options =>
            //{
            //    options.MinificationSettings.RemoveRedundantAttributes = true;
            //    options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
            //    options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
            //    options.MinificationSettings.MinifyEmbeddedCssCode = false;
            //    options.MinificationSettings.RemoveOptionalEndTags = false;
            //});

            services.Configure<MongodbSettings>(options => Configuration.GetSection(nameof(MongodbSettings)).Bind(options));
            services.Configure<UEditorConfig>(options => Configuration.GetSection(nameof(UEditorConfig)).Bind(options));

            services.AddSingleton<MongoDbContext>();
            services.AddScoped(typeof(MongoDbBaseRepository<>));

            services.AddScoped<UrlRecordServcie>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseResponseCaching();
            }

            app.UseAuthentication();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });

            //app.UseWebMarkupMin();
            app.UseRequestCulture();
            app.UseUeditor();
            app.UseUploadResource();
            app.UseSubmitForm();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller=Home}/{action=Index}");
                routes.MapRoute(name: "default", template: "{controller=RazorContent}/{action=Index}/{id?}");
                routes.MapDynamicContentRoute();
            });
        }
    }
}

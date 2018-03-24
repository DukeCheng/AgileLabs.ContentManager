using AgileLabs.ContentManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Middlewares
{
    public class FormMiddleware
    {
        private readonly RequestDelegate _next;

        public FormMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHostingEnvironment env)
        {
            if (context.Request.Path.Value.StartsWith("/submit", StringComparison.OrdinalIgnoreCase))
            {
                var url = context.Request.GetDisplayUrl().ToLower();

                if (url.Contains($"submit"))
                {
                    url = url.Replace("submit", "api");
                }

                //create json param
                var queryString = context.Request.Form;

                //build dictionary
                var dictionary = new Dictionary<string, string>();

                queryString.ToList().ForEach(item =>
                {
                    if (item.Key == @"__RequestVerificationToken")
                        return;
                    dictionary.Add(item.Key, item.Value);
                });

                //request api 
                await context.PostAsync(url,
                    dictionary,
                    System.Text.Encoding.GetEncoding("utf-8"));

            }
            else
            {
                await _next(context);
            }
        }
    }
}

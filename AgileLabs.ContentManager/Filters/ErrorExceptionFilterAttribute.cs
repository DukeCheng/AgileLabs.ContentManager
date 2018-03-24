using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileLabs.ContentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgileLabs.ContentManager.Filters
{
    public class ErrorExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(new WebResponseModel
            {
                error = true,
                errorMsg = context.Exception.Message
            });
        }
    }
}

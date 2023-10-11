using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBookstore_RealBotz.Models.Common
{
    public class SessionExpirationCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Session.IsAvailable || context.HttpContext.Session.GetString("UserId") == null)
            {
                string message = "Your session has expired. Please log in again.";
                context.Result = new RedirectToActionResult("Index", "Account", new { message });
            }

            base.OnActionExecuting(context);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Helper
{
    public class AdminSessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (SessionHandler.IsExpired)
            {
                filterContext.Result = new RedirectResult("~/Account/AdminLogin");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
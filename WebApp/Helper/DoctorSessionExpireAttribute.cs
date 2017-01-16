using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Helper
{
    public class DoctorSessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (HttpContext.Current.Session["username"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/DoctorLogin");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
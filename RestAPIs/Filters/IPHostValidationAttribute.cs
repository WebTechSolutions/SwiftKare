using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RestAPIs.Models;
using RestAPIs.Repositories;

namespace RestAPIs.Filters
{
    // ReSharper disable once InconsistentNaming
    public class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var visitorsIpAddr = "::1";
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                visitorsIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
            {
                visitorsIpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                var context = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
                if (context != null) visitorsIpAddr = context.Request.UserHostAddress;
            }

            try
            {
                var objRepo = new IPsRepository();
                if (objRepo.Exists(visitorsIpAddr)) return;
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Messages.UnauthorizedIpAddress);
            }
            catch (Exception)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Messages.UnauthorizedIpAddress);
                //actionContext.Response =
                //    new HttpResponseMessage(HttpStatusCode.Forbidden)
                //    {
                //        Content = new StringContent("Unauthorized IP Address")
                //    };
            }
        }
    }
}
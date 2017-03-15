using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using RestAPIs.Models;
namespace RestAPIs.Filters
{
    public class CustomHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if (request.RequestUri.Scheme == Uri.UriSchemeHttps) return;
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, Messages.HttpsInvalidMessage);
        }
    }




}
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //base.HandleUnauthorizedRequest(filterContext);
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "AdminLogin", action = "AdminLogin" }));
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "AdminLogin", action = "AdminLogin" }));
            }
        }
    }
}
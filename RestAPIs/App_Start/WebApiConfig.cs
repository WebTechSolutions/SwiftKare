using System.Web.Http;
using RestAPIs.Filters;
using RestAPIs.Models;

namespace RestAPIs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );


            if (ApplicationGlobalVariables.Instance.ApplyHttpsFilterRequired)
                config.Filters.Add(new CustomHttpsAttribute());

            //if (ApplicationGlobalVariables.Instance.ApplyIPsAuthorizationFilter)
            //    config.Filters.Add(new IPHostValidationAttribute());

        }
    }
}
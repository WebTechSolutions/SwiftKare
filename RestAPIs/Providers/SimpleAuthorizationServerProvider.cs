using Microsoft.Owin.Security.OAuth;
using RestAPIs.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using RestAPIs.Repositories;
using Microsoft.AspNet.Identity.Owin;
using Identity.Membership;
using Identity.Membership.Models;
using Microsoft.AspNet.Identity;

namespace RestAPIs.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var objModel = new OauthUserModel();
            context.Validated();
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            if (clientId == objModel.OauthClient && clientSecret == objModel.OauthClientSecret)
            {
                context.Validated(clientId);
            }
            else
            {
                context.Rejected();
            }
            return base.ValidateClientAuthentication(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            

            var userIdentity = await userManager.CreateIdentityAsync(user, context.Options.AuthenticationType);

            

            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim("sub", context.UserName));
            id.AddClaim(new Claim("role", "apiconsumer"));

            context.Validated(userIdentity);
        }

    }
}
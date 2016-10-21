using Microsoft.Owin.Security.OAuth;
using RestAPIs.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using RestAPIs.Repositories;

namespace RestAPIs.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            // OAuth2 supports the notion of client authentication
            // this is not used here

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
            // validate user credentials (demo!)
            // user credentials should be stored securely (salted, iterated, hashed yada)
            //var objModel =  new OauthUserModel();
            //if (string.IsNullOrEmpty(objModel.OauthUserName) && string.IsNullOrEmpty(objModel.OauthPassword))
            //{
            //    var objRepo = new OauthUsersRepository(context.UserName,context.Password);
            //    objModel = objRepo.Find(null);
            //    if (objModel == null)
            //    {
            //        context.Rejected();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!(context.UserName == objModel.OauthUserName && context.Password == objModel.OauthPassword))
            //    {
            //        context.Rejected();
            //        return;
            //    }
            //}
            // create identity
            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim("sub", context.UserName));
            id.AddClaim(new Claim("role", "apiconsumer"));

            context.Validated(id);
        }
    }
}
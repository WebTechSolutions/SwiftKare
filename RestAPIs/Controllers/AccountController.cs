using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Identity.Membership;
using Identity.Membership.Models;
using RestAPIs.Extensions;
using RestAPIs.Models;

namespace RestAPIs.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }


        private bool IsValidclient(HttpRequestMessage request)
        {
            var headerValue = request.GetHeader("Authorization");

            if (headerValue == null)
                return false;

            if (headerValue.Contains(":"))
            {
                var arr = headerValue.Split(':');
                if (arr.Length > 1)
                {
                    var clientId = arr[0];
                    var clientSecret = arr[1];
                    var objModel = new OauthUserModel();
                    if (!(clientId == objModel.OauthClient && clientSecret == objModel.OauthClientSecret))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        public async Task<SignInStatus> Login(LoginViewModel model, HttpRequestMessage request)
        {
            if (!IsValidclient(request))
                return SignInStatus.Failure;

            //    var id = headerValues.FirstOrDefault();
            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            return result;
        }

        


        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IdentityResult> Register(RegisterViewModel model, HttpRequestMessage request)
        {
            

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            // Add the Address properties:
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.PostalCode = model.PostalCode;

            var result = await UserManager.CreateAsync(user, model.Password);
            return result;
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<string> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return "NotConfirmed";
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                return code;
            }

            // If we got this far, something failed, redisplay form
            return "";
        }


        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IdentityResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            if(user!=null)
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                return result;
            }
            return null;
        }


        [HttpPost]
        [Route("LogOff")]
        public string LogOff()
        {
            AuthenticationManager.SignOut();
            return "LogOff";
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }




    }
}

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





        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        public async Task<SignInStatus> Login(LoginViewModel model, HttpRequestMessage request)
        {

            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }
            // return SignInStatus.Failure;

            try
            {
                //    var id = headerValues.FirstOrDefault();
                // This doen't count login failures towards lockout only two factor authentication
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                return result;
            }

            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred while posting in api/account/login, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }




        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IdentityResult> Register(RegisterViewModel model, HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }

            try
            {

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                                // Add the Address properties:

                var result = await UserManager.CreateAsync(user, model.Password);
               

                return result;
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred while posting in api/Account/Register, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<string> ForgotPassword(ForgotPasswordViewModel model, HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Email);


                    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("user is not exist with this email address or email is not confirmed"),
                            ReasonPhrase = "Not Confirmed"
                        };
                        throw new HttpResponseException(resp);
                    }

                    var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    return code;
                }

                // If we got this far, something failed, redisplay form
                return "";
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred while posting in api/account/ForgotPassword, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }


        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IdentityResult> ResetPassword(ResetPasswordViewModel model, HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "BadRequest"
                };
                throw new HttpResponseException(resp);
            }
            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                    return result;
                }
                else
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("user is not exist with this email address"),
                        ReasonPhrase = "Not Found"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred while posting in api/account/ResetPassword, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }


        [HttpPost]
        [Route("LogOff")]
        public string LogOff(HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "BadRequest"
                };
                throw new HttpResponseException(resp);
            }
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

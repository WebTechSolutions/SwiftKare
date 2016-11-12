using System.Globalization;
using WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using Newtonsoft.Json;
using Identity.Membership;
using Identity.Membership.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class AdminLoginController : Controller
    {
        public AdminLoginController()
        {
        }

        public AdminLoginController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult AdminLogin()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminLogin(LoginRegisterViewModel model, string returnUrl)
        {
            //var IsPatient = (bool)ViewBag.IsPatient;


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var strContent = JsonConvert.SerializeObject(model);
            //var response = ApiConsumerHelper.PostData("api/Account/Login", strContent);
            //var resultTest = JsonConvert.DeserializeObject<SignInStatus>(response);

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true

            var user = await UserManager.FindAsync(model.LoginViewModel.Email, model.LoginViewModel.Password);
            if (user != null)
            {
                if (String.IsNullOrEmpty(returnUrl))
                {
                    if (UserManager.IsInRole(user.Id, "Admin"))
                    {
                        //return RedirectToLocal(returnUrl);
                        var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, shouldLockout: false);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                Session["LogedUserID"] = model.LoginViewModel.Email;
                                Session["LogedUserFullname"] = user.FirstName + " "+user.LastName;
                                return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                    }
                }
                return View(model);
            }

            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.IsPatient = true;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(LoginRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {

                    UserName = model.RegisterViewModel.Email,
                    Email = model.RegisterViewModel.Email,
                    FirstName = model.RegisterViewModel.FirstName,
                    LastName = model.RegisterViewModel.LastName,
                };

                // Add the Address properties:



                var result = await UserManager.CreateAsync(user, model.RegisterViewModel.Password);

                if (result.Succeeded)
                {
                    var IsPatient = ViewBag.IsPatient != null ? (bool)ViewBag.IsPatient : false;

                    var requestUri = IsPatient ? "api/Patients" : "api/Doctors";

                    requestUri = "api/Patients";//testing

                    var patient = new DataAccess.Patient();
                    patient.userId = user.Id;
                    patient.lastName = user.LastName;
                    patient.firstName = user.FirstName;
                    patient.email = user.Email;
                    var strContent = JsonConvert.SerializeObject(patient);
                    var response = ApiConsumerHelper.PostData(requestUri, strContent);
                    dynamic resultAdd = JsonConvert.DeserializeObject(response);



                    var userAssignRole = new UserAssignRoleModel();
                    userAssignRole.UserId = user.Id;//"8466ba63-b903-4d0a-8633-ce399ed1b542";//
                    userAssignRole.Role = "Patient";




                    strContent = JsonConvert.SerializeObject(userAssignRole);
                    response = ApiConsumerHelper.PostData("api/Roles/AssignRole", strContent);
                    resultAdd = JsonConvert.DeserializeObject(response);

                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id,
                        "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\""
                        + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form

            //return View("Login", model);
            return View("Login", model);
        }


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("AdminLogin", "AdminLogin");
        }
       
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Default", "Admin");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

           


            //public ActionResult logoutuser()
            //{
            //    Session["LogedUserID"] = null;
            //    return RedirectToAction("Index");

            //}

        }
        #endregion
    }
}
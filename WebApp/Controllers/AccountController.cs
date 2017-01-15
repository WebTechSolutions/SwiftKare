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
using DataAccess;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.PatientRepositories;
using System.Text;
using WebApp.Repositories.AdminRepository;
//Jam
namespace WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        string error = "";
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
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        // GET: /Account/PatientLogin
        [AllowAnonymous]
        public ActionResult PatientLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.error = TempData["error"];
            return View();
        }
        public ActionResult PatientSignup()
        {
           
            return View();
        }
        // GET: /Account/DoctorLogin
        [AllowAnonymous]
        public ActionResult DoctorLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        // GET: /Account/AdminLogin
        [AllowAnonymous]
        public ActionResult AdminLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
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
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRegisterViewModel model, string returnUrl)
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
            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    {

                        //var userId = HttpContext.User.Identity.GetUserId();
                        string userId = UserManager.FindByName(model.LoginViewModel.Email)?.Id;
                        SessionHandler.UserName = model.LoginViewModel.Email;
                        SessionHandler.Password = model.LoginViewModel.Password;
                        SessionHandler.UserId = userId;

                        var roles = UserManager.GetRoles(userId);
                        if (roles.Contains("Doctor"))
                        {
                            var objRepo = new DoctorRepository();
                            var doctor = objRepo.GetByUserId(userId);
                            var userModel = new UserInfoModel();
                            userModel.Id = doctor.doctorID;
                            userModel.Email = doctor.email;
                            userModel.FirstName = doctor.firstName;
                            userModel.LastName = doctor.lastName;
                            userModel.userId = doctor.userId;
                            userModel.title = doctor.title;
                            userModel.timeZone = doctor.timeZone;
                            userModel.role = doctor.role;
                            userModel.iOSToken = doctor.iOSToken;
                            userModel.AndroidToken = doctor.AndroidToken;
                            SessionHandler.UserInfo = userModel;

                            if (doctor.picture != null && doctor.picture.Count() > 0) {
                                SessionHandler.ProfilePhoto = Encoding.ASCII.GetString(doctor.picture);
                            }

                            if (doctor.active == null || (bool)doctor.active)
                                return RedirectToAction("DoctorTimings", "Doctor");
                        }
                        else if (roles.Contains("Patient"))
                        {
                            var objRepo = new PatientRepository();
                            var patient = objRepo.GetByUserId(userId);
                            var userModel = new UserInfoModel();
                            userModel.Id = patient.patientID;
                            userModel.Email = patient.email;
                            userModel.FirstName = patient.firstName;
                            userModel.LastName = patient.lastName;
                            userModel.userId = patient.userId;
                            userModel.title = patient.title;
                            userModel.timeZone = patient.timeZone;
                            userModel.role = patient.role;
                            userModel.iOSToken = patient.iOSToken;
                            userModel.AndroidToken = patient.AndroidToken;
                            SessionHandler.UserInfo = userModel;

                            if (patient.active == null || (bool)patient.active)
                                return RedirectToAction("Index", "Patient");
                        }
                        else if (roles.Contains("Admin"))
                        {
                            var user = await UserManager.FindAsync(model.LoginViewModel.Email, model.LoginViewModel.Password);
                            Session["LogedUserID"] = model.LoginViewModel.Email;
                            Session["LogedUserFullname"] = user.FirstName + " " + user.LastName;
                            return RedirectToAction("Default", "Admin");
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // POST: /Account/PatientLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PatientLogin(LoginRegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //var userId = HttpContext.User.Identity.GetUserId();
                        string userId = UserManager.FindByName(model.LoginViewModel.Email)?.Id;
                        SessionHandler.UserName = model.LoginViewModel.Email;
                        SessionHandler.Password = model.LoginViewModel.Password;
                        SessionHandler.UserId = userId;

                        var objRepo = new PatientRepository();
                        var patient = objRepo.GetByUserId(userId);
                        var userModel = new UserInfoModel();
                        userModel.Id = patient.patientID;
                        userModel.Email = patient.email;
                        userModel.FirstName = patient.firstName;
                        userModel.LastName = patient.lastName;
                        userModel.userId = patient.userId;
                        userModel.title = patient.title;
                        userModel.timeZone = patient.timeZone;
                        userModel.role = patient.role;
                        userModel.iOSToken = patient.iOSToken;
                        userModel.AndroidToken = patient.AndroidToken;
                        SessionHandler.UserInfo = userModel;

                        if (patient.picture != null && patient.picture.Count() > 0)
                        {
                            SessionHandler.ProfilePhoto = Encoding.ASCII.GetString(patient.picture);
                        }

                        if (patient.active == null || (bool)patient.active)
                            return RedirectToAction("Index", "Appointment");
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminLogin(LoginRegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //var userId = HttpContext.User.Identity.GetUserId();
                        string userId = UserManager.FindByName(model.LoginViewModel.Email)?.Id;
                        SessionHandler.UserName = model.LoginViewModel.Email;
                        SessionHandler.Password = model.LoginViewModel.Password;
                        SessionHandler.UserId = userId;

                        var objRepo = new AdminRepository();
                        var admin = objRepo.GetByUserId(userId);
                        var userModel = new UserInfoModel();
                        userModel.Id = admin.adminID;
                        userModel.Email = admin.email;
                        userModel.FirstName = admin.firstName;
                        userModel.LastName = admin.lastName;
                        SessionHandler.UserInfo = userModel;
                        if (admin.active == null || (bool)admin.active)
                            Session["LogedUserID"] = model.LoginViewModel.Email;
                            Session["LogedUserFullname"] = userModel.FirstName + " " + userModel.LastName;
                        return RedirectToAction("Default", "Admin");
                        
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        // POST: /Account/DoctorLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DoctorLogin(LoginRegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, model.LoginViewModel.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //var userId = HttpContext.User.Identity.GetUserId();
                        string userId = UserManager.FindByName(model.LoginViewModel.Email)?.Id;
                        SessionHandler.UserName = model.LoginViewModel.Email;
                        SessionHandler.Password = model.LoginViewModel.Password;
                        SessionHandler.UserId = userId;
                        var objRepo = new DoctorRepository();
                        var doctor = objRepo.GetByUserId(userId);
                        if (doctor == null)
                        {
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                        }
                        if (doctor.status == null || !((bool)doctor.status))
                        {
                            ModelState.AddModelError("", "Account review is in progress. You can login after approval.");
                            return View(model);
                        }

                        var userModel = new UserInfoModel();
                        userModel.Id = doctor.doctorID;
                        userModel.Email = doctor.email;
                        userModel.FirstName = doctor.firstName;
                        userModel.LastName = doctor.lastName;
                        userModel.userId = doctor.userId;
                        userModel.title = doctor.title;
                        userModel.timeZone = doctor.timeZone;
                        userModel.role = doctor.role;
                        userModel.iOSToken = doctor.iOSToken;
                        userModel.AndroidToken = doctor.AndroidToken;
                        SessionHandler.UserInfo = userModel;

                        if (doctor.picture != null && doctor.picture.Count() > 0)
                        {
                            SessionHandler.ProfilePhoto = Encoding.ASCII.GetString(doctor.picture);
                        }


                        if (doctor.active == null || (bool)doctor.active)
                            //return RedirectToAction("DoctorTimings", "Doctor");
                            return RedirectToAction("Index", "DoctorAppointment");

                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
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
                dynamic addedResult;
                if (result.Succeeded)
                {
                    SessionHandler.UserName = model.RegisterViewModel.Email;
                    SessionHandler.Password = model.RegisterViewModel.Password;
                    SessionHandler.UserId = user.Id;

                    if (model.IsPatient)
                    {
                        PatientRepository objRepo = new PatientRepository();
                        Patient obj = new Patient
                        {
                            userId = user.Id,
                            lastName = user.LastName,
                            firstName = user.FirstName,
                            email = user.Email
                        };
                        addedResult = objRepo.Add(obj);
                    }
                    else
                    {
                        DoctorRepository objRepo = new DoctorRepository();
                        Doctor obj = new Doctor
                        {
                            userId = user.Id,
                            lastName = user.LastName,
                            firstName = user.FirstName,
                            email = user.Email
                        };
                        addedResult = objRepo.Add(obj);
                    }
                    if (addedResult != null)
                    {
                        ViewBag.SuccessMessage = "Your Account has been created, please login";
                        return View("Login");
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form

            //return View("Login", model);
            return View("Login", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup(LoginRegisterViewModel model, string returnUrl)
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
                dynamic addedResult;
                if (result.Succeeded)
                {
                    SessionHandler.UserName = model.RegisterViewModel.Email;
                    SessionHandler.Password = model.RegisterViewModel.Password;
                    SessionHandler.UserId = user.Id;

                    
                        PatientRepository objRepo = new PatientRepository();
                        Patient obj = new Patient
                        {
                            userId = user.Id,
                            lastName = user.LastName,
                            firstName = user.FirstName,
                            email = user.Email
                        };
                        addedResult = objRepo.Add(obj);
                   
                    if (addedResult != null)
                    {
                        ViewBag.SuccessMessage = "Your Account has been created, please login";
                        return View("PatientLogin", model);
                    }
                }
                AddErrors(result);
                foreach (var item in result.Errors)
                {
                    error += item;
                    break;
                }
                
            }

            // If we got this far, something failed, redisplay form
            //return View("PatientLogin", model);
            TempData["error"] = error;
            return Redirect(Url.Action("PatientLogin", "Account") + "#signup");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDoctor(LoginRegisterViewModel model)
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
                dynamic addedResult;
                if (result.Succeeded)
                {
                    SessionHandler.UserName = model.RegisterViewModel.Email;
                    SessionHandler.Password = model.RegisterViewModel.Password;
                    SessionHandler.UserId = user.Id;

                    
                        DoctorRepository objRepo = new DoctorRepository();
                    Doctor obj = new Doctor
                    {
                        userId = user.Id,
                        lastName = user.LastName,
                        firstName = user.FirstName,
                        email = user.Email,
                        status = false
                        };
                        addedResult = objRepo.Add(obj);
                   
                    if (addedResult != null)
                    {
                        ViewBag.SuccessMessage = "Your Account has been created, You can login after approval of your account.";
                        //Send Simple Email

                        var sampleEmailBody = @"
                        <h3>Mail From SwiftKare</h3>
                        <p>Your account has been created with SwiftKare successfully.</p>
                        <p>You can login after approval of your account.</p>
                        <p>&nbsp;</p>
                        <p><strong>-Best Regards,<br/>SwiftKare</strong></p>
                        ";

                        var oSimpleEmail = new EmailHelper(obj.email, "SwiftKare Membership", sampleEmailBody);
                        oSimpleEmail.SendMessage();
                        return View("DoctorLogin", model);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form

            //return View("Login", model);
            return View("DoctorLogin", model);
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
                if (user != null)
                    user.EmailConfirmed = true;
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.ErrorMessage = "Your Account does't exist, please try again.";
                    return View("ForgotPassword");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                ForgotPasswordCodeModel.Token = code;

                var callbackUrl = Url.Action("Questions", "Account", new { email = model.Email, code = code }, protocol: Request.Url.Scheme);

                EmailHelper oHelper = new EmailHelper(user.Email, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                oHelper.SendMessage();

                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                return View("ForgotPasswordConfirmationLink");
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Questions(string email, string code)
        {
            var user = await UserManager.FindByNameAsync(email);
            ForgotPasswordCodeModel.Token = code;


            var objModel = new SecretQuestionModel();
            objModel.Email = user.UserName;
            var roles = UserManager.GetRoles(user.Id);
            Random rnd = new Random();
            int caseSwitch = rnd.Next(1, 4);
            if (roles.Contains("Patient"))
            {
                PatientRepository objRepo = new PatientRepository();
                var resultAdd = objRepo.GetByUserId(user.Id);
                switch (caseSwitch)
                {
                    case 1:
                        objModel.SecretQuestion = resultAdd.secretQuestion1;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer1;
                        break;
                    case 2:
                        objModel.SecretQuestion = resultAdd.secretQuestion2;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer2;
                        break;
                    default:
                        objModel.SecretQuestion = resultAdd.secretQuestion3;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer3;
                        break;
                }
            }
            else if (roles.Contains("Doctor"))
            {
                DoctorRepository objRepo = new DoctorRepository();
                var resultAdd = objRepo.GetByUserId(user.Id);
                switch (caseSwitch)
                {
                    case 1:
                        objModel.SecretQuestion = resultAdd.secretQuestion1;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer1;
                        break;
                    case 2:
                        objModel.SecretQuestion = resultAdd.secretQuestion2;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer2;
                        break;
                    default:
                        objModel.SecretQuestion = resultAdd.secretQuestion3;
                        objModel.SecretAnswerHidden = resultAdd.secretAnswer3;
                        break;
                }
            }


            return View("ForgotPasswordConfirmation", objModel);
        }


        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmationLink()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordConfirmation(SecretQuestionModel model)
        {
            var token = ForgotPasswordCodeModel.Token;
            if (model.SecretAnswerHidden.Trim().ToLower() == model.SecretAnswer.Trim().ToLower())
            {
                var newPassword = "New@Pa_" + System.Web.Security.Membership.GeneratePassword(10, 4);
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                //newPassword = "Admin@123";//comment when on live

                var result = await UserManager.ResetPasswordAsync(user.Id, token, newPassword);
                if (result.Succeeded)
                {
                    //send email there...
                    EmailHelper oHelper = new EmailHelper(user.Email, "Your password has been reset successfully.", "Your new temporary password is " + newPassword + ". Please change your password after login.");
                    oHelper.SendMessage();

                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                AddErrors(result);
            }
            else
            {
                ViewBag.ErrorMessage = "Your Answer does't match, please try again.";
            }
            //ModelState.AddModelError("", "Please enter valid answer!");

            return View("ForgotPasswordConfirmation", model);
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
            var roles = UserManager.GetRoles(SessionHandler.UserId);
            var actionName = "Login";
            if (roles.Contains("Doctor"))
                actionName = "DoctorLogin";
            else if (roles.Contains("Patient"))
                actionName = "PatientLogin";

            AuthenticationManager.SignOut();
            return RedirectToAction(actionName, "Account");
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
            return RedirectToAction("Index", "Home");
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
        }
        #endregion
    }
}

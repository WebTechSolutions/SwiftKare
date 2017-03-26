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
            if(returnUrl.Contains("Doctor"))
            {
                return RedirectToAction("DoctorLogin", "Account");
            }
            else if (!returnUrl.Contains("Doctor") && !returnUrl.Contains("Admin"))
            {
                return RedirectToAction("PatientLogin", "Account");
            }
            else if (returnUrl.Contains("Admin"))
            {
                return RedirectToAction("AdminLogin", "Account");
            }
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
                            userModel.timeZoneOffset = doctor.timeZoneoffset;
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
                            userModel.timeZoneOffset = patient.timeZoneoffset;
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
                        if(patient==null)
                        {
                            ModelState.AddModelError("", "Invalid login attempt.");
                            ViewBag.ModelError = "Invalid login attempt.";
                            return View(model);
                        }
                        var userModel = new UserInfoModel();
                        userModel.Id = patient.patientID;
                        userModel.Email = patient.email;
                        userModel.FirstName = patient.firstName;
                        userModel.LastName = patient.lastName;
                        userModel.userId = patient.userId;
                        userModel.title = patient.title;
                        userModel.timeZone = patient.timeZone;
                        userModel.timeZoneOffset = patient.timeZoneoffset;
                        userModel.role = patient.role;
                        userModel.iOSToken = patient.iOSToken;
                        userModel.AndroidToken = patient.AndroidToken;
                        SessionHandler.UserInfo = userModel;

                        if (patient.ProfilePhotoBase64 != null && patient.ProfilePhotoBase64.Count() > 0)
                        {
                            SessionHandler.ProfilePhoto = patient.ProfilePhotoBase64; //Encoding.ASCII.GetString(patient.picture);
                        }
                        else
                        {
                            SessionHandler.ProfilePhoto = "data:image/png;base64,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";
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
                    ViewBag.ModelError = "Invalid login attempt.";
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
                        if (admin == null)
                        {
                            ModelState.AddModelError("", "Invalid login attempt.");
                            ViewBag.ModelError = "Invalid login attempt.";
                            return View(model);
                        }
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
                    ViewBag.ModelError = "Invalid login attempt.";
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
                            ViewBag.ModelError = "Invalid login attempt.";
                            return View(model);
                        }
                        if (doctor.status == null || !((bool)doctor.status))
                        {
                            ModelState.AddModelError("", "Account review is in progress. You can login after approval.");
                            ViewBag.ModelError = "Account review is in progress. You can login after approval.";
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
                        userModel.timeZoneOffset = doctor.timeZoneoffset;
                        userModel.role = doctor.role;
                        userModel.iOSToken = doctor.iOSToken;
                        userModel.AndroidToken = doctor.AndroidToken;
                        SessionHandler.UserInfo = userModel;

                        if (doctor.ProfilePhotoBase64 != null && doctor.ProfilePhotoBase64.Count() > 0)
                        {
                            SessionHandler.ProfilePhoto = doctor.ProfilePhotoBase64;
                        }
                        else
                        {
                            SessionHandler.ProfilePhoto = "data:image/png;base64,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";
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
                    ViewBag.ModelError = "Invalid login attempt.";
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
                        //ViewBag.SuccessMessage = "Your Account has been created, please login";
                        ViewBag.error = "Your Account has been created, please login";
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
            ViewBag.ModelError += "\n" + error;
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
                        ViewBag.error = "Your Account has been created, You can login after approval of your account.";
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
                var newPassword = "New@Pa9_" + System.Web.Security.Membership.GeneratePassword(10, 4);
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
            var actionName = "";//"Login";
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
            ViewBag.ModelError = "";
            foreach (var error in result.Errors)
            {
               
                ModelState.AddModelError("", error);
                ViewBag.ModelError += error;
                break;
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

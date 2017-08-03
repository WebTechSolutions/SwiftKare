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
using DataAccess;
using System.Data.Entity;
using RestAPIs.Helper;

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
        public async Task<DataAccess.CustomModels.UserModel> Login(LoginApiModel model, HttpRequestMessage request)
        {
            var userModel = new DataAccess.CustomModels.UserModel
            {
                Email = model.Email
            };

            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }


            if (model.Role.ToLower() == "patient" || model.Role.ToLower() == "doctor")
            {

                try
                {
                    //    var id = headerValues.FirstOrDefault();
                    // This doen't count login failures towards lockout only two factor authentication
                    // To enable password failures to trigger lockout, change to shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                    var userId = UserManager.FindByName(model.Email)?.Id;
                    if (result == SignInStatus.Success)
                    {
                        SwiftKareDBEntities db = new SwiftKareDBEntities();
                        if (model.Role.ToLower() == "doctor")
                        {
                            var doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);
                            if (doctor != null)
                            {
                                userModel.Id = doctor.doctorID;
                                userModel.FirstName = doctor.firstName;
                                userModel.LastName = doctor.lastName;
                                userModel.Email = doctor.email;
                                userModel.userId = doctor.userId;
                                userModel.title = doctor.title;
                                userModel.timeZone = doctor.timezone;
                                userModel.userId = doctor.userId;
                                userModel.role = model.Role;
                                userModel.iOSToken = doctor.iOSToken;
                                userModel.AndroidToken = doctor.AndroidToken;
                            }
                            else
                            {
                                userModel.Errors = new List<string>();
                                userModel.Errors.Add("User does not exist with this role.");
                            }

                        }
                        else if (model.Role.ToLower() == "patient")
                        {
                            var patient = db.Patients.SingleOrDefault(o => o.userId == userId);
                          
                            if (patient != null)
                            {

                                userModel.Id = patient.patientID;
                                userModel.FirstName = patient.firstName;
                                userModel.LastName = patient.lastName;
                                userModel.userId = patient.userId;
                                userModel.title = patient.title;
                                userModel.timeZone = patient.timezone;
                                userModel.userId = patient.userId;
                                userModel.role = model.Role;
                                userModel.iOSToken = patient.iOSToken;
                                userModel.AndroidToken = patient.AndroidToken;
                            }
                            else
                            {
                                userModel.Errors = new List<string>();
                                userModel.Errors.Add("User does not exist with this role.");
                            }

                        }
                       
                    }
                    else if (result == SignInStatus.Failure)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Login fail, please try later");
                    }
                    else if (result == SignInStatus.LockedOut)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Account has been locked");
                    }
                    else if (result == SignInStatus.RequiresVerification)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Account need to verify");
                    }
                    // return result;
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
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent("Role is undefined"),
                    ReasonPhrase = "Undefined Role"
                };
                throw new HttpResponseException(resp);
            }
            if (userModel.Id <= 0 && userModel.Errors == null)
            {
                userModel.Errors = new List<string>();
                userModel.Errors.Add("Unexpected error from api/login");
            }

            return userModel;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("UniversalLogin")]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        public async Task<DataAccess.CustomModels.UserModel> UniversalLogin(PatientLoginApiModel model, HttpRequestMessage request)
        {
            
            var userModel = new DataAccess.CustomModels.UserModel
            {
                Email = model.Email
            };

            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }


            //if (model.Role.ToLower() == "patient" || model.Role.ToLower() == "doctor")
            //{

            try
            {
                //    var id = headerValues.FirstOrDefault();
                // This doen't count login failures towards lockout only two factor authentication
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email.Trim(), model.Password.Trim(), false, shouldLockout: false);
                if (result == SignInStatus.Success)
                {

                    if (model.offset != null)
                    {
                         if (model.offset.Equals("330")) model.offset = "-330";
                         if (model.offset.Trim().Equals("")) model.offset = "-300";
                    }
                    else model.offset = "-300";
                    

                    var userId = UserManager.FindByName(model.Email.Trim())?.Id;
                    var roleFromDb = UserManager.GetRoles(userId).FirstOrDefault();

                    SwiftKareDBEntities db = new SwiftKareDBEntities();
                    if (roleFromDb.ToString().ToLower() == "doctor")
                    {
                        string iOSToken = model.iOSToken;
                        string androidToken = model.andriodToken;

                        //update doctor table with  Tokens 
                        Doctor doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);
                        if (doctor != null)
                        {
                            if (model.offset != null)

                            {

                                if (model.offset.Trim() != "")
                                {
                                    if (doctor.timezoneoffset != model.offset)
                                    {
                                        model.offset = model.offset.Replace("+", "");
                                        DataAccess.TimeZone tz = db.TimeZones.FirstOrDefault(t => t.zoneOffset == model.offset);
                                        if (tz != null)
                                        {
                                            doctor.timezone = tz.zoneName;
                                            doctor.timezoneoffset = tz.zoneOffset;
                                        }
                                    }
                                }
                            }
                            if (iOSToken.Trim() != "") doctor.iOSToken = iOSToken;
                            if (androidToken.Trim() != "") doctor.AndroidToken = androidToken;
                            db.Entry(doctor).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        // var doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);

                        if (doctor != null)
                        {
                            if (doctor.status == null || doctor.status == false)
                            {
                                userModel.Errors = new List<string>();
                                userModel.Errors.Add("Account review is in progress. You can login after approval.");
                            }
                            else
                            {
                                userModel.Id = doctor.doctorID;
                                userModel.FirstName = doctor.firstName;
                                userModel.LastName = doctor.lastName;
                                userModel.Email = doctor.email;
                                userModel.title = doctor.title;
                                userModel.timeZone = doctor.timezoneoffset;// timezoneoffset
                                userModel.userId = doctor.userId;
                                userModel.role = roleFromDb.ToString();
                                userModel.iOSToken = doctor.iOSToken;
                                userModel.AndroidToken = doctor.AndroidToken;
                               

                            }

                        }
                        else
                        {
                            userModel.Errors = new List<string>();
                            userModel.Errors.Add("User does not exist with this role.");
                        }

                    }
                    else if (roleFromDb.ToString().ToLower() == "patient")
                    {
                        string iOSToken = model.iOSToken;
                        string androidToken = model.andriodToken;
                        //update patient table with  Tokens 
                        Patient patient = db.Patients.SingleOrDefault(o => o.userId == userId);
                        if (model.offset != null)
                        {
                            if (model.offset.Trim() != "")
                            {
                                if (patient.timezoneoffset != model.offset)
                                {
                                    model.offset = model.offset.Replace("+", "");
                                    DataAccess.TimeZone tz = db.TimeZones.FirstOrDefault(t => t.zoneOffset == model.offset);
                                    if (tz != null)
                                    {
                                        patient.timezone = tz.zoneName;
                                        patient.timezoneoffset = tz.zoneOffset;
                                    }
                                }
                            }


                        }
                        if (iOSToken.Trim() != "") patient.iOSToken = iOSToken;
                        if (androidToken.Trim() != "") patient.AndroidToken = androidToken;
                        db.Entry(patient).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        if (patient != null)
                        {

                            userModel.Id = patient.patientID;
                            userModel.FirstName = patient.firstName;
                            userModel.LastName = patient.lastName;
                            //  userModel.userId = patient.userId;
                            userModel.title = patient.title;
                            userModel.timeZone = patient.timezoneoffset;
                            userModel.userId = patient.userId;
                            userModel.role = roleFromDb.ToString();
                            userModel.iOSToken = patient.iOSToken;
                            userModel.AndroidToken = patient.AndroidToken;
                            userModel.pictureUrl = System.Configuration.ConfigurationManager.AppSettings["profilePictureURL"].ToString();
                        }
                        else
                        {
                            userModel.Errors = new List<string>();
                            userModel.Errors.Add("User does not exist with this role.");
                        }

                    }

                }
                else if (result == SignInStatus.Failure)
                {
                    userModel.Errors = new List<string>();
                    userModel.Errors.Add("Login fail,Incorrect User name or Password.");
                }
                else if (result == SignInStatus.LockedOut)
                {
                    userModel.Errors = new List<string>();
                    userModel.Errors.Add("Account has been locked");
                }
                else if (result == SignInStatus.RequiresVerification)
                {
                    userModel.Errors = new List<string>();
                    userModel.Errors.Add("Account need to verify");
                }
            }

            catch (Exception ex)
            {
                userModel.Errors = new List<string>();
                userModel.Errors.Add("Exception Occur:"+ex.Message);
                //userModel.Errors.Add(model.Email + "," + model.Password + "," + model.offset + "," + model.iOSToken);
                return userModel;
                /* throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                     {
                         //Content = new StringContent("An error occurred while posting in api/account/login, please try again or contact the administrator."),
                         Content = new StringContent(ex.Message),
                         ReasonPhrase = ex.Message

                     });
                 }*/
            }
            //}
            //else
            //{
                //var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                //{
                //    Content = new StringContent("Role is undefined"),
                //    ReasonPhrase = "Undefined Role"
                //};
                //throw new HttpResponseException(resp);
            //}
            if (userModel.Id <= 0 && userModel.Errors == null)
            {
                userModel.Errors = new List<string>();
                userModel.Errors.Add("Unexpected error from api/login");
            }
            return userModel;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("DoctorLogin")]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        public async Task<DataAccess.CustomModels.UserModel> DoctorLogin(DoctorLoginApiModel model, HttpRequestMessage request)
        {
            var userModel = new DataAccess.CustomModels.UserModel
            {
                Email = model.Email
            };

            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }


           // if (model.Role.ToLower() == "patient" || model.Role.ToLower() == "doctor")
            //{

                try
                {
                    //    var id = headerValues.FirstOrDefault();
                    // This doen't count login failures towards lockout only two factor authentication
                    // To enable password failures to trigger lockout, change to shouldLockout: true
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                    var userId = UserManager.FindByName(model.Email)?.Id;
                    if (result == SignInStatus.Success)
                    {
                        SwiftKareDBEntities db = new SwiftKareDBEntities();
                        //if (model.Role.ToLower() == "doctor")
                        //{
                            var doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);
                            if (doctor != null)
                            {
                                userModel.Id = doctor.doctorID;
                                userModel.FirstName = doctor.firstName;
                                userModel.LastName = doctor.lastName;
                                userModel.Email = doctor.email;
                                userModel.userId = doctor.userId;
                                userModel.title = doctor.title;
                                userModel.timeZone = doctor.timezone;
                                userModel.userId = doctor.userId;
                                userModel.role = "Doctor";
                                userModel.iOSToken = doctor.iOSToken;
                                userModel.AndroidToken = doctor.AndroidToken;
                            }
                            else
                            {
                                userModel.Errors = new List<string>();
                                userModel.Errors.Add("User does not exist with this role.");
                            }

                       // }
                        

                    }
                    else if (result == SignInStatus.Failure)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Login fail, please try later");
                    }
                    else if (result == SignInStatus.LockedOut)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Account has been locked");
                    }
                    else if (result == SignInStatus.RequiresVerification)
                    {
                        userModel.Errors = new List<string>();
                        userModel.Errors.Add("Account need to verify");
                    }
                    // return result;
                }

                catch (Exception)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("An error occurred while posting in api/account/login, please try again or contact the administrator."),
                        ReasonPhrase = "Critical Exception"
                    });
                }
            //}
            //else
            //{
            //    var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
            //    {
            //        Content = new StringContent("Role is undefined"),
            //        ReasonPhrase = "Undefined Role"
            //    };
            //    throw new HttpResponseException(resp);
            //}
            if (userModel.Id <= 0 && userModel.Errors == null)
            {
                userModel.Errors = new List<string>();
                userModel.Errors.Add("Unexpected error from api/login");
            }

            return userModel;
        }


        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<DataAccess.CustomModels.UserModel> Register(RegisterApiModel model, HttpRequestMessage request)
        {

            var userModel = new DataAccess.CustomModels.UserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }


            if (model.Role.ToLower() == "patient" || model.Role.ToLower() == "doctor")
            {
                try
                {

                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        SwiftKareDBEntities db = new SwiftKareDBEntities();

                        if (model.Role.ToLower() == "patient")
                        {
                            var resultRole = await UserManager.AddToRoleAsync(user.Id, "Patient");
                            var patient = new Patient
                            {
                                userId = user.Id,
                                lastName = model.LastName,
                                firstName = model.FirstName,
                                email = user.Email,
                                active=true
                            };
                            db.Patients.Add(patient);
                            await db.SaveChangesAsync();
                            userModel.Id = patient.patientID;

                            //add the patient
                        }
                        else if (model.Role.ToLower() == "doctor")
                        {
                            var resultRole = await UserManager.AddToRoleAsync(user.Id, "Doctor");
                            var doctor = new Doctor
                            {
                                userId = user.Id,
                                lastName = model.LastName,
                                firstName = model.FirstName,
                                email = user.Email,
                                active=true,
                                status = false
                            };
                            db.Doctors.Add(doctor);
                            await db.SaveChangesAsync();
                            userModel.Id = doctor.doctorID;
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        userModel.Errors = result.Errors.ToList();
                    }


                    return userModel;
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
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent("Role is undefined"),
                    ReasonPhrase = "Undefined Role"
                };
                throw new HttpResponseException(resp);
            }






        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<DataAccess.CustomModels.ForgotModel> ForgotPassword(ForgotApiModel model, HttpRequestMessage request)
        {
            var objModel = new DataAccess.CustomModels.ForgotModel { Email = model.Email };
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }

            if (model.Role.ToLower() == "patient" || model.Role.ToLower() == "doctor")
            {

                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = await UserManager.FindByNameAsync(model.Email);
                        if (user == null)
                        {
                            // Don't reveal that the user does not exist or is not confirmed
                            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                Content = new StringContent("user is not exist with this email address or email is not confirmed"),
                                ReasonPhrase = "Not Confirmed"
                            };
                            throw new HttpResponseException(resp);
                        }
                        SwiftKareDBEntities db = new SwiftKareDBEntities();
                        Random rnd = new Random();
                        int caseSwitch = rnd.Next(1, 4);
                        if (model.Role.ToLower() == "doctor")
                        {
                            Doctor doctor = db.Doctors.SingleOrDefault(o => o.userId == user.Id);
                            switch (caseSwitch)
                            {
                                case 1:
                                    objModel.SecretQuestion = doctor.secretQuestion1;
                                    objModel.SecretAnswer = doctor.secretAnswer1;
                                    break;
                                case 2:
                                    objModel.SecretQuestion = doctor.secretQuestion2;
                                    objModel.SecretAnswer = doctor.secretAnswer2;
                                    break;
                                default:
                                    objModel.SecretQuestion = doctor.secretQuestion3;
                                    objModel.SecretAnswer = doctor.secretAnswer3;
                                    break;
                            }
                        }
                        else if (model.Role.ToLower() == "patient")
                        {
                            Patient patient = db.Patients.SingleOrDefault(o => o.userId == user.Id);
                            switch (caseSwitch)
                            {
                                case 1:
                                    objModel.SecretQuestion = patient.secretQuestion1;
                                    objModel.SecretAnswer = patient.secretAnswer1;
                                    break;
                                case 2:
                                    objModel.SecretQuestion = patient.secretQuestion2;
                                    objModel.SecretAnswer = patient.secretAnswer2;
                                    break;
                                default:
                                    objModel.SecretQuestion = patient.secretQuestion3;
                                    objModel.SecretAnswer = patient.secretAnswer3;
                                    break;
                            }
                        }
                        else
                        {
                            var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                            {
                                Content = new StringContent("Role is undefined"),
                                ReasonPhrase = "Undefined Role"
                            };
                            throw new HttpResponseException(resp);
                        }



                        // var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        // return code;
                    }

                    // If we got this far, something failed, redisplay form
                    //return "";
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
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent("Role is undefined"),
                    ReasonPhrase = "Undefined Role"
                };
                throw new HttpResponseException(resp);
            }

            return objModel;
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPasswordUniversal")]
        public async Task<DataAccess.CustomModels.ForgotModel> ForgotPasswordUniversal(ForgotApiModelUniversal model, HttpRequestMessage request)
        {
            var objModel = new DataAccess.CustomModels.ForgotModel { url = "https://13.64.233.80/Account/ForgotPassword" };
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }

              // try
              //  {
              //      if (ModelState.IsValid)
              //      {
              //          var user = await UserManager.FindByNameAsync(model.Email);
              //          if (user == null)
              //          {
              //              // Don't reveal that the user does not exist or is not confirmed
              //              var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
              //              {
              //                  Content = new StringContent("user doesnot exist with this email address or email is not confirmed"),
              //                  ReasonPhrase = "Not Confirmed"
              //              };
              //              throw new HttpResponseException(resp);
              //          }
              //     var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
              //var   callbackUrl = "https://13.64.233.80/"+ Url.Route("Default", new { Controller = "Account", Action = "Questions", email = model.Email, code = code });

              //      EmailHelper oHelper = new EmailHelper(user.Email, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
              //      oHelper.SendMessage();
                    
              //      } 

              //  }
              //  catch (Exception ex)
              //  {
              //      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
              //      {
              //          Content = new StringContent("An error occurred while posting in api/account/ForgotPassword, please try again or contact the administrator."),
              //          ReasonPhrase = ex.ToString()
              //      });
              //  }
            

            return objModel;
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<DataAccess.CustomModels.ResetPasswordModel> ResetPassword(ResetPasswordViewModel model, HttpRequestMessage request)
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
            var objModel = new DataAccess.CustomModels.ResetPasswordModel();

            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);

                model.Code= UserManager.GeneratePasswordResetToken(user.Id);

                if (user != null)
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        objModel.Messages = new List<string>();
                        objModel.Messages.Add("Your Password has been reset, please try!...");
                    }
                    else
                    {
                        objModel.Messages = result.Errors.ToList();
                    }
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

            return objModel;

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

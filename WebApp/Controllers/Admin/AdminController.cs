using System;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using DataAccess;
using SwiftKare.Models.Utilities;
using Identity.Membership.Models;
using Microsoft.AspNet.Identity;
using Identity.Membership;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using WebApp.Helper;
using System.Data.Entity;
using WebApp;

namespace SwiftKare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
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

        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {


                try
                {
                    var admin = db.SP_SelectAdmin();
                    var roles = db.AspNetRoles.ToList();
                    ViewBag.Roles=roles;
                    return View(admin);

                }
                catch (Exception ex)
                {
                    ViewBag.successMessage = "";
                    ViewBag.errorMessage = "Error occurred while loading data.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }

        }



        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {

                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                var id = "";
                var firstName = "";
                var lastName = "";
                var email = "";
                var password = "";
                var roleID = "";
               

                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {

                        firstName = Request.Form["firstname"].ToString();
                        lastName = Request.Form["lastname"].ToString();
                        email = Request.Form["email"].ToString();
                        password = Request.Form["password"].ToString();
                        //roleID = Request.Form["sltRole"].ToString();
                        var roles = db.AspNetRoles.ToList();
                        //if (roleID == "")
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "Select valid Role";
                        //    var _existingadminList = db.SP_SelectAdmin();
                        //    //var roles = db.Roles
                        //    //  .Where(a => a.active == true).ToList();

                        //    ViewBag.Roles = roles;
                        //    return View(_existingadminList);
                        //}
                        if (!Regex.IsMatch(firstName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid First Name";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = roles;
                            return View(_existingadminList);
                        }
                        if (!Regex.IsMatch(lastName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Last Name";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //.Where(a => a.active == true).ToList();
                            ViewBag.Roles = roles;

                            return View(_existingadminList);
                        }
                        Utility util = new Utility();
                        if (!(util.IsValid(email)))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Email Address";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = roles;
                            return View(_existingadminList);
                        }
                        var checkemail = (
                                   from p in db.AdminUsers
                                   where (p.email == email && p.active == true)
                                   select p
                               ).FirstOrDefault();
                        if (checkemail == null)
                        {
                            var user = new ApplicationUser
                            {

                                UserName = email,
                                Email = email,
                                FirstName = firstName,
                                LastName = lastName,

                            };
                            var result = await UserManager.CreateAsync(user, password);

                            if (result.Succeeded)
                            {
                                db.SP_AddAdmin(firstName, lastName, email, user.Id, Session["LogedUserID"].ToString());
                                db.SaveChanges();

                                var userAssignRole = new UserAssignRoleModel();
                                userAssignRole.UserId = user.Id;//"8466ba63-b903-4d0a-8633-ce399ed1b542";//
                                userAssignRole.Role = "Admin";
                                var strContent = JsonConvert.SerializeObject(userAssignRole);
                                var response = ApiConsumerHelper.PostData("api/Roles/AssignRole", strContent);
                                dynamic resultAdd = JsonConvert.DeserializeObject(response);

                                ViewBag.successMessage = "Record has been saved successfully";
                                ViewBag.errorMessage = "";
                            }
                            else
                            {
                                ViewBag.successMessage = "";
                                foreach (var error in result.Errors)
                                {
                                    ViewBag.errorMessage = error;
                                }

                                var _existingadminList = db.SP_SelectAdmin();
                                return View(_existingadminList);
                            }
                        }
                        else
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "User with this Email Address already exists";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = roles;
                            return View(_existingadminList);
                        }



                    }
                    if (action == "edit")
                    {

                        id = Request.Form["id"].ToString();
                        firstName = Request.Form["firstName"].ToString();
                        lastName = Request.Form["lastName"].ToString();
                        email = Request.Form["email"].ToString();
                        password = Request.Form["password"].ToString();
                        //roleID = Request.Form["sltRole"].ToString();
                        var rroles = db.AspNetRoles.ToList();
                        //if (roleID == "")
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "Select valid Role";
                        //    var _existingadminList = db.SP_SelectAdmin();
                        //    //var roles = db.Roles
                        //    //  .Where(a => a.active == true).ToList();

                        //    ViewBag.Roles = rroles;
                        //    return View(_existingadminList);
                        //}
                        if (!Regex.IsMatch(firstName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid First Name";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //  .Where(a => a.active == true).ToList();

                            ViewBag.Roles = rroles;
                            return View(_existingadminList);
                        }
                        if (!Regex.IsMatch(lastName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Last Name";
                            var _existingadminList = db.SP_SelectAdmin();
                            // var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = rroles;
                            return View(_existingadminList);
                        }
                        Utility util = new Utility();
                        if (!(util.IsValid(email)))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Email Address";
                            var _existingadminList = db.SP_SelectAdmin();
                            // var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = rroles;
                            return View(_existingadminList);
                        }
                      
                      
                        var checkemail = (
                               from p in db.AspNetUsers
                               where (p.Email == email && p.Id!=id )
                               select p
                           ).FirstOrDefault();
                        if (checkemail == null )
                        {
                           
                            
                            string token = await UserManager.GeneratePasswordResetTokenAsync(id);
                            var result = await UserManager.ResetPasswordAsync(id, token, password);
                            
                            if (result.Succeeded)
                            {
                                AdminUser adminUser = db.AdminUsers.Where(a=>a.userId==id).FirstOrDefault();
                                if (adminUser != null)
                                {
                                    //Update AdminUsers table
                                    adminUser.lastName = lastName;
                                    adminUser.FirstName = firstName;
                                    adminUser.email = email;
                                    adminUser.mb = Session["LogedUserID"].ToString();
                                    adminUser.md = DateTime.Now;
                                    db.AdminUsers.Add(adminUser);
                                    db.Entry(adminUser).State = EntityState.Modified;
                                }
                                //Update AspNetUsers table
                                AspNetUser aspnetUser = await db.AspNetUsers.FindAsync(id);
                                if (aspnetUser == null)
                                {
                                    ViewBag.successMessage = "";
                                    ViewBag.errorMessage = "Admin user not found.";
                                    return View();
                                }
                                aspnetUser.LastName = lastName;
                                aspnetUser.FirstName = firstName;
                                aspnetUser.Email = email;
                                db.AspNetUsers.Add(aspnetUser);
                                db.Entry(aspnetUser).State = EntityState.Modified;
                               
                                await db.SaveChangesAsync();
                                ViewBag.successMessage = "Record has been saved successfully";
                                ViewBag.errorMessage = "";
                               
                            }
                            else
                            {
                                ViewBag.successMessage = "";
                                foreach (var error in result.Errors)
                                {
                                    ViewBag.errorMessage = error;
                                }
                                var _existingadminList = db.SP_SelectAdmin();
                                return View(_existingadminList);
                            }
                           
                        }
                        else
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "User with this Email Address already exists";
                            var _existingadminList = db.SP_SelectAdmin();
                            //var roles = db.Roles
                            //.Where(a => a.active == true).ToList();

                            ViewBag.Roles = rroles;
                            return View(_existingadminList);
                        }
                    }
                    if (action == "delete")
                    {

                        id = Request.Form["id"].ToString();
                        var userid = Request.Form["userid"].ToString();
                        AdminUser adminUser = db.AdminUsers.Where(a => a.userId == userid).FirstOrDefault();
                        if (adminUser != null)
                        {
                            //Update AdminUsers table
                            adminUser.active = false;
                            adminUser.mb = Session["LogedUserID"].ToString();
                            adminUser.md = DateTime.Now;
                            db.AdminUsers.Add(adminUser);
                            db.Entry(adminUser).State = EntityState.Modified;
                            ViewBag.successMessage = "Record has been deleted successfully";
                            ViewBag.errorMessage = "";
                        }
                        //db.sp_DeleteAdmin(Convert.ToInt64(id), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        //AspNetUser admin = db.AspNetUsers.Find(userid);
                        
                        //db.AspNetUsers.Remove(admin);
                        
                        //db.AspNetUsers.Remove(admin);
                        //db.SaveChanges();
                        else
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Admin user not found.";
                        }
                        

                    }
                    //Send Email//


                    //Send Email//
                    var __existingadminList = db.SP_SelectAdmin();
                    // var _roles = db.Roles
                    //.Where(a => a.active == true).ToList();
                    var _roles = db.AspNetRoles.ToList();
                    ViewBag.Roles = _roles;
                    return View(__existingadminList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingadminList = db.SP_SelectAdmin();
                    var roles = db.AspNetRoles.ToList();
                    ViewBag.Roles = roles;
                    return View(_existingadminList);
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }

        }

        private void SendActivationEmail(int userId)
        {
            //add userid and activation code to db//

            //add userid and dbcode to db//
            string activationCode = Guid.NewGuid().ToString();
            //// Create the email object first, then add the properties.
            //var myMessage = new SendGridMessage();

            //// Add the message properties.
            //myMessage.From = new MailAddress("rushaliwatane1991@gmail.com");

            //// Add multiple addresses to the To field.
            //myMessage.AddTo("tech@maestro-control.com");

            //myMessage.Subject = "Account Activation";

            ////Add the HTML and Text bodies
            //myMessage.Html = "<p>Hello " + txtUsername.Text.Trim() + ",";
            //    string body = "Hello " + txtUsername.Text.Trim() + ",";
            //    body += "<br /><br />Please click the following link to activate your account";
            //    body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("CS.aspx", "CS_Activation.aspx?ActivationCode=" + activationCode) + "'>Click here to activate your account.</a>";
            //    body += "<br /><br />Thanks";
            //    mm.Body = body;
            //    mm.IsBodyHtml = true;
            //myMessage.Text = "Your Password is";

            //// Create network credentials to access your SendGrid account
            //var username = "azure_b284591f27317e98d93ad4cf19eac30f@azure.com";
            //var pswd = "f9H3sHa7oQvrhXV";

            ////var username = System.Environment.GetEnvironmentVariable("SENDGRID_USER");
            ////var pswd = System.Environment.GetEnvironmentVariable("SENDGRID_PASS");
            ////var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");



            //var credentials = new NetworkCredential(username, pswd);
            //// Create an Web transport for sending email.
            //var transportWeb = new Web(credentials);         
            //// Send the email, which returns an awaitable task.
            //transportWeb.DeliverAsync(myMessage);
         
        }

        //
        // POST: /Doctor/Edit/5

       
        public ActionResult Default()
        {
            return View();

        }

    }
}

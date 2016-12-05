using System;
using System.Web.Mvc;
using SwiftKare.Models.Utilities;
using System.Text.RegularExpressions;
using DataAccess;
using Identity.Membership.Models;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using WebApp;
using System.Data.Entity;

namespace SwiftKare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Doctor_AdminController : Controller
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
                    var doc = db.SP_SelectDoctor();
                    return View(doc);

            }
            catch (Exception ex)
            {
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
                var userid = "";
                var firstName = "";
                var lastName = "";
                var email = "";
                var password = "";
                bool isAllValid = true;

                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {

                        firstName = Request.Form["firstName"].ToString();
                        lastName = Request.Form["lastName"].ToString();
                        email = Request.Form["email"].ToString();
                        password = Request.Form["password"].ToString();
                        if (!Regex.IsMatch(firstName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid First Name";
                            var _existingdList = db.SP_SelectDoctor();
                            return View(_existingdList);
                        }
                        if (!Regex.IsMatch(lastName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Last Name";
                            var _existingdList = db.SP_SelectDoctor();
                            return View(_existingdList);
                        }
                        Utility util = new Utility();
                        if (!(util.IsValid(email)))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Email Address";
                            var _existingdList = db.SP_SelectDoctor();
                            return View(_existingdList);
                        }

                        //db.SP_AddDoctor(firstName, lastName, email, password, Session["LogedUserID"].ToString());
                        //db.SaveChanges();
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
                           var doctor = new DataAccess.Doctor();
                            doctor.userId = user.Id;
                            doctor.lastName = user.LastName;
                            doctor.firstName = user.FirstName;
                            doctor.email = user.Email;
                            doctor.cb = Session["LogedUserID"].ToString();
                            doctor.active = true;
                            doctor.status = false;
                            db.Doctors.Add(doctor);
                            db.SaveChanges();

                            var userAssignRole = new UserAssignRoleModel();
                            userAssignRole.UserId = user.Id;//"8466ba63-b903-4d0a-8633-ce399ed1b542";//
                            userAssignRole.Role = "Doctor";


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
                            var _existingdList = db.SP_SelectDoctor();
                            return View(_existingdList);
                        }

                    }
                    if (action == "edit")
                    {

                        id = Request.Form["id"].ToString();
                        userid= Request.Form["userid"].ToString();
                        password = Request.Form["password"].ToString();
                        string token = await UserManager.GeneratePasswordResetTokenAsync(userid);
                        //var firstName = Request.Form["firstName"].ToString();
                        //var lastName = Request.Form["lastName"].ToString();
                        //var email = Request.Form["email"].ToString();
                        //db.sp_ResetDoctorPassword(Convert.ToInt64(id), password, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        //db.SaveChanges();
                        var result = await UserManager.ResetPasswordAsync(userid,token,password);

                        if (result.Succeeded)
                        {
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
                            var _existingdList = db.SP_SelectDoctor();
                            return View(_existingdList);
                        }

                    }
                    if (action == "delete")
                    {
                        id = Request.Form["id"].ToString();
                        userid = Request.Form["userid"].ToString();
                        Doctor doc = db.Doctors.Where(a => a.userId == userid).FirstOrDefault();
                        if (doc != null)
                        {
                           
                            doc.active = false;
                            doc.mb = Session["LogedUserID"].ToString();
                            doc.md = DateTime.Now;
                            db.Doctors.Add(doc);
                            db.Entry(doc).State = EntityState.Modified;
                            ViewBag.successMessage = "Record has been deleted successfully";
                            ViewBag.errorMessage = "";
                        }
                        else
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Doctor not found.";
                        }

                    }


                    var _existingdoctorsList = db.SP_SelectDoctor();
                    return View(_existingdoctorsList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }

        }

      

        //
        // POST: /Doctor/Edit/5

        public ActionResult DoctorsForApproval()
         {
            if (Session["LogedUserID"] != null)
            {


                try
                {
                     var doc = db.SP_SelectDoctorsForApproval();
                     return View(doc);
                 }
                 catch (Exception ex)
                 {
                     return View();
                 }
        }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
    }


}


         [HttpPost]
         public ActionResult ApproveDoctor(FormCollection collection)
         {
    if (Session["LogedUserID"] != null)
    {


        try
        {
                     var id = Request.Form["id"].ToString();
                     db.sp_ApproveDoctor(Convert.ToInt64(id),Session["LogedUserID"].ToString(), System.DateTime.Now);
                     db.SaveChanges();
                     return RedirectToAction("DoctorsForApproval");
                 }
                 catch (Exception ex)
                 {
                     return RedirectToAction("DoctorsForApproval");
                 }
}
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }
            

         }

         public ActionResult Report()
         {
    if (Session["LogedUserID"] != null)
    {


        try
        {
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
}
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }
         }
         [CustomAuthorizationAttribute]
        [HttpPost]
         public ActionResult Report(FormCollection form)
         {
    if (Session["LogedUserID"] != null)
    {


        try
        {
                     var datefrom=Request.Form["datefrom"].ToString();
                     var dateto = Request.Form["dateto"].ToString();
                     //var doc=db.SP_DoctorReport(DateTime.ParseExact(datefrom, "dd/MM/yyyy", null), DateTime.ParseExact(dateto, "dd/MM/yyyy", null));
                     var doc = db.SP_DoctorReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto));
                     return View(doc);
                 }
                 catch (Exception ex)
                 {
                     return View();
                 }
}
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }
            
         }

        
    }
}

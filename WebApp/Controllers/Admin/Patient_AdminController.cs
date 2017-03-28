
using DataAccess;
using Identity.Membership;
using Identity.Membership.Models;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SwiftKare.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class Patient_AdminController : Controller
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
                    var _patientView = db.SP_SelectPatient();
                    return View(_patientView);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while loading data.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("AdminLogin", "Account");
            }


        }

        //
        // POST: /Doctor/Create

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(FormCollection collection)
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
            if (Session["LogedUserID"] != null)
            {

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
                            var _existingpList = db.SP_SelectPatient();
                            return View(_existingpList);
                        }
                        if (!Regex.IsMatch(lastName, @"^[a-zA-Z\s]+$"))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Last Name";
                            var _existingpList = db.SP_SelectPatient();
                            return View(_existingpList);
                        }
                        Utility util = new Utility();
                        if (!(util.IsValid(email)))
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Provide valid Email Address";
                            var _existingpList = db.SP_SelectPatient();
                            return View(_existingpList);
                        }

                        
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
                            //var patient = new DataAccess.Patient();
                            //patient.userId = user.Id;
                            //patient.lastName = user.LastName;
                            //patient.firstName = user.FirstName;
                            //patient.email = user.Email;
                            //patient.cb = Session["LogedUserID"].ToString();
                            //patient.active = true;
                            //db.Patients.Add(patient);
                            //db.SaveChanges();
                            db.SP_AddPatient(firstName, lastName, email, user.Id,Session["LogedUserID"].ToString());
                            db.SaveChanges();

                            var userAssignRole = new UserAssignRoleModel();
                            userAssignRole.UserId = user.Id;//"8466ba63-b903-4d0a-8633-ce399ed1b542";//
                            userAssignRole.Role = "Patient";




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
                            var _existingpList = db.SP_SelectPatient();
                            return View(_existingpList);
                        }

                    }
                    if (action == "edit")
                    {

                        id = Request.Form["id"].ToString();
                        userid = Request.Form["userid"].ToString();
                        password = Request.Form["password"].ToString();
                        string token = await UserManager.GeneratePasswordResetTokenAsync(userid);
                        //var firstName = Request.Form["firstName"].ToString();
                        //var lastName = Request.Form["lastName"].ToString();
                        //var email = Request.Form["email"].ToString();
                        //db.sp_ResetPatientPassword(Convert.ToInt64(id), password, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        // db.SaveChanges();
                        var result = await UserManager.ResetPasswordAsync(userid, token, password);

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
                            var _existingpList = db.SP_SelectPatient();
                            return View(_existingpList);
                        }

                    }
                    if (action == "delete")
                    {
                        long pid = Convert.ToInt64(Request.Form["id"].ToString());
                        userid = Request.Form["userid"].ToString();
                        Patient patient = db.Patients.Where(a => a.patientID == pid).FirstOrDefault();
                        if (patient != null)
                        {
                            //Update AdminUsers table
                            patient.active = false;
                            patient.mb = Session["LogedUserID"].ToString();
                            patient.md = DateTime.Now;
                            db.Entry(patient).State = EntityState.Modified;
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been deleted successfully";
                            ViewBag.errorMessage = "";
                        }
                        else
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Patient not found.";
                        }

                    }


                    var _existingpatientsList = db.SP_SelectPatient();
                    return View(_existingpatientsList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("AdminLogin", "Account");
            }


        }

        //
        // POST: /Doctor/Edit/5

        public ActionResult Report()
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

        [HttpPost]
      
        public ActionResult Report(FormCollection form)
        {
           

                try
                {
                    var datefrom = Request.Form["datefrom"].ToString();
                    var dateto = Request.Form["dateto"].ToString();
                string fromdateString = datefrom.Trim();
                string todateString = dateto.Trim();
                string format = "dd/MM/yyyy";
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime fd = DateTime.ParseExact(fromdateString, format, provider);
                DateTime td = DateTime.ParseExact(todateString, format, provider);
                var pat = db.SP_PatientReport(fd, td);
                    return View(pat);
                }
                catch (Exception ex)
                {
                    return View();
                }
            
        }
    }

   
}

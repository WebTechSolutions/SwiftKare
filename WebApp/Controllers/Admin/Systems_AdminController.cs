using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class Systems_AdminController : Controller
    {
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var system = db.SP_SelectSystems();
                    return View(system);

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

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                var systemname = "";
                var systemid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        systemname = Request.Form["systemname"].ToString();
                        var system = (
                                       from p in db.PatientSystems
                                       where (p.systemName == systemname && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (system != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "System already exists";

                        }
                        if (system == null)
                        {
                            db.SP_AddSystems(systemname, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        systemid = Request.Form["id"].ToString();
                        systemname = Request.Form["systemname"].ToString();
                        //var system = (
                        //               from p in db.Systems
                        //               where (p.systemName == systemname && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (system != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "System already exists";

                        //}
                        //if (system == null)
                        //{
                        db.sp_UpdateSystems(Convert.ToInt64(systemid), systemname, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        systemid = Request.Form["id"].ToString();
                        db.sp_DeleteSystems(Convert.ToInt64(systemid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingsystemList = db.SP_SelectSystems();
                    return View(__existingsystemList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingsystemList = db.SP_SelectSystems();
                    return View(_existingsystemList);
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }

    }
}

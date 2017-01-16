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
    public class States_AdminController : Controller
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
                    var states = db.SP_SelectStates();
                    return View(states);

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
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                var statename = "";
                var stateid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        statename = Request.Form["statename"].ToString();
                        var state = (
                                       from p in db.States
                                       where (p.stateName == statename && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (state != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "State already exists";

                        }
                        if (state == null)
                        {
                            db.SP_AddStates(statename, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        stateid = Request.Form["id"].ToString();
                        statename = Request.Form["statename"].ToString();
                        //var state = (
                        //               from p in db.States
                        //               where (p.stateName == statename && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (state != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "State already exists";

                        //}
                        //if (state == null)
                        //{
                        db.sp_UpdateStates(Convert.ToInt64(stateid), statename, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        stateid = Request.Form["id"].ToString();
                        db.sp_DeleteStates(Convert.ToInt64(stateid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingstateList = db.SP_SelectStates();
                    return View(__existingstateList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingstateList = db.SP_SelectStates();
                    return View(_existingstateList);
                }
            }
            else
            {
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }

        //
        // POST: /Doctor/Edit/5


    }
}


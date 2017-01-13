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
    public class Medicine_AdminController : Controller
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
                    var medicine = db.SP_SelectMedicine();
                    return View(medicine);

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


                var medicinename = "";
                var medicineid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        medicinename = Request.Form["medicinename"].ToString();
                        var medicine = (
                                       from p in db.Medicines
                                       where (p.medicineName == medicinename && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (medicine != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Medicine already exists";

                        }
                        if (medicine == null)
                        {
                            db.SP_AddMedicine(medicinename, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        medicineid = Request.Form["id"].ToString();
                        medicinename = Request.Form["medicinename"].ToString();
                        //var medicine = (
                        //               from p in db.Medicine
                        //               where (p.medicineName == medicinename && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (medicine != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "Medicine already exists";
                            
                        //}
                        //if (medicine == null)
                        //{
                            db.sp_UpdateMedicine(Convert.ToInt64(medicineid), medicinename, Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        medicineid = Request.Form["id"].ToString();
                        db.sp_DeleteMedicine(Convert.ToInt64(medicineid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingallmedicineList = db.SP_SelectMedicine();
                    return View(__existingallmedicineList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingmedList = db.SP_SelectMedicine();
                    return View(_existingmedList);
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

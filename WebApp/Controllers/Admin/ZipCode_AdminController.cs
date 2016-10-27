using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;

namespace SwiftKare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ZipCode_AdminController : Controller
    {
        //
        // GET: /ZipCode/

        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var zipcode = db.SP_SelectZipCode();
                    return View(zipcode);

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
                var zipcode = "";
                var zipcodeid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        zipcode = Request.Form["zipname"].ToString();
                        var zip = (
                                       from p in db.Zips
                                       where (p.zipName == zipcode && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (zip != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "ZipCode already exists";

                        }
                        if (zip == null)
                        {
                            db.SP_AddZipCode(zipcode, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        zipcodeid = Request.Form["id"].ToString();
                        zipcode = Request.Form["zipname"].ToString();
                        //var zip = (
                        //               from p in db.ZipCode
                        //               where (p.zipName == zipcode && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (zip != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "ZipCode already exists";
                        //    //var _existingallergyList = db.SP_SelectAllergy();
                        //    //return View(_existingallergyList);
                        //}
                        //if (zip == null)
                        //{
                        db.sp_UpdateZipCode(Convert.ToInt64(zipcodeid), zipcode, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        zipcodeid = Request.Form["id"].ToString();
                        db.sp_DeleteZipCode(Convert.ToInt64(zipcodeid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingzipList = db.SP_SelectZipCode();
                    return View(__existingzipList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingzipList = db.SP_SelectZipCode();
                    return View(_existingzipList);
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

using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class Speciality_AdminController : Controller
    {

        
        SwiftKareDBEntities db = new SwiftKareDBEntities();
          
            public ActionResult Create()
            {
                if (Session["LogedUserID"] != null)
                {

                    try
                    {
                        var speciality = db.SP_SelectSpeciality();
                        return View(speciality);

                    }
                    catch (Exception ex)
                    {
                        ViewBag.errorMessage = "Error occurred while loading data.";
                        return View();
                    }
                }
                else
                {

                    return RedirectToAction("../Account/AdminLogin");
                }


            }

            [HttpPost]
            public ActionResult Create(FormCollection collection)
            {
                if (Session["LogedUserID"] != null)
                {
                    var specialityname = "";
                    var specialityid = "";
                    ViewBag.successMessage = "";
                    ViewBag.errorMessage = "";
                    try
                    {
                        var action = Request.Form["action"].ToString();
                        if (action == "create")
                        {
                            specialityname = Request.Form["specialityname"].ToString();
                            var speciality = (
                                           from p in db.Speciallities
                                           where (p.specialityName == specialityname && p.active == true)
                                           select p
                                       ).FirstOrDefault();
                            if (speciality != null)
                            {
                                ViewBag.successMessage = "";
                                ViewBag.errorMessage = "Speciality already exists";

                            }
                            if (speciality == null)
                            {
                                db.SP_AddSpeciality(specialityname, Session["LogedUserID"].ToString());
                                db.SaveChanges();
                                ViewBag.successMessage = "Record has been saved successfully";
                                ViewBag.errorMessage = "";
                            }
                        }
                        if (action == "edit")
                        {
                            specialityid = Request.Form["id"].ToString();
                            specialityname = Request.Form["specialityname"].ToString();
                            //var speciality = (
                            //               from p in db.Speciallity
                            //               where (p.specialityName == specialityname && p.active == true)
                            //               select p
                            //           ).FirstOrDefault();
                            //if (speciality != null)
                            //{
                            //    ViewBag.successMessage = "";
                            //    ViewBag.errorMessage = "Speciality already exists";

                            //}
                            //if (speciality == null)
                            //{
                            db.sp_UpdateSpeciality(Convert.ToInt64(specialityid), specialityname, Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                            //}
                        }
                        if (action == "delete")
                        {
                            specialityid = Request.Form["id"].ToString();
                            db.sp_DeleteSpeciality(Convert.ToInt64(specialityid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been deleted successfully";
                            ViewBag.errorMessage = "";
                        }
                        var __existingspecList = db.SP_SelectSpeciality();
                        return View(__existingspecList);

                    }
                    catch (Exception ex)
                    {
                        ViewBag.errorMessage = "Error occurred while processing your request.";
                        var _existingspecList = db.SP_SelectSpeciality();
                        return View(_existingspecList);
                    }
                }
                else
                {
                    return RedirectToAction("../Account/AdminLogin");
                }
            }


            //
            // POST: /Doctor/Edit/5



        }
    }

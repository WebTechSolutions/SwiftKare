using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class City_AdminController : Controller
    {
        //
        // GET: /Doctor/
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {


                try
                {
                    var city = db.SP_SelectCity();
                    return View(city);

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

                var cityname = "";
                var cityid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        cityname = Request.Form["cityname"].ToString();
                        var city = (
                                       from p in db.Cities
                                       where (p.cityName == cityname && p.active==true)
                                       select p
                                   ).FirstOrDefault();
                        if (city != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "City already exists";
                            
                        }
                        if (city == null)
                        {
                            db.SP_AddCity(cityname, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        cityid = Request.Form["id"].ToString();
                        cityname = Request.Form["cityname"].ToString();
                        //var city = (
                        //               from p in db.City
                        //               where (p.cityName == cityname && p.active==true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (city != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "City already exists";
                           
                        //}
                        //if (city == null)
                        //{
                            db.sp_UpdateCity(Convert.ToInt64(cityid), cityname, Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        cityid = Request.Form["id"].ToString();
                        db.sp_DeleteCity(Convert.ToInt64(cityid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingcityList = db.SP_SelectCity();
                    return View(__existingcityList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingcityList = db.SP_SelectCity();
                    return View(_existingcityList);
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

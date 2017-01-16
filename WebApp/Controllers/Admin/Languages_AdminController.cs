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
    public class Languages_AdminController : Controller
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
                    var languages = db.SP_SelectLanguages();
                    return View(languages);

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

                var languagename = "";
                var languageid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        languagename = Request.Form["languagename"].ToString();
                        var language = (
                                       from p in db.Languages
                                       where (p.languageName == languagename && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (language != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Language already exists";

                        }
                        if (language == null)
                        {
                            db.SP_AddLanguage(languagename, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        languageid = Request.Form["id"].ToString();
                        languagename = Request.Form["languagename"].ToString();
                        //var language = (
                        //               from p in db.Languages
                        //               where (p.languageName == languagename && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (language != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "Language already exists";
                            
                        //}
                        //if (language == null)
                        //{
                            db.sp_UpdateLanguages(Convert.ToInt64(languageid), languagename, Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        languageid = Request.Form["id"].ToString();
                        db.sp_DeleteLanguage(Convert.ToInt64(languageid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existinglangList = db.SP_SelectLanguages();
                    return View(__existinglangList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existinglangList = db.SP_SelectLanguages();
                    return View(_existinglangList);
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }


        }


    }
}

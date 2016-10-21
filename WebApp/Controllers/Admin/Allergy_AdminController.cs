using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class Allergy_AdminController : Controller
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
                    var allergy = db.SP_SelectAllergy();
                    return View(allergy);

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

                var allergyname = "";
                var allergyid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                     var action=Request.Form["action"].ToString();
                     if (action == "create")
                     {
                         allergyname = Request.Form["allergyname"].ToString();
                         var allergy = (
                                        from p in db.Allergies
                                        where (p.allergyName == allergyname && p.active == true)
                                        select p
                                    ).FirstOrDefault();
                         if (allergy != null)
                         {
                             ViewBag.successMessage = "";
                             ViewBag.errorMessage = "Allergy already exists";
                             
                         }
                         if (allergy == null)
                         {
                             db.SP_AddAllergy(allergyname, Session["LogedUserID"].ToString());
                             db.SaveChanges();
                             ViewBag.successMessage = "Record has been saved successfully";
                             ViewBag.errorMessage = "";
                         }
                     }
                     if (action == "edit")
                     {
                         allergyid = Request.Form["id"].ToString();
                         allergyname = Request.Form["allergyname"].ToString();
                         //var allergy = (
                         //               from p in db.Allergy
                         //               where (p.allergyName == allergyname && p.active == true)
                         //               select p
                         //           ).FirstOrDefault();
                         //if (allergy != null)
                         //{
                          //   ViewBag.successMessage = "";
                          //   ViewBag.errorMessage = "Allergy already exists";
                             //var _existingallergyList = db.SP_SelectAllergy();
                             //return View(_existingallergyList);
                         //}
                         //if (allergy == null)
                         //{
                             db.sp_UpdateAllergy(Convert.ToInt64(allergyid), allergyname, Session["LogedUserID"].ToString(), System.DateTime.Now);
                             db.SaveChanges();
                             ViewBag.successMessage = "Record has been saved successfully";
                             ViewBag.errorMessage = "";
                         //}
                     }
                     if (action == "delete")
                     {
                         allergyid = Request.Form["id"].ToString();
                         db.sp_DeleteAllergy(Convert.ToInt64(allergyid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                         db.SaveChanges();
                         ViewBag.successMessage = "Record has been deleted successfully";
                         ViewBag.errorMessage = "";
                     }
                    var __existingallergyList = db.SP_SelectAllergy();
                    return View(__existingallergyList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingallergyList = db.SP_SelectAllergy();
                    return View(_existingallergyList);
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

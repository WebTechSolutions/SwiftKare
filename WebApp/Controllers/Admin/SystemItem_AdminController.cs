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
    public class SystemItem_AdminController : Controller
    {
        //
        // GET: /SystemItem/

        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();

        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var systemitem = db.SP_SelectSystemItems();
                    var systems = db.PatientSystems
                    .Where(a => a.active == true).ToList();
                    ViewBag.Systems = systems;
                    ViewBag.systemid = 0;
                    return View(systemitem);

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
                var systemitem = "";
                var systemitemid = "";
                var systemid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        systemitem = Request.Form["systemitem"].ToString();
                        systemid = Request.Form["sltSystem"].ToString();

                        var item = (
                                       from p in db.SystemItemsses
                                       where (p.systemItemName == systemitem && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (item != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "System Item already exists";

                        }
                        if (item == null)
                        {
                            db.SP_AddSystemItem(systemitem, Convert.ToInt64(systemid), Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        systemitemid = Request.Form["id"].ToString();
                        systemitem = Request.Form["systemitem"].ToString();
                        systemid = Request.Form["sltSystem"].ToString();
                        //var item = (
                        //               from p in db.SystemItemss
                        //               where (p.systemItemName == systemitem && p.active == true)
                        //               select p
                        //           ).FirstOrDefault();
                        //if (item != null)
                        //{
                        //    ViewBag.successMessage = "";
                        //    ViewBag.errorMessage = "System Item already exists";

                        //}
                        //if (item == null)
                        //{
                        db.sp_UpdateSystemItem(Convert.ToInt64(systemitemid), Convert.ToInt64(systemid), systemitem, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        // }
                    }
                    if (action == "delete")
                    {
                        systemitemid = Request.Form["id"].ToString();
                        db.sp_DeleteSystemItem(Convert.ToInt64(systemitemid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingitemList = db.SP_SelectSystemItems();
                    var systems = db.PatientSystems
                    .Where(a => a.active == true).ToList();
                    ViewBag.Systems = systems;
                    ViewBag.systemid = systemid;
                    return View(__existingitemList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingitemList = db.SP_SelectSystemItems();
                    var systems = db.PatientSystems.ToList();
                    ViewBag.Systems = systems;
                    return View(_existingitemList);
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }

        public PartialViewResult LoadSystemItems(long id)
        {
            try
            {
               
                var oData = (from sys in db.SystemItemsses
                             where sys.systemID == id
                             select new DataAccess.CustomModels.SystemItemsModel
                             {
                                 systemItemID = sys.systemItemID,
                                 systemItemName = sys.systemItemName,
                                 systemID = sys.systemID,
                                 systemName = db.PatientSystems.Where(s => s.systemID == id).Select(s => s.systemName).FirstOrDefault()
                             }).ToList();
                 
                return PartialView("SystemItemGrid", oData);

            }

            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                ViewBag.Success = "";
            }
            return PartialView("SystemItemGrid");
        }
    }
}

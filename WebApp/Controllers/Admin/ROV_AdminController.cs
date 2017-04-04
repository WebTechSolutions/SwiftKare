using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

namespace WebApp.Controllers.Admin
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class ROV_AdminController : Controller
    {
        SwiftKareDBEntities db = new SwiftKareDBEntities();

        public ActionResult Create()
        {
            try
            {
                var rov = db.ROVs.Where(r => r.active == true).ToList();
                return View(rov);

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Error occurred while loading data.";
                return View();
            }
           
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
                var rovname = "";
                long rovid;
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        rovname = Request.Form["rovname"].ToString();
                        var obj_rov = (
                                       from p in db.ROVs
                                       where (p.name == rovname && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (obj_rov != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Reason of Visit already exists";

                        }
                        if (obj_rov == null)
                        {
                            ROV dbrov=new ROV();
                            dbrov.name = rovname;
                            dbrov.cd = DateTime.Now;
                            dbrov.cb = SessionHandler.UserId;
                            dbrov.active = true;
                            db.ROVs.Add(dbrov);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        rovid = Convert.ToInt32(Request.Form["id"].ToString());
                        rovname = Request.Form["rovname"].ToString();
                        ROV dbrov = new ROV();
                        dbrov = db.ROVs.Where(r => r.rovID == rovid).FirstOrDefault();
                        dbrov.name = rovname;
                        dbrov.md = DateTime.Now;
                        dbrov.mb = SessionHandler.UserId;
                        db.Entry(dbrov).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        
                    }
                    if (action == "delete")
                    {
                        rovid = Convert.ToInt32(Request.Form["id"].ToString());
                        ROV dbrov = new ROV();
                        dbrov = db.ROVs.Where(r => r.rovID == rovid).FirstOrDefault();
                        dbrov.active = false;
                        db.Entry(dbrov).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                var rov = db.ROVs.Where(r => r.active == true).ToList();
                return View(rov);

            }
            catch (Exception ex)
            {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _rov = db.ROVs.Where(r => r.active == true).ToList();
                    return View(_rov);
            }
            


        }
    }
}

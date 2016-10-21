using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class DoctorSpecialityController : Controller
    {
        //
        // GET: /DoctorSpeciality/

        SwiftKareDBEntities db = new SwiftKareDBEntities();

        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var __existingdocspeList = db.SP_SelectDoctorSpeciality();
                    var doctors = db.Doctors
                   .Where(a => a.active == true).ToList();
                    ViewBag.Doctors = doctors;
                    var speciality = db.Speciallities
                   .Where(a => a.active == true).ToList();
                    ViewBag.Speciality = speciality;
                    return View(__existingdocspeList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while loading data.";
                    return View();
                }
            }
            else
            {

                return RedirectToAction("../Login/Index");
            }


        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                var specialityid = "";
                var doctorid = "";
                var docspecialityid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        specialityid = Request.Form["sltspeciality"].ToString();
                        doctorid = Request.Form["sltdoctor"].ToString();
                        db.SP_AddDoctorSpeciality(Convert.ToInt64(doctorid), Convert.ToInt64(specialityid), Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                     
                    }
                    if (action == "edit")
                    {
                        docspecialityid = Request.Form["id"].ToString();
                        specialityid = Request.Form["sltspeciality"].ToString();
                        doctorid = Request.Form["sltdoctor"].ToString();
                        db.sp_UpdateDoctorSpeciality(Convert.ToInt64(docspecialityid), Convert.ToInt64(doctorid), Convert.ToInt64(specialityid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        docspecialityid = Request.Form["id"].ToString();
                        db.sp_DeleteDoctorSpeciality(Convert.ToInt64(docspecialityid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingdocspeList = db.SP_SelectDoctorSpeciality();
                    var doctors = db.Doctors
                    .Where(a => a.active == true).ToList();
                    ViewBag.Doctors = doctors;
                    var speciality = db.Speciallities
                    .Where(a => a.active == true).ToList();
                    ViewBag.Speciality = speciality;
                    return View(__existingdocspeList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var __existingdocspeList = db.SP_SelectDoctorSpeciality();
                    var doctors = db.Doctors
                  .Where(a => a.active == true).ToList();
                    ViewBag.Doctors = doctors;
                    var speciality = db.Speciallities
                   .Where(a => a.active == true).ToList();
                    ViewBag.Speciality = speciality;
                    return View(__existingdocspeList);
                }
            }
            else
            {
                return RedirectToAction("../Login/Index");
            }
        }

    }
}

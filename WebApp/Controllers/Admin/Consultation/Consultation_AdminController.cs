using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers.Consultation
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class Consultation_AdminController : Controller
    {
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        //
        // GET: /Consultation/

        public ActionResult ReviewsApproval()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doc = db.SP_SelectConsultation();
                    return View(doc);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }
        [HttpPost]
        public ActionResult ApproveReview(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                try
                {
                    var id = Request.Form["id"].ToString();
                    db.sp_ApproveConsultation(Convert.ToInt64(id), Session["LogedUserID"].ToString(), System.DateTime.Now);
                    db.SaveChanges();
                    return RedirectToAction("ReviewsApproval");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ReviewsApproval");
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }

        }

        public ActionResult ConsultationReport()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doctors = (from d in db.Doctors
                                   where d.firstName!=null && d.lastName!=null
                                   select new DocConsultationReport
                                   {
                                       doctorid = d.doctorID,
                                       firstName = d.firstName ,
                                       lastName=d.lastName
                                   }).ToList();
                    var patients = (from d in db.Patients
                                    where d.firstName != null && d.lastName != null
                                    select new PatConsultationReport
                                   {
                                       patientid = d.patientID,
                                       firstName = d.firstName,
                                       lastName = d.lastName
                                   }).ToList();
                    ViewBag.Doctors = doctors;
                    ViewBag.Patients = patients;
                    return View();
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }

        [HttpPost]
       
        public ActionResult ConsultationReport(FormCollection form)
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doctors = (from d in db.Doctors
                                   where d.firstName != null && d.lastName != null
                                   select new DocConsultationReport
                                   {
                                       doctorid = d.doctorID,
                                       firstName = d.firstName,
                                       lastName = d.lastName
                                   }).ToList();
                    var patients = (from d in db.Patients
                                    where d.firstName != null && d.lastName != null
                                    select new PatConsultationReport
                                    {
                                        patientid = d.patientID,
                                        firstName = d.firstName,
                                        lastName = d.lastName
                                    }).ToList();
                    ViewBag.Doctors = doctors;
                    ViewBag.Patients = patients;
                    var datefrom = Request.Form["datefrom"].ToString().Trim();
                    var dateto = Request.Form["dateto"].ToString().Trim();
                    var doctorid = Request.Form["sltDoctor"].ToString();
                    var patientid = Request.Form["sltPatient"].ToString();
                    
                   
                    if (doctorid == "0" && patientid != "0")
                    {
                        var doc = db.SP_ConsultationReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), Convert.ToInt32(patientid), null);
                        return View(doc);
                    }
                    if (doctorid != "0" && patientid == "0")
                    {
                        var doc = db.SP_ConsultationReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), null,Convert.ToInt32(doctorid));
                        return View(doc);
                    }
                    var docc = db.SP_ConsultationReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), null, null);
                    return View(docc);
                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }

        public ActionResult AppReport()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                   return View();
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }
        [HttpPost]
        
        public ActionResult AppReport(FormCollection form)
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    
                    var datefrom=Request.Form["datefrom"].ToString();
                    var dateto = Request.Form["dateto"].ToString();
                    var criteria = Request.Form["sltCriteria"].ToString();
                    if (criteria == "")
                    {
                        ViewBag.successMessage = "";
                        ViewBag.errorMessage = "Select valid Role";
                        return View();
                    }
                    var app = db.SP_AppointmentReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto),criteria);
                    return View(app);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }
    }
}

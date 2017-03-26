using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers.TransactionHistory
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class Transaction_AdminController : Controller
    {
        //
        // GET: /Transaction/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult TransactionHistory()
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
                    //var doc = db.SP_selectTransactionHistory();
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
        public ActionResult TransactionHistoryReport(FormCollection form)
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
                        var doc = db.SP_selectTransactionHistory(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), Convert.ToInt32(patientid), null);
                        return View("TransactionHistory", doc);
                    }
                    if (doctorid != "0" && patientid == "0")
                    {
                        var doc = db.SP_selectTransactionHistory(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), null, Convert.ToInt32(doctorid));
                        return View("TransactionHistory", doc);
                    }
                    var docc = db.SP_selectTransactionHistory(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto), null, null);
                    return View("TransactionHistory", docc);
                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = ex.Message;
                    return View("TransactionHistory");
                }
            }
            else
            {
                return RedirectToAction("AdminLogin", "Account");
            }
        }


    }
}

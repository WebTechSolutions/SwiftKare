using DataAccess;
using System;
using System.Collections.Generic;
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
                return RedirectToAction("../AdminLogin/AdminLogin");
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
                return RedirectToAction("../AdminLogin/AdminLogin");
            }

        }

        public ActionResult ConsultationReport()
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
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }

        [HttpPost]
        [CustomAuthorizationAttribute]
        public ActionResult ConsultationReport(FormCollection form)
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var datefrom = Request.Form["datefrom"].ToString();
                    var dateto = Request.Form["dateto"].ToString();
                    var doc = db.SP_ConsultationReport(Convert.ToDateTime(datefrom), Convert.ToDateTime(dateto));
                    return View(doc);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("../AdminLogin/AdminLogin");
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
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }
        [HttpPost]
        [CustomAuthorizationAttribute]
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
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }
    }
}

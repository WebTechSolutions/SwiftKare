using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

namespace WebApp.Controllers
{
    public class ConsultationController : Controller
    {
        // GET: Consultation
        public ActionResult Index()
        {
            if (SessionHandler.IsExpired)
            {
                return RedirectToAction("DoctorLogin", "Account");
            }
            else
            {
                ViewBag.errorMessage = "";
                ViewBag.successMessage = "";
                return View();
            }
        }
        public ActionResult ViewDetail(long consID)
        {
            try
            {
                //var oData = oAppointmentRepository.GetAppDetail(appID);
                return PartialView("PartialViewDetail");
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
            }
            return PartialView("PartialViewDetail");

        }
    }
}
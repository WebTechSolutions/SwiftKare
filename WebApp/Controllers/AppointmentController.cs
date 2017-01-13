using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

using WebApp.Repositories.PatientRepositories;

//
namespace WebApp.Controllers
{
    [PatientSessionExpire]
    [Authorize(Roles = "Patient")]
    public class AppointmentController : Controller
    {
        AppointmentRepository oAppointmentRepository;
        ConsultationRepository oConsultationRepository;

        public AppointmentController()
        {
            oAppointmentRepository = new AppointmentRepository();
            oConsultationRepository = new ConsultationRepository();
        }
        // GET: Appointment
        public ActionResult Index()
        {
           
                return View();
           
        }

        public PartialViewResult PartialReschedule()
        {
            try
            {
                var oData = oAppointmentRepository.GetRescheduleApp(SessionHandler.UserInfo.Id);

                return PartialView("PartialReschedule", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialReschedule");
        }
        public PartialViewResult PartialPending()
        {
            try
            {
                var oData = oAppointmentRepository.GetPendingApp(SessionHandler.UserInfo.Id);

                return PartialView("PartialPending", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialPending");
        }
        public PartialViewResult PartialUpcoming()
        {
            try
            {
                var oData = oAppointmentRepository.GetUpcomingApp(SessionHandler.UserInfo.Id);

                return PartialView("PartialUpcoming", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialUpcoming");
        }

      
        public PartialViewResult ViewAppDetails(long? appID)
        {
            try
            {
                long apID = Convert.ToInt64(appID);
                var oData = oAppointmentRepository.GetAppDetail(apID);
                return PartialView("PartialViewDetail", oData);
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
            }
            return PartialView("PartialViewDetail");
        }
       
        public JsonResult RescheduleApp(RescheduleAppointmentModel _objRAPP)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oAppointmentRepository.RescheduleApp(_objRAPP);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        public JsonResult CompleteApp(CompleteConsultPatient _objCAPP)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oConsultationRepository.CompleteConsult(_objCAPP);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase.ToString() });
            }

        }

    }
}
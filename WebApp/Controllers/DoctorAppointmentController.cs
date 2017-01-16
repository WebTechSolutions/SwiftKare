using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;

namespace WebApp.Controllers
{
    [DoctorSessionExpire]
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentController : Controller
    {
        // GET: DoctorAppointment
        DoctorAppointmentRepositroy oAppointmentRepository;
        DoctorConsultationRepository oDoctorConsultationRepositroy;
        SeeDoctorRepository oSeeDoctorRepository;
        public DoctorAppointmentController()
        {
            oAppointmentRepository = new DoctorAppointmentRepositroy();
            oSeeDoctorRepository = new SeeDoctorRepository();
            oDoctorConsultationRepositroy = new DoctorConsultationRepository();
        }
        // GET: Appointment
        public ActionResult Index()
        {
            
                return View();
           
        }

        public PartialViewResult PartialDoctorReschedule()
        {
            try
            {
                var oData = oAppointmentRepository.GetRescheduleApp(SessionHandler.UserInfo.Id);

                return PartialView("PartialDoctorReschedule", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
                return PartialView("PartialDoctorReschedule");
            }
           
        }

        public PartialViewResult PartialDoctorUpcoming()
        {
            try
            {
                var oData = oAppointmentRepository.GetUpcomingApp(SessionHandler.UserInfo.Id);
                ViewBag.errorMessage = "";
                ViewBag.successMessage = "";
                return PartialView("PartialDoctorUpcoming", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
                
                return PartialView("PartialDoctorUpcoming");
            }
           
        }

        public PartialViewResult PartialDoctorPending()
        {
            try
            {
                var oData = oAppointmentRepository.GetPendingApp(SessionHandler.UserInfo.Id);
                ViewBag.errorMessage = "";
                ViewBag.successMessage = "";
                return PartialView("PartialDoctorPending", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";

                return PartialView("PartialDoctorPending");
            }

        }

        public PartialViewResult ViewAppDetails(long? appID)
        {
            try
            {
                long apID = Convert.ToInt64(appID);
                var oData = oAppointmentRepository.GetAppDetail(apID);
                return PartialView("PartialDoctorViewAppDetail", oData);
            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.errorMessage = "";
                return PartialView("PartialDoctorViewAppDetail");
            }
           
        }
        [HttpPost]
        public JsonResult CancelReschedule(CancelRescheduleRequestModel model)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oAppointmentRepository.CancelReschedule(model);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        [HttpPost]
        public JsonResult Reschedule(RescheduleRequestModel model)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oAppointmentRepository.Reschedule(model);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        public JsonResult CompleteApp(CompleteConsultDoctor _objCAPP)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oDoctorConsultationRepositroy.CompleteConsult(_objCAPP);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }


    }
}
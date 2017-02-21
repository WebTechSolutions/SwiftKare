using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.ProfileRepositories;

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
                if(oData!=null)
                {
                    if (oData.PatientVM.ProfilePhotoBase64 == null)
                    {
                        oData.PatientVM.ProfilePhotoBase64 = "data:image/png;base64,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";
                    }
                   
                }
               
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

        public PartialViewResult PatientProfile(long patientID)
        {
            try
            {
                ProfileRepository oProfileRepository = new ProfileRepository();
                var oModel = oProfileRepository.GetPatientProfileViewOnly(patientID);
                if (oModel != null)
                {
                    oModel.ConvertByteArrayToBase64();
                    if (oModel.ProfilePhotoBase64 == null)
                    {
                        oModel.ProfilePhotoBase64 = "data:image/png;base64,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";
                    }

                }

                return PartialView("PatientProfileView", oModel);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PatientProfileView");
        }

        public JsonResult UpdateTimezone(string localtimezone)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                TimezoneModel model = new TimezoneModel();
                model.timezone = localtimezone;
                model.userid = SessionHandler.UserId;
                ProfileRepository oProfileRepository = new ProfileRepository();
                apiresult = oProfileRepository.UpdateTimezone(model);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase.ToString() });
            }

        }
    }
}
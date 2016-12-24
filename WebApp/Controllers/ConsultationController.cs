using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.PatientRepositories;

namespace WebApp.Controllers
{
    public class ConsultationController : Controller
    {
        ConsultationRepository oConsultationRepository;
        long appID = 0;
        public ConsultationController()
        {
            oConsultationRepository = new ConsultationRepository();

        }
        // GET: Consultation
        public ActionResult Index()
        {
            if (SessionHandler.IsExpired)
            {
                return RedirectToAction("PatientLogin", "Account");
            }
            else
            {
                ViewBag.errorMessage = "";
                ViewBag.successMessage = "";
                return View();
            }
        }
        public PartialViewResult PartialViewConsultation()
        {
            try
            {
                var oData = oConsultationRepository.GetPatientConsultations(SessionHandler.UserInfo.Id);

                return PartialView("PartialViewConsultation", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialViewConsultation");
        }
        public ActionResult ViewDetail(long? consultID)
        {
            try
            {
                long cid = Convert.ToInt64(consultID);
                var oData = oConsultationRepository.GetConsultationDetail(cid);
                return PartialView("PartialViewDetail",oData);
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
            }
            return PartialView("PartialViewDetail");

        }
        public JsonResult WriteReview(AddConsultReviewodel _objReview)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oConsultationRepository.WriteReview(_objReview);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }


       
    }
}
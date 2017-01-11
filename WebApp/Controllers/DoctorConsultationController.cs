using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;


namespace WebApp.Controllers
{
    public class DoctorConsultationController : Controller
    {

        DoctorConsultationRepository oConsultationRepository;
        long consultID = 0;

        public DoctorConsultationController()
        {
            oConsultationRepository = new DoctorConsultationRepository();

        }
        // GET: DoctorConsultation
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

        public PartialViewResult PartialViewDoctorConsultation()
        {
            try
            {
                var oData = oConsultationRepository.GetDoctorConsultations(SessionHandler.UserInfo.Id);

                return PartialView("PartialViewDoctorConsultation", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialViewDoctorConsultation");
        }

        public ActionResult ViewDetail(long? consultID)
        {
            try
            {
                long cid = Convert.ToInt64(consultID);
                var oData = oConsultationRepository.GetConsultationDetail(cid);
                return PartialView("PartialViewDoctorConsultationDetail", oData);
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.errorMessage = ex.Response.ReasonPhrase.ToString();
                ViewBag.successMessage = "";
            }
            return PartialView("PartialViewDoctorConsultationDetail");

        }


        public string GetDoseSpotUrl(long patientId) {
            var cRetUrl = new DoseSpotRepository().GetPatientDoseSpotUrl(patientId);
            return cRetUrl;
        }


    }

}
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
           
                return View();
          
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
    }

}
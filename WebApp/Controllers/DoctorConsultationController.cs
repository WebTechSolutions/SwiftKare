using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.PatientRepositories;

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
                if(consultID!=0)
                {
                    long cid = Convert.ToInt64(consultID);
                    var oData = oConsultationRepository.GetConsultationDetail(cid);

                    var oChat = oConsultationRepository.GetChat(cid);
                    ViewBag.ChatText = oChat;
                    return PartialView("PartialViewDoctorConsultationDetail", oData);
                }
                else
                {
                    return PartialView("PartialViewDoctorConsultationDetail");
                }
                
                
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
        public FileResult Download(long fileId,long patID)
        {
            PatientFilesRepository oPatientFilesRepository = new PatientFilesRepository();
            var oFileToDownload = oPatientFilesRepository.GetPatientFile(patID, fileId);

            string fileName = oFileToDownload.FileName;
            string contentType = string.Empty;

            switch (Path.GetExtension(oFileToDownload.FileName).Trim('.').ToLower())
            {
                case "txt":
                    contentType = "text/plain";
                    break;

                case "pdf":
                    contentType = "application/pdf";
                    break;

                case "docx":
                    contentType = "application/docx";
                    break;

                case "csv":
                    contentType = "text/csv";
                    break;

                case "jpg":
                    contentType = "image/jpeg";
                    break;

                case "png":
                    contentType = "image/png";
                    break;

                default:
                    contentType = "application/binary";
                    break;
            }

            return File(oFileToDownload.fileContent, contentType, fileName);
        }

    }

}
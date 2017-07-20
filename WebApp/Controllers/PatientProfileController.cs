using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.ProfileRepositories;

namespace WebApp.Controllers
{
    [PatientSessionExpire]
    [Authorize(Roles = "Patient")]
    public class PatientProfileController : Controller
    {
        #region Declarations

        ProfileRepository oProfileRepository;
        public PatientProfileController()
        {
            oProfileRepository = new ProfileRepository();
        }

        #endregion

        #region Action Methods

        public ActionResult Index()
        {
            setInitialViewData();
            var oModel = oProfileRepository.GetPatientProfileWithAllValues(SessionHandler.UserInfo.Id);
         //   oModel.ConvertByteArrayToBase64();

            return View(oModel);
        }

        public ActionResult Profile()
        {
            var oModel = oProfileRepository.GetPatientProfileViewOnly(SessionHandler.UserInfo.Id);
          //  oModel.ConvertByteArrayToBase64();

            return View(oModel);
        }

        [HttpPost]
        public string SavePatientProfile(PatientProfileVM oModel)
        {
            try
            {
                oModel.ConvertBase64ToByteArray();
                var oRetMsg = oProfileRepository.UpdatePatientProfileWithAllValues(oModel);

                SessionHandler.ProfilePhoto = oModel.ProfilePhotoBase64;
                SessionHandler.UserInfo.title = oModel.TitleName;
               var timezone = oProfileRepository.GetPatientTimeZone(SessionHandler.UserId);
                SessionHandler.UserInfo.timeZone = timezone.zonename;
                SessionHandler.UserInfo.timeZoneOffset = timezone.zoneoffset;
                return oRetMsg.message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string SaveSecretAnswers(PatientProfileVM oModel)
        {
            try
            {
                var oRetMsg = oProfileRepository.updatePatientSecretAnswers(SessionHandler.UserInfo.Id, new UpdateSecretQuestions
                {
                    secretanswer1 = oModel.SectetAnswer1,
                    secretanswer2 = oModel.SectetAnswer2,
                    secretanswer3 = oModel.SectetAnswer3,
                    secretquestion1 = oModel.SectetQuestion1,
                    secretquestion2 = oModel.SectetQuestion2,
                    secretquestion3 = oModel.SectetQuestion3,
                    doctorID = SessionHandler.UserInfo.Id
                });
                return oRetMsg.message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string ChangePassword(string newPassword)
        {
            try
            {
                var oRetMsg = oProfileRepository.ChangePatientPassword(new DoctorPasswordModel { doctorID = SessionHandler.UserInfo.Id, password = newPassword });
                return oRetMsg.message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        private void setInitialViewData()
        {
            var oInitialData = oProfileRepository.GetPatientProfileInitialValues();

            ViewBag.drpdnTitle = oInitialData.lstTitleVM.Select(x => new SelectListItem { Text = x.titleName, Value = x.titleName }).ToList();
            ViewBag.drpdnSuffix = oInitialData.lstSuffixVM.Select(x => new SelectListItem { Text = x.suffixName, Value = x.suffixName }).ToList();

            ViewBag.drpdnLanguage = oInitialData.lstLanguageVM.Select(x => new SelectListItem { Text = x.languageName, Value = x.languageName }).ToList();
            ViewBag.drpdnTimeZone = oInitialData.lstTimeZoneVM.Select(x => new SelectListItem { Text = x.timeZone, Value = x.zoneName }).ToList();
            ViewBag.drpdnCity = oInitialData.lstCityVM.Select(x => new SelectListItem { Text = x.cityName, Value = x.cityName }).ToList();
            ViewBag.drpdnState = oInitialData.lstStateVM.Select(x => new SelectListItem { Text = x.stateName, Value = x.stateName }).ToList();
            ViewBag.SecretQuestion = oInitialData.lstSecretQuestionVM.Select(x => new SelectListItem { Text = x.secretQuestion, Value = x.secretQuestion }).ToList();
        }

        #endregion


    }
}
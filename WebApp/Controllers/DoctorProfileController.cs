using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;
using WebApp.Helper;
using System;
using System.Globalization;
using WebApp.Repositories.ProfileRepositories;
using System.Text;

namespace WebApp.Controllers
{
    [DoctorSessionExpire]
    [Authorize(Roles = "Doctor")]
    public class DoctorProfileController : Controller
    {
        #region Declarations

        ProfileRepository oProfileRepository;
        public DoctorProfileController()
        {
            oProfileRepository = new ProfileRepository();
        }

        #endregion

        #region Action Methods

        public ActionResult Index()
        {
            setInitialViewData();
            var oModel = oProfileRepository.GetDoctorProfileWithAllValues(SessionHandler.UserInfo.Id);
            //oModel.ConvertByteArrayToBase64();

            return View(oModel);
        }

        public ActionResult Profile()
        {
            var oModel = oProfileRepository.GetDoctorProfileWithAllValues(SessionHandler.UserInfo.Id);
            //oModel.ConvertByteArrayToBase64();

            return View(oModel);
        }

        [HttpPost]
        public string SaveDoctorProfile(DoctorProfileVM oModel)
        {
            try
            {
                oModel.ConvertBase64ToByteArray();
                var oRetMsg = oProfileRepository.UpdateDoctorProfileWithAllValues(oModel);

                SessionHandler.ProfilePhoto = oModel.ProfilePhotoBase64;
                var timezone = oProfileRepository.GetDoctorTimeZone(SessionHandler.UserId);
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
        public string SaveConsultCharges(long charges)
        {
            try
            {
                var oRetMsg = oProfileRepository.UpdateConsultCharges(new UpdateConsultCharges { consultCharges = charges, doctorID = SessionHandler.UserInfo.Id });
                return oRetMsg.message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string SaveSecretAnswers(DoctorProfileVM oModel)
        {
            try
            {
                var oRetMsg = oProfileRepository.AddUpdateDoctorSecretAnswers(new UpdateSecretQuestions
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
                var oRetMsg = oProfileRepository.ChangePassword(new DoctorPasswordModel { doctorID = SessionHandler.UserInfo.Id, password = newPassword });
                return oRetMsg.message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        private void setInitialViewData()
        {
            var oInitialData = oProfileRepository.GetDoctorProfileInitialValues();

            ViewBag.drpdnTitle = oInitialData.lstTitleVM.Select(x => new SelectListItem { Text = x.titleName, Value = x.titleName }).ToList();
            ViewBag.drpdnSuffix = oInitialData.lstSuffixVM.Select(x => new SelectListItem { Text = x.suffixName, Value = x.suffixName }).ToList();

            ViewBag.drpdnSpeciality = oInitialData.lstSpecialityVM.Select(x => new SelectListItem { Text = x.specialityName, Value = x.specialityName }).ToList();
            ViewBag.drpdnLanguage = oInitialData.lstLanguageVM.Select(x => new SelectListItem { Text = x.languageName, Value = x.languageName }).ToList();
            ViewBag.drpdnTimeZone = oInitialData.lstTimeZoneVM.Select(x => new SelectListItem { Text = x.timeZone, Value = x.zoneName }).ToList();
            ViewBag.drpdnCity = oInitialData.lstCityVM.Select(x => new SelectListItem { Text = x.cityName, Value = x.cityName }).ToList();
            ViewBag.drpdnState = oInitialData.lstStateVM.Select(x => new SelectListItem { Text = x.stateName, Value = x.stateName }).ToList();
            ViewBag.SecretQuestion = oInitialData.lstSecretQuestionVM.Select(x => new SelectListItem { Text = x.secretQuestion, Value = x.secretQuestion }).ToList();
        }

        #endregion


    }

}
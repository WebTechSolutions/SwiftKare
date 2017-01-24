using DataAccess.CustomModels;
using Identity.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.VideoCallRepository;

namespace WebApp.Controllers
{
    [Authorize]
    public class UserChatController : Controller
    {
        #region Declaration and Constructors

        VideoCallRepository oVideoCallRepository;

        public UserChatController()
        {
            oVideoCallRepository = new VideoCallRepository();
        }

        #endregion

        [HttpPost]
        public string ReadyForCall(string senderId, string receiverId, string userType, string doctorName, string patientName, string aptId = "")
        {
            try
            {
                var openTokSession = UserChatHelper.GetOpenTokSessionInformation(senderId, receiverId, userType, doctorName, patientName, aptId);

                if (openTokSession == null || string.IsNullOrEmpty(openTokSession.SessionId) || string.IsNullOrEmpty(openTokSession.TokenId))
                {
                    return "";
                }

                openTokSession.UserType = userType;

                HttpContext.Session["MyOpenTokSession"] = openTokSession;
                return openTokSession.SessionId.ToString()+"*"+ openTokSession.TokenId.ToString();
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        // GET: UserChat
        public ActionResult Index()
        {
            var openTokSession = (OpenTokSession)(HttpContext.Session["MyOpenTokSession"]);

            if (openTokSession == null || string.IsNullOrEmpty(openTokSession.SessionId) || string.IsNullOrEmpty(openTokSession.TokenId))
            {
                //Redirect user to appropriate page
            }

            ViewBag.UserType = openTokSession.UserType;

            var userMgr = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userMgr.GetRoles(SessionHandler.UserId);
            if (roles.Contains("Doctor"))
            {
                ViewBag.RecipientName = openTokSession.PatientName;
            }
            else {
                ViewBag.RecipientName = openTokSession.DoctorName;
            }
            ViewBag.OpenTokApiKey = UserChatHelper.TokBoxApiKey;
            ViewBag.OpenTokSession = openTokSession.SessionId;
            ViewBag.OpenTokToken = openTokSession.TokenId;

            ViewBag.RosItems = oVideoCallRepository.GetROSItems();

            return View();
        }


        #region Handles call-chat related api calls

        [HttpPost]
        public void PatientCallDoctor(long doctorId)
        {
            oVideoCallRepository.PatientCallDoctor(SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email, doctorId);
        }

        [HttpPost]
        public void DoctorCallPatient(long patientId)
        {
            oVideoCallRepository.DoctorCallPatient(SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email, patientId);
        }


        [HttpPost]
        public void DoctorAcceptedCall(long patientId)
        {
            oVideoCallRepository.DoctorAcceptsCall(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void PatientAcceptedCall(long doctorId)
        {
            oVideoCallRepository.PatientAcceptsCall(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }


        [HttpPost]
        public void DoctorDeclinedCall(long patientId)
        {
            oVideoCallRepository.DoctorRejectsCall(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void PatientDeclinedCall(long doctorId)
        {
            oVideoCallRepository.PatientRejectsCall(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }


        [HttpPost]
        public string CreateConsult(CreateConsultModel oModel)
        {
            var oRet = oVideoCallRepository.CreateConsult(oModel);
            return oRet.ID.ToString();
        }

        [HttpPost]
        public void StartConsultation(long consultId)
        {
            oVideoCallRepository.AddConsultStartTime(new AddConsultTimeModel { consultID = consultId, userEmail = SessionHandler.UserInfo.Email });
        }

        [HttpPost]
        public void StopConsultation(long consultId)
        {
            oVideoCallRepository.AddConsultEndTime(new AddConsultTimeModel { consultID = consultId, userEmail = SessionHandler.UserInfo.Email });
            oVideoCallRepository.AddVCLog(new VCLogModel { consultID = consultId, endBy = SessionHandler.UserInfo.Email, endReason = "Natural Call Stop", logBy = SessionHandler.UserInfo.Email });
        }

        [HttpPost]
        public void AddVCLog(long consultId, string endReason)
        {
            oVideoCallRepository.AddVCLog(new VCLogModel { consultID = consultId, endBy = SessionHandler.UserInfo.Email, endReason = endReason, logBy = SessionHandler.UserInfo.Email });
        }

        [HttpPost]
        public void AddChatMessages(long consultId, string message, string sender, string receiver)
        {
            oVideoCallRepository.AddChatMessages(new ChatMessageModel { consultID = consultId, message = message, sender = "", reciever = SessionHandler.UserInfo.Email });
        }

        [HttpPost]
        public void AddDoctorNotesSubjective(long consultId, string cValues)
        {
            oVideoCallRepository.AddDoctorNotesSubjective(consultId, cValues);
        }

        [HttpPost]
        public void AddDoctorNotesObjective(long consultId, string cValues)
        {
            oVideoCallRepository.AddDoctorNotesObjective(consultId, cValues);
        }

        [HttpPost]
        public void AddDoctorNotesAssessment(long consultId, string cValues)
        {
            oVideoCallRepository.AddDoctorNotesAssessment(consultId, cValues);
        }

        [HttpPost]
        public void AddDoctorNotesPlans(long consultId, string cValues)
        {
            oVideoCallRepository.AddDoctorNotesPlans(consultId, cValues);
        }


        [HttpPost]
        public void AddConsultROS(long consultID, long sysitemid, string sysitemname)
        {
            oVideoCallRepository.AddConsultROS(new ConsultROSModel { consultID = consultID, sysitemid = sysitemid, sysitemname = sysitemname, userID = SessionHandler.UserInfo.Id.ToString() });
        }


        [HttpPost]
        public void RemoveConsultROS(long consultID, long sysitemid, string sysitemname)
        {
            oVideoCallRepository.RemoveConsultROS(new ConsultROSModel { consultID = consultID, sysitemid = sysitemid, sysitemname = sysitemname, userID = SessionHandler.UserInfo.Id.ToString() });
        }

        #endregion

    }


}
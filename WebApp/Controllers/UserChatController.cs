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
                return openTokSession.SessionId.ToString() + "*" + openTokSession.TokenId.ToString();
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        // GET: UserChat
        public ActionResult Index(string tokboxInfo)
        {
            //tokboxInfo=SessionId*TokenId*TokBoxApiKey*UserType*PatientName*DoctorName
            

            //  var openTokSession = (OpenTokSession)(HttpContext.Session["MyOpenTokSession"]);

            //  if (openTokSession == null || string.IsNullOrEmpty(openTokSession.SessionId) || string.IsNullOrEmpty(openTokSession.TokenId))
            //  {
            //Redirect user to appropriate page
            //  }
            if (tokboxInfo != "0")
            {
                string[] tokboxArray = tokboxInfo.Split('*');
                string sessionId = tokboxArray[0].ToString();
                string tokenId = tokboxArray[1].ToString();
                string tokboxApiKey="";// = tokboxArray[2].ToString();
                string userType= "";//= tokboxArray[3].ToString();
                string patientName = "Patient";
             //   if (tokboxArray[2]!=null) patientName = tokboxArray[2].ToString();
                string doctorName = "Doctor";
             //   if (tokboxArray[3] != null) doctorName = tokboxArray[3].ToString();


                var userMgr = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userMgr.GetRoles(SessionHandler.UserId);
                userType = roles[0].ToString();
                if (roles.Contains("Doctor"))
                {
                    userType = "Doctor";
                    ViewBag.RecipientName = patientName;// openTokSession.PatientName;
                }
                else
                {
                    userType = "Patient";
                    ViewBag.RecipientName = doctorName;// openTokSession.DoctorName;
                }
                ViewBag.UserType = userType;// openTokSession.UserType;
                ViewBag.OpenTokApiKey =  UserChatHelper.TokBoxApiKey;
                ViewBag.OpenTokSession = sessionId;// openTokSession.SessionId;
                ViewBag.OpenTokToken = tokenId;// openTokSession.TokenId;
            }
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
        public void ConsultCreatedbyPatient(long doctorId)
        {
            oVideoCallRepository.ConsultCreatedLog(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void ConsultCreationFailedbyPatient(long doctorId)
        {
            oVideoCallRepository.ConsultCreationFailedLog(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void ConsultCreatedbyDoctor(long patientId)
        {
            oVideoCallRepository.ConsultCreatedLog(patientId,SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void ConsultCreationFailedbyDoctor(long patientId)
        {
            oVideoCallRepository.ConsultCreationFailedLog(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        
        [HttpPost]
        public void AddCallLogbyDoctor(long patientId,string message)
        {
            oVideoCallRepository.AddCallLog(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email,message);
        }

        [HttpPost]
        public void AddCallLogbyPatient(long doctorId, string message)
        {
            oVideoCallRepository.AddCallLog(SessionHandler.UserInfo.Id,doctorId, SessionHandler.UserInfo.Email, message);
        }



        [HttpPost]
        public void TokBoxInfoCreatedbyDoctor(long patientId)
        {
            oVideoCallRepository.TokboxCreatedLog(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void TokBoxInfoCreationFailedbyDoctor(long patientId)
        {
            oVideoCallRepository.TokboxFailLog(patientId, SessionHandler.UserInfo.Id, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void TokBoxInfoCreatedbyPatient(long doctorId)
        {
            oVideoCallRepository.TokboxCreatedLog(SessionHandler.UserInfo.Id, doctorId,SessionHandler.UserInfo.Email);
        }
        [HttpPost]
        public void TokBoxInfoCreationFailedbyPatient(long doctorId)
        {
            oVideoCallRepository.TokboxFailLog(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void PaymentSuccessLog(long doctorId)
        {
            oVideoCallRepository.PaymentSuccessLog(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
        }

        [HttpPost]
        public void PaymentFailLog(long doctorId)
        {
            oVideoCallRepository.PaymentFailLog(SessionHandler.UserInfo.Id, doctorId, SessionHandler.UserInfo.Email);
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
        public void AddVCLog(long consultId, string endReason,int callDuration)
        {
            oVideoCallRepository.AddVCLog(new VCLogModel { consultID = consultId, endBy = SessionHandler.UserInfo.Email, endReason = endReason, logBy = SessionHandler.UserInfo.Email,duration=callDuration });
        }

        [HttpPost]
        public void AddChatMessages(long consultId, string message, string sender, string receiver)
        {
            oVideoCallRepository.AddChatMessages(new ChatMessageModel { consultID = consultId, message = message, sender = sender, reciever = SessionHandler.UserInfo.Email });
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
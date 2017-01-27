using DataAccess.CustomModels;
using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.VideoCallRepository;

namespace WebApp.Helper
{
    public class OpenTokSession
    {
        public string UserType { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string SessionId { get; set; }
        public string TokenId { get; set; }
        public string ConsultId { get; set; }
    }

    public class UserChatHelper
    {

        #region Declarations

        public static int TokBoxApiKey = Convert.ToInt32(ConfigurationManager.AppSettings["TokBoxApiKey"].ToString());
        private static string TokBoxSecretKey = ConfigurationManager.AppSettings["TokBoxSecretKey"].ToString();


        #endregion

        #region Public Methods

        public static OpenTokSession GetOpenTokSessionInformation(string senderId, string receiverId, string userType, string doctorName, string patientName,  string appId = "")
        {
            OpenTokSession oRet = null;

           /* var lstAllOpenTokSessions = (List<OpenTokSession>)(HttpContext.Current.Application["AllOpenTokSessions"]);
            if (lstAllOpenTokSessions != null)
            {
                oRet = lstAllOpenTokSessions.FirstOrDefault(x => (x.SenderId == senderId && x.ReceiverId == receiverId) || (x.SenderId == receiverId && x.ReceiverId == senderId));
            }
            else
            {
                lstAllOpenTokSessions = new List<OpenTokSession>();
            }*/

            if (oRet == null || string.IsNullOrEmpty(oRet.SessionId) || string.IsNullOrEmpty(oRet.TokenId))
            {
                //Initiate Open Tok Session
                var sessionId = GenerateOpenTokSession();
                var tokenId = GenerateOpenTokToken(sessionId);

                oRet = new OpenTokSession()
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    SessionId = sessionId,
                    TokenId = tokenId,
                    UserType = userType,
                    DoctorName = doctorName,
                    PatientName = patientName
                };

                //Create consult ID

              /*  if (appId == "")
                {
                    var oRetConsultInfo = new VideoCallRepository().CreateConsultWithoutAppointment(
                        new CreateConsultModel
                        {
                            sessionID = sessionId,
                            token = tokenId,
                            userID = senderId,
                            doctorId = Convert.ToInt64(senderId),
                            patientId = Convert.ToInt64(receiverId)
                        });

                    oRet.ConsultId = oRetConsultInfo.ID.ToString();
                }
                else
                {
                    var oRetConsultInfo = new VideoCallRepository().CreateConsult(
                        new CreateConsultModel
                        {
                            sessionID = sessionId,
                            appID = Convert.ToInt64(appId),
                            token = tokenId,
                            userID = senderId,
                            doctorId = Convert.ToInt64(senderId),
                            patientId = Convert.ToInt64(receiverId)
                        });

                    oRet.ConsultId = oRetConsultInfo.ID.ToString();
                }*/

               // lstAllOpenTokSessions.Add(oRet);
            }

           // HttpContext.Current.Application["AllOpenTokSessions"] = lstAllOpenTokSessions;
            return oRet;
        }

        public static string GenerateOpenTokSession()
        {
            return new HelperRepository().GenerateOpenTokSession();
        }

        public static string GenerateOpenTokToken(string sessionId)
        {
            return new HelperRepository().GenerateOpenTokToken(sessionId);
        }

        #endregion

    }
}
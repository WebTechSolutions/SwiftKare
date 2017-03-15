using DataAccess.CustomModels;
using Newtonsoft.Json.Linq;
using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RestAPIs.Helpers
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
         
        public static JObject GenerateOpenTokSession()
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);
          //  OpenTokSession session = new OpenTokSession();
          //  session.SessionId= openTok.CreateSession(mediaMode: MediaMode.RELAYED).Id;
            dynamic sessionId = new JObject();
            sessionId.SessionId= openTok.CreateSession(mediaMode: MediaMode.RELAYED).Id;
           
            return sessionId;
        }

        public static string GenerateOpenTokSessionWeb()
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);
            //  OpenTokSession session = new OpenTokSession();
           string sessionId= openTok.CreateSession(mediaMode: MediaMode.RELAYED).Id;
           
            return sessionId;
        }

        public static JObject GenerateOpenTokToken(string sessionId)
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);
            //By default token is valid for 24 hours. So does not need to modify it
            dynamic token = new JObject();
            token.TokenId = openTok.GenerateToken(sessionId);
            return token;
        }

        public static string GenerateOpenTokTokenWeb(string sessionId)
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);
            //By default token is valid for 24 hours. So does not need to modify it
           // dynamic token = new JObject();
            string token= openTok.GenerateToken(sessionId);
            return token;
        }

        #endregion

    }
}
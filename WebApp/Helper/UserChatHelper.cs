using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApp.Helper
{
    public class UserChatHelper
    {

        #region Declarations

        public static int TokBoxApiKey = Convert.ToInt32(ConfigurationManager.AppSettings["TokBoxApiKey"].ToString());
        private static string TokBoxSecretKey = ConfigurationManager.AppSettings["TokBoxSecretKey"].ToString();


        #endregion

        #region Public Methods


        public static string GenerateOpenTokSession()
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);
            return openTok.CreateSession(mediaMode: MediaMode.RELAYED).Id;
        }

        public static string GenerateOpenTokToken(string sessionId)
        {
            var openTok = new OpenTok(TokBoxApiKey, TokBoxSecretKey);

            //By default token is valid for 24 hours. So does not need to modify it
            return openTok.GenerateToken(sessionId);
        }

        #endregion

    }
}
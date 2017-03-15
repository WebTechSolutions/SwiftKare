using System;

namespace RestAPIs.Models
{
    public class OauthUserModel
    {
        public string OauthUserName = ApplicationGlobalVariables.Instance.OauthUserName;
        public string OauthPassword = ApplicationGlobalVariables.Instance.OauthPassword;
        public string OauthClient = ApplicationGlobalVariables.Instance.OauthClientId;
        public string OauthClientSecret = ApplicationGlobalVariables.Instance.OauthClientSecret;

        public double OauthAccessTokenExpireSeconds = Convert.ToDouble(ApplicationGlobalVariables.Instance.OauthAccessTokenExpireSeconds);
    }
}
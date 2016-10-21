using System;
using System.Web.Configuration;

namespace RestAPIs.Models
{
    public sealed class ApplicationGlobalVariables
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ApplicationGlobalVariables instance = new ApplicationGlobalVariables();
        public readonly bool ApplyHttpsFilterRequired;
        public readonly bool ApplyIPsAuthorizationFilter;
        public readonly string ConnectionString;
        public readonly bool IsApiHelpEnabled;
        public readonly string OauthUserName;
        public readonly string OauthPassword;

        public readonly string OauthClientId;
        public readonly string OauthClientSecret;

        public readonly string OauthAccessTokenExpireSeconds;
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ApplicationGlobalVariables()
        {
        }

        private ApplicationGlobalVariables()
        {
            ApplyHttpsFilterRequired = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigApplyHttpsFilter)) && Convert.ToBoolean(WebConfigurationManager.AppSettings[ConfigApplyHttpsFilter]);
            ApplyIPsAuthorizationFilter = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigApplyIPsAuthorizationFilter)) && Convert.ToBoolean(WebConfigurationManager.AppSettings[ConfigApplyIPsAuthorizationFilter]);
            IsApiHelpEnabled = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigIsApiHelpEnabled)) && Convert.ToBoolean(WebConfigurationManager.AppSettings[ConfigIsApiHelpEnabled]);

            OauthUserName = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigOauthUserName)) ? WebConfigurationManager.AppSettings[ConfigOauthUserName] : "";
            OauthPassword = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigOauthPassword)) ? WebConfigurationManager.AppSettings[ConfigOauthPassword] : "";


            OauthClientId = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigOauthClientId)) ? WebConfigurationManager.AppSettings[ConfigOauthClientId] : "";
            OauthClientSecret = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigOauthClientSecret)) ? WebConfigurationManager.AppSettings[ConfigOauthClientSecret] : "";


            OauthAccessTokenExpireSeconds = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigOauthAccessTokenExpireSeconds)) ? WebConfigurationManager.AppSettings[ConfigOauthAccessTokenExpireSeconds] : "3600";

            try
            {
                if (string.IsNullOrEmpty(WebConfigurationManager.ConnectionStrings[ConfigConnectionString].ToString()))
                    return;
                ConnectionString = WebConfigurationManager.ConnectionStrings[ConfigConnectionString].ToString();
            }
            catch (Exception)
            {
                ConnectionString = "";
            }
        }


        public static ApplicationGlobalVariables Instance
        {
            get { return instance; }
        }

        #region Constants
        private const string ConfigApplyHttpsFilter = "ApplyHttpsFilter";
        private const string ConfigApplyIPsAuthorizationFilter = "ApplyIPsAuthorizationFilter";
        private const string ConfigConnectionString = "ConnectionString";
        private const string ConfigIsApiHelpEnabled = "IsApiHelpEnabled";
        private const string ConfigOauthUserName= "OauthUserName";
        private const string ConfigOauthPassword = "OauthPassword";

        private const string ConfigOauthClientId = "OauthClientId";
        private const string ConfigOauthClientSecret = "OauthClientSecret";


        private const string ConfigOauthAccessTokenExpireSeconds = "OauthAccessTokenExpireSeconds";
        #endregion
    }


    public static class Messages
    {
        public const string MissingToken = "Authorization Token is missing";
        public const string InvalidToken = "User is not authorized to access this api.";
        public const string HttpsInvalidMessage = "Https is required";
        public const string UnauthorizedIpAddress = "IP is not authorized";
    }
}
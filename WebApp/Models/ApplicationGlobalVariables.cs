using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WebApp.Models
{
    public sealed class ApplicationGlobalVariables
    {
        private static readonly ApplicationGlobalVariables instance = new ApplicationGlobalVariables();
        public readonly string ApiBaseUrl;
        public string ClientId;
        public string Secret;


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ApplicationGlobalVariables()
        {
        }

        //private ApplicationGlobalVariables()
        //{
        //    ApiBaseUrl = "http://localhost:13040/";
        //    ClientId = "abc";
        //    Secret = "ddddddddd";
        //}
        private ApplicationGlobalVariables()
        {
            ApiBaseUrl = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigApiBaseUrl)) ? WebConfigurationManager.AppSettings[ConfigApiBaseUrl] : "http://localhost:13040/";
            ClientId = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigClientId)) ? WebConfigurationManager.AppSettings[ConfigClientId] : "abc";
            Secret = !string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get(ConfigSecret)) ? WebConfigurationManager.AppSettings[ConfigSecret] : "ddddddddd";
        }


        public static ApplicationGlobalVariables Instance
        {
            get
            {
                return instance;
            }
        }

        private const string ConfigApiBaseUrl = "ApiBaseUrl";
        private const string ConfigClientId = "ClientId";
        private const string ConfigSecret = "Secret";
    }
}

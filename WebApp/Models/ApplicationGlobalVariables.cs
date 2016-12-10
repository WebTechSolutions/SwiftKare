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

        private ApplicationGlobalVariables()
        {
            ApiBaseUrl = "http://localhost:13040/";
            // ApiBaseUrl = "http://13.91.42.71:8079/";
            ClientId = "abc";
            Secret = "ddddddddd";
        }



        public static ApplicationGlobalVariables Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

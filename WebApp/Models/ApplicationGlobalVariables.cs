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
            

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ApplicationGlobalVariables()
        {
        }

        private ApplicationGlobalVariables()
        {
            ApiBaseUrl = "http://localhost:13040/";
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

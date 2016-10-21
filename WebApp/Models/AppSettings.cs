using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WebApp.Models
{
    public static class AppSettings
    {
        public static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AppName");
            }
        }
        
    }
}
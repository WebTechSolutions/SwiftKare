using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushNotiProject.Models
{
    public class PN_AppModel
    {
        public String title { get; set; }
        public String alert { get; set; }
        public int badge { get; set; }
        public String sound { get; set; }
    }
}
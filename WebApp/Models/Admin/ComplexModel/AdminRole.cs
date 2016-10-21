using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SwiftKare.Models.ComplexModel
{
    public class AdminRole
    {
        public int adminID { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int roleID { get; set; }
        public string roleName { get; set; }
    }
}
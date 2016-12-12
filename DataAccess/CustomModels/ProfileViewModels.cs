using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{

    public class SecretQuestionVM
    {
        public long secretQuestionID { get; set; }
        public string secretQuestion { get; set; }
    }

    public class TimeZoneVM
    {
        public long zoneID { get; set; }
        public string timeZone { get; set; }
    }

    public class CityVM
    {
        public long cityID { get; set; }
        public string cityName { get; set; }
    }

    public class StateVM
    {
        public long stateID { get; set; }
        public string stateName { get; set; }
    }

}

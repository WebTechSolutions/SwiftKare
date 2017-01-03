using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class SurgeriesModel
    {
        //public long surgeryID { get; set; } 
        public string surgeryName { get; set; }
    }
    public class PSurgeries
    {

        public Nullable<long> surgeryID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string bodyPart { get; set; }
       
    }
}

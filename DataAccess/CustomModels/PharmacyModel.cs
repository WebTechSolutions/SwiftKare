using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PharmacyModel
    {
        public long pharmacyID { get; set; }
        public long patientID { get; set; }
        public string pharmacyName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class GetPatientSurgeries
    {
        public long surgeryID { get; set; }
        public string bodyPart { get; set; }
    }
}

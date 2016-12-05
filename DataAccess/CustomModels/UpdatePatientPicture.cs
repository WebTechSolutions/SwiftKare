using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class UpdatePatientPicture
    {
        public long patientID { get; set; }
        public byte[] picture { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class UpdateDoctorPicture
    {
        public long doctorID { get; set; }
        public byte[] picture { get; set; }
    }
}

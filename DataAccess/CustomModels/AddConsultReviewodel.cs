using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class AddConsultReviewodel
    {
        public long consultID { get; set; }
        public long patientID { get; set; }
        public int star { get; set; }
        public string reviewText { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class VCLogModel
    {
        public long consultID { get; set; }
        public int duration { get; set; }
        public string endBy { get; set; }
        public string endReason { get; set; }
        public string logBy { get; set; }
    }
}

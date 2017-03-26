using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class ROV_Custom
    {
        public long rovID { get; set; }
        public string rov { get; set; }
        //public string chiefcomplaints { get; set; }
        //public long patientId { get; set; }
    }
    public partial class SystemItemsModel
    {
        public long systemItemID { get; set; }
        public string systemItemName { get; set; }
        public Nullable<long> systemID { get; set; }
        public string systemName { get; set; }
    }
}

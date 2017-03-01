using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class ConsultationSOAPModel
    {
        public long consultID { get; set; }
        public string subjective { get; set; }
        public string objective { get; set; }
        public string assessment { get; set; }
        public string plans { get; set; }
        public List<ChatLogModel> chat { get; set; }
        public List<ROSVM> rosItems { get; set; }
    }
    public class ConsultationModel
    {
        public long consultID { get; set; }
        public string subjective { get; set; }
        public string objective { get; set; }
        public string assessment { get; set; }
        public string plans { get; set; }
        public List<ROSVM> rosItems { get; set; }
        public DoctorVM DoctorVM { get; set; }
        public PatientVM PatientVM { get; set; }
        public AppointmentVM AppointmentVM { get; set; }
        public List<AppFiles> AppFiles { get; set; }

    }
    public class ChatLogModel
    {
        public string sender { get; set; }
        public string reciever { get; set; }
        public string message { get; set; }
        public DateTime? cd { get; set; }
    }
    public class ROSVM
    {
        public string systemItemName { get; set; }
    }
    public class AppointmentVM
    {
        public long appID { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
        public int? paymentAmt { get; set; }
    }

    public class ROSItem {

        public ROSItem() {
            systemItems = new List<ROSItemDetail>();
        }

        public long systemID { get; set; }
        public string systemName { get; set; }

        public List<ROSItemDetail> systemItems { get; set; }
    }

    public class ROSItemDetail
    {
        public long systemItemID { get; set; }
        public string systemItemName { get; set; }
    }

}

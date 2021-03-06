﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CommonModels
{
   public  class AppointmentModel
    {
        //public long appID { get; set; }
        public Nullable<long> doctorID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string notes { get; set; }
        public string paymentID { get; set; }
        public Nullable<int> paymentAmt { get; set; }
        //public string appTypeAV { get; set; }
        public string chiefComplaints { get; set; }
        public string rov { get; set; }


        public string rovFile1Base64 { get; set; }
        public string rovFile1Name { get; set; }

        public string rovFile2Base64 { get; set; }
        public string rovFile2Name { get; set; }

        public string rovFile3Base64 { get; set; }
        public string rovFile3Name { get; set; }

        //public string consultationType { get; set; }
        //public string consultationStatus { get; set; }
    }

    public class RescheduleAppointmentModel
    {
        public long appID { get; set; }
        public string userID { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string appType { get; set; }
    }
    public class CompleteConsultPatient
    {
        public long appID { get; set; }
        public string userID { get; set; }
        
    }
    public class CompleteConsultDoctor
    {
        public long appID { get; set; }
        public string userID { get; set; }

    }
}

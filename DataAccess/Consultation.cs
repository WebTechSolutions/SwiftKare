//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Consultation
    {
        public long consultID { get; set; }
        public Nullable<long> appID { get; set; }
        public Nullable<int> duration { get; set; }
        public string review { get; set; }
        public Nullable<int> reviewStar { get; set; }
        public Nullable<bool> reviewStatus { get; set; }
        public Nullable<long> doctorID { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<long> consultationRosID { get; set; }
        public string subjective { get; set; }
        public string objective { get; set; }
        public string assessment { get; set; }
        public string plans { get; set; }
        public string cb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<System.TimeSpan> startTime { get; set; }
        public Nullable<System.TimeSpan> endTime { get; set; }
        public string seesionID { get; set; }
        public string token { get; set; }
        public string endby { get; set; }
        public string status { get; set; }
    
        public virtual ConsultationRO ConsultationRO { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }
}

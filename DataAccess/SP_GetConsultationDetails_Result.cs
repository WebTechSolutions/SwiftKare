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
    
    public partial class SP_GetConsultationDetails_Result
    {
        public string DoctorName { get; set; }
        public string gender { get; set; }
        public string specialityName { get; set; }
        public Nullable<int> DoctorAge { get; set; }
        public byte[] picture { get; set; }
        public string PatientName { get; set; }
        public string pharmacy { get; set; }
        public byte[] PatientPicture { get; set; }
        public Nullable<int> PatientAge { get; set; }
        public Nullable<System.DateTime> appDate { get; set; }
        public string chiefComplaints { get; set; }
        public string rov { get; set; }
        public string subjective { get; set; }
        public string assessment { get; set; }
        public string objective { get; set; }
        public string plans { get; set; }
    }
}

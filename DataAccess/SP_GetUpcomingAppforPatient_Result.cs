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
    
    public partial class SP_GetUpcomingAppforPatient_Result
    {
        public long doctorID { get; set; }
        public string DoctorName { get; set; }
        public long patientID { get; set; }
        public string PatientName { get; set; }
        public Nullable<System.DateTime> appDate { get; set; }
        public Nullable<System.TimeSpan> appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
    }
}

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
    
    public partial class SP_GetDcotorConsultations_Result
    {
        public long consultID { get; set; }
        public string PatientName { get; set; }
        public int duration { get; set; }
        public string review { get; set; }
        public int reviewStar { get; set; }
        public Nullable<bool> reviewStatus { get; set; }
        public string appTime { get; set; }
        public string appDate { get; set; }
        public string rov { get; set; }
        public string gender { get; set; }
        public string languageName { get; set; }
        public long patientID { get; set; }
        public string utcappDate { get; set; }
    }
}

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
    
    public partial class SP_GetPatientAllergies_Result
    {
        public long allergiesID { get; set; }
        public string allergyName { get; set; }
        public string severity { get; set; }
        public string reaction { get; set; }
        public string source { get; set; }
        public Nullable<long> patientID { get; set; }
        public string status { get; set; }
        public string cb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<System.DateTime> reportedDate { get; set; }
    }
}

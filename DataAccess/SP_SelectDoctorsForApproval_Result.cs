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
    
    public partial class SP_SelectDoctorsForApproval_Result
    {
        public long doctorID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string secretQuestion1 { get; set; }
        public string secretQuestion2 { get; set; }
        public string secretQuestion3 { get; set; }
        public string secretAnswer1 { get; set; }
        public string secretAnswer2 { get; set; }
        public string secretAnswer3 { get; set; }
        public string gender { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string zip { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public string licenseNumber { get; set; }
        public string npiNumber { get; set; }
        public string additionalInfo { get; set; }
        public byte[] picture { get; set; }
        public string cb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<double> lat { get; set; }
        public Nullable<double> lon { get; set; }
        public Nullable<bool> status { get; set; }
        public string publication { get; set; }
        public string aboutMe { get; set; }
        public string userId { get; set; }
    }
}

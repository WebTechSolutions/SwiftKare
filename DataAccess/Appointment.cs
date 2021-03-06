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
    
    public partial class Appointment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Appointment()
        {
            this.AppAttachments = new HashSet<AppAttachment>();
            this.Consultations = new HashSet<Consultation>();
        }
    
        public long appID { get; set; }
        public Nullable<long> doctorID { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<System.DateTime> appDate { get; set; }
        public Nullable<System.TimeSpan> appTime { get; set; }
        public string notes { get; set; }
        public string paymentID { get; set; }
        public Nullable<int> paymentAmt { get; set; }
        public string cb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
        public string appTypeAV { get; set; }
        public string chiefComplaints { get; set; }
        public string rov { get; set; }
        public string consultationType { get; set; }
        public string appointmentStatus { get; set; }
        public string rescheduleRequiredBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppAttachment> AppAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consultation> Consultations { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }
}

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
    
    public partial class DoctorTiming
    {
        public long doctorTimingsID { get; set; }
        public Nullable<long> doctorID { get; set; }
        public string day { get; set; }
        public Nullable<System.TimeSpan> from { get; set; }
        public Nullable<System.TimeSpan> to { get; set; }
        public string cb { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
        public string mb { get; set; }
        public Nullable<System.DateTime> md { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<bool> repeatTimings { get; set; }
    
        public virtual Doctor Doctor { get; set; }
    }
}

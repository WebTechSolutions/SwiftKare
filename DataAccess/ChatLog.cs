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
    
    public partial class ChatLog
    {
        public long chatID { get; set; }
        public string sender { get; set; }
        public string reciever { get; set; }
        public string message { get; set; }
        public Nullable<long> consultID { get; set; }
        public Nullable<System.DateTime> cd { get; set; }
    }
}
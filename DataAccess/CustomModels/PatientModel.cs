﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientModel
    {
        public long patientID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string userId { get; set; }
        public string secretQuestion1 { get; set; }
        public string secretQuestion2 { get; set; }
        public string secretQuestion3 { get; set; }

        public byte[] picture { get; set; }
        public string ProfilePhotoBase64 { get; set; }
        public string secretAnswer1 { get; set; }
        public string secretAnswer2 { get; set; }
        public string secretAnswer3 { get; set; }

        public Nullable<bool> active { get; set; }

        public string title { get; set; }
        public string timeZone { get; set; }
        public string timeZoneoffset { get; set; }
        public string role { get; set; }

        public string iOSToken { get; set; }
        public string AndroidToken { get; set; }
    }
}

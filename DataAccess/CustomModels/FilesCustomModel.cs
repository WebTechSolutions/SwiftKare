﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class FilesCustomModel
    {
        public string documentType { get; set; }
        public string FileName { get; set; }
        public string fileContent { get; set; }
        public string fileContentBase64 { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<long> doctorID { get; set; }
    }

    public class PatientFileType
    {
        public int fileTypeID { get; set; }
        public string typeName { get; set; }
    }
}

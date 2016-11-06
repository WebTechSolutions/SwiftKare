using System;

namespace DataAccess.CustomModels
{
    public class PatientMedication_Custom
    {
        public long medicationID { get; set; }
        public string medicineName { get; set; }
        public string frequency { get; set; }
        public Nullable<long> patientId { get; set; }
        public string userId { get; set; }
      
      
      
    }
}

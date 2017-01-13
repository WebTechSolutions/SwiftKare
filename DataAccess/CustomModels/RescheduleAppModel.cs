using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class RescheduleAppModel
    {
        public string consultationStatus { get; set; }
        public long appID { get; set; }

        public long doctorID { get; set;}
        public long patientID { get; set; }
        public string DoctorName { get; set;}
        public string PatientName { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
        public string utcappDate { get; set; }    
    }
    public class ReschedulePendingAppModel
    {
        public string consultationStatus { get; set; }
        public long appID { get; set; }
        public long? consultID { get; set; }
        public long doctorID { get; set; }
        public long patientID { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
        public string utcappDate { get; set; }
    }

    public class GetAppDetail
    {

        public long appID { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string rov { get; set; }
        public string chiefComplaints { get; set; }
        public int? paymentAmt { get; set; }
        public DoctorVM DoctorVM { get; set; }
        public PatientVM PatientVM { get; set; }
        public string utcappDate { get; set; }
    }


    public class DoctorVM
    {
        public string ProfilePhotoBase64 { get; set; }
        public byte[] docPicture { get; set; }
        public string doctorName { get; set; }
        public string doctorGender { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public DateTime? doctordob { get; set; }
        public string dcellPhone { get; set; }
        public List<SpecVM> specialities { get; set; }
        public List<LangVM> languages { get; set; }

        public string GetAge()
        {
            try
            {
                if (doctordob.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - doctordob.Value.Year;

                    // Do stuff with it.
                    if (doctordob.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} Years", age);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ConvertBase64ToByteArray()
        {
            if (!string.IsNullOrEmpty(ProfilePhotoBase64))
            {
                docPicture = Encoding.ASCII.GetBytes(ProfilePhotoBase64);
            }
        }

        public void ConvertByteArrayToBase64()
        {
            if (docPicture != null && docPicture.Count() > 0)
            {
                ProfilePhotoBase64 = Encoding.ASCII.GetString(docPicture);
            }
        }
    }

    public class PatientVM
    {
        public string ProfilePhotoBase64 { get; set; }
        public byte[] patPicture { get; set; }
        public string patientName { get; set; }
        public string pcellPhone { get; set; }
        public DateTime? patientDOB { get; set; }
        public List<LangVM> patlanguages { get; set; }
        public string patientGender { get; set; }
        public string pharmacy { get; set; }

        public string GetAge()
        {
            try
            {
                if (patientDOB.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - patientDOB.Value.Year;

                    // Do stuff with it.
                    if (patientDOB.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} Years", age);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ConvertBase64ToByteArray()
        {
            if (!string.IsNullOrEmpty(ProfilePhotoBase64))
            {
                patPicture = Encoding.ASCII.GetBytes(ProfilePhotoBase64);
            }
        }

        public void ConvertByteArrayToBase64()
        {
            if (patPicture != null && patPicture.Count() > 0)
            {
                ProfilePhotoBase64 = Encoding.ASCII.GetString(patPicture);
            }
        }
    }

    public class LangVM
    {
        public string languageName { get; set; }
    }

    public class SpecVM
    {
        public string specialityName { get; set; }
    }

    public class GetDoctorINFOVM
    {
        public string ProfilePhotoBase64 { get; set; }
        public byte[] docPicture { get; set; }
        public string doctorName { get; set; }
        public string doctorGender { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string email { get; set; }
        public int? consultCharges { get; set; }
        public DateTime? doctordob { get; set; }
        public string dcellPhone { get; set; }
        public List<SpecVM> specialities { get; set; }
        public List<LangVM> languages { get; set; }

        public string GetAge()
        {
            try
            {
                if (doctordob.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - doctordob.Value.Year;

                    // Do stuff with it.
                    if (doctordob.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} Years", age);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ConvertBase64ToByteArray()
        {
            if (!string.IsNullOrEmpty(ProfilePhotoBase64))
            {
                docPicture = Encoding.ASCII.GetBytes(ProfilePhotoBase64);
            }
        }

        public void ConvertByteArrayToBase64()
        {
            if (docPicture != null && docPicture.Count() > 0)
            {
                ProfilePhotoBase64 = Encoding.ASCII.GetString(docPicture);
            }
        }
    }
}

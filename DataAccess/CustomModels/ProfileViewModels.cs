using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{

    public class DoctorProfileVM
    {

        //Doctor Profile
        public long DoctorID { get; set; }

        public string ProfilePhotoBase64 { get; set; }
        public byte[] ProfilePhoto { get; set; }

        public string Title { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }
        public string TimeZone { get; set; }

        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public string[] Speciality { get; set; }
        public string Publication { get; set; }
        public string Education { get; set; }

        public string WorkExperience { get; set; }
        public string[] Languages { get; set; }
        public string[] LicenseStates { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //Consult Charges
        public long? ConsultCharges { get; set; }

        //Secret Question and Answers
        public string SectetQuestion1 { get; set; }
        public string SectetQuestion2 { get; set; }
        public string SectetQuestion3 { get; set; }

        public string SectetAnswer1 { get; set; }
        public string SectetAnswer2 { get; set; }
        public string SectetAnswer3 { get; set; }

        #region Methods

        public void ConvertBase64ToByteArray()
        {
            if (!string.IsNullOrEmpty(ProfilePhotoBase64))
            {
                ProfilePhoto = Encoding.ASCII.GetBytes(ProfilePhotoBase64);
            }
        }

        public void ConvertByteArrayToBase64()
        {
            if (ProfilePhoto != null && ProfilePhoto.Count() > 0)
            {
                ProfilePhotoBase64 = Encoding.ASCII.GetString(ProfilePhoto);
            }
        }

        public string GetAge()
        {
            try
            {
                if (DOB.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - DOB.Value.Year;

                    // Do stuff with it.
                    if (DOB.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} year", age);
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

        #endregion
    }

    public class DoctorProfileInitialValues
    {
        public DoctorProfileInitialValues()
        {
            lstSpecialityVM = new List<SpecialityVM>();
            lstLanguageVM = new List<LanguageVM>();
            lstSecretQuestionVM = new List<SecretQuestionVM>();
            lstTimeZoneVM = new List<TimeZoneVM>();
            lstCityVM = new List<CityVM>();
            lstStateVM = new List<StateVM>();
        }

        public List<SpecialityVM> lstSpecialityVM { get; set; }
        public List<LanguageVM> lstLanguageVM { get; set; }
        public List<SecretQuestionVM> lstSecretQuestionVM { get; set; }
        public List<TimeZoneVM> lstTimeZoneVM { get; set; }
        public List<CityVM> lstCityVM { get; set; }
        public List<StateVM> lstStateVM { get; set; }
    }

    public class PatientProfileInitialValues
    {
        public PatientProfileInitialValues()
        {
            lstLanguageVM = new List<LanguageVM>();
            lstSecretQuestionVM = new List<SecretQuestionVM>();
            lstTimeZoneVM = new List<TimeZoneVM>();
            lstCityVM = new List<CityVM>();
            lstStateVM = new List<StateVM>();
        }

        public List<LanguageVM> lstLanguageVM { get; set; }
        public List<SecretQuestionVM> lstSecretQuestionVM { get; set; }
        public List<TimeZoneVM> lstTimeZoneVM { get; set; }
        public List<CityVM> lstCityVM { get; set; }
        public List<StateVM> lstStateVM { get; set; }
    }


    public class PatientProfileVM
    {
        //Patient Profile
        public long PatientID { get; set; }

        public string ProfilePhotoBase64 { get; set; }
        public byte[] ProfilePhoto { get; set; }

        public string Title { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }
        public string TimeZone { get; set; }

        public int? Height { get; set; }
        public int? Weight { get; set; }

        public string[] Languages { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //Secret Question and Answers
        public string SectetQuestion1 { get; set; }
        public string SectetQuestion2 { get; set; }
        public string SectetQuestion3 { get; set; }

        public string SectetAnswer1 { get; set; }
        public string SectetAnswer2 { get; set; }
        public string SectetAnswer3 { get; set; }

        #region Methods

        public void ConvertBase64ToByteArray()
        {
            if (!string.IsNullOrEmpty(ProfilePhotoBase64))
            {
                ProfilePhoto = Encoding.ASCII.GetBytes(ProfilePhotoBase64);
            }
        }

        public void ConvertByteArrayToBase64()
        {
            if (ProfilePhoto != null && ProfilePhoto.Count() > 0)
            {
                ProfilePhotoBase64 = Encoding.ASCII.GetString(ProfilePhoto);
            }
        }

        public string GetAge()
        {
            try
            {
                if (DOB.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - DOB.Value.Year;

                    // Do stuff with it.
                    if (DOB.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} year", age);
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

        #endregion
    }


    public class SpecialityVM
    {
        public long specialityID { get; set; }
        public string specialityName { get; set; }
    }

    public class LanguageVM
    {
        public long languageID { get; set; }
        public string languageName { get; set; }
    }

    public class SecretQuestionVM
    {
        public long secretQuestionID { get; set; }
        public string secretQuestion { get; set; }
    }

    public class TimeZoneVM
    {
        public long zoneID { get; set; }
        public string timeZone { get; set; }
    }

    public class CityVM
    {
        public long cityID { get; set; }
        public string cityName { get; set; }
    }

    public class StateVM
    {
        public long stateID { get; set; }
        public string stateName { get; set; }
    }

}

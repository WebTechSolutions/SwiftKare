using DataAccess;
using DataAccess.CustomModels;
using DoseSpot.EncryptionLibrary;
using RestAPIs.DoseSpotApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace RestAPIs.Helper
{
    public class DoseSpotHelper
    {

        public static PharmacySearchMessageResult SearchPharmacy(DoseSpotPharmacySearch oModel)
        {
            APISoapClient api = new DoseSpotApi.APISoapClient("APISoap12");
            SingleSignOn sso = new DoseSpotApi.SingleSignOn();

            var oCredentials = CreateSingleSignOnCodeAndUserIdVerify();

            sso.SingleSignOnClinicId = 664;
            sso.SingleSignOnUserId = 2844;
            sso.SingleSignOnCode = oCredentials.Key;
            sso.SingleSignOnUserIdVerify = oCredentials.Value;

            PharmacySearchMessage oSrch = new PharmacySearchMessage
            {
                PharmacyNameSearch = oModel.Name,
                PharmacyCity = oModel.City,
                PharmacyStateTwoLetters = oModel.State,
                PharmacyZipCode = oModel.Zip,
                SingleSignOn = sso
            };

            PharmacySearchMessageResult oRes = api.PharmacySearch(oSrch);
            return oRes;
        }


        public static string GetEPrescriptionUrl(DoseSpotPatientEntry oModel)
        {
            DoseSpotPatient oDoseSpotPatient = new DoseSpotPatient
            {
                PatientId = oModel.PatientId,
                FirstName = oModel.FirstName,
                MiddleName = "",
                LastName = oModel.LastName,
                DateOfBirth = oModel.DateOfBirth,
                Prefix = "",
                Suffix = "",

                Gender = oModel.Gender,
                Address1 = oModel.Address1,
                Address2 = oModel.Address2,
                City = oModel.City,
                State = oModel.State,
                ZipCode = oModel.ZipCode,

                PrimaryPhone = oModel.Phone,
                PrimaryPhoneType = "Home",
                PhoneAdditional1 = "",
                PhoneAdditionalType1 = "",
                PhoneAdditional2 = "",
                PhoneAdditionalType2 = ""
            };

            //Default Criterias - Starts
            int SingleSignOnUserId = 2844;
            int SingleSignOnClinicId = 664;
            string ClinicKey = "qeF5FJef6T6FNTanQS9HuvvuNdkTvvZT";

            //Default Criterias - Ends

            string cPostData = SingleSignOnUtils.GetSingleSignOnQueryStringForPatient(ClinicKey, SingleSignOnClinicId, SingleSignOnUserId, oDoseSpotPatient);
            string cPrefix = SingleSignOnUtils.GetSingleSignOnPageLocation("my.staging.dosespot.com", true);
            string cRetUrl = cPrefix + cPostData;

            return cRetUrl;
        }

        public static string RegisterPatientWithDoseSpot(DoseSpotPatientEntry oModel)
        {
            DoseSpotPatient oDoseSpotPatient = new DoseSpotPatient
            {
                FirstName = oModel.FirstName,
                MiddleName = "",
                LastName = oModel.LastName,
                DateOfBirth = oModel.DateOfBirth,
                Prefix = "",
                Suffix = "",

                Gender = oModel.Gender,
                Address1 = oModel.Address1,
                Address2 = oModel.Address2,
                City = oModel.City,
                State = oModel.State,
                ZipCode = oModel.ZipCode,

                PrimaryPhone = oModel.Phone,
                PrimaryPhoneType = "Home",
                PhoneAdditional1 = "",
                PhoneAdditionalType1 = "",
                PhoneAdditional2 = "",
                PhoneAdditionalType2 = ""
            };

            //Default Criterias - Starts
            int SingleSignOnUserId = 2844;
            int SingleSignOnClinicId = 664;
            string ClinicKey = "qeF5FJef6T6FNTanQS9HuvvuNdkTvvZT";

            //Default Criterias - Ends

            string cPostData = SingleSignOnUtils.GetSingleSignOnQueryStringForPatient(ClinicKey, SingleSignOnClinicId, SingleSignOnUserId, oDoseSpotPatient);
            string cPrefix = SingleSignOnUtils.GetSingleSignOnPageLocation("my.staging.dosespot.com", true);
            string cRetUrl = cPrefix + cPostData;

            return GetPatientIdFromUrl(cRetUrl);
        }


        /// <summary>
        /// Data-mine patientId from retuened html
        /// </summary>
        /// <param name="urlAddress"></param>
        /// <returns></returns>
        private static string GetPatientIdFromUrl(string urlAddress)
        {
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string data = client.DownloadString(urlAddress);

                //Find Patient Id in response - Starts
                if (!string.IsNullOrEmpty(data))
                {
                    //%3fpatientid%3d342502
                    var startIndex = data.IndexOf("%2fsecure%2fPatientDetail.aspx%3fPatientID%3d") + "%2fsecure%2fPatientDetail.aspx%3fPatientID%3d".Length;
                    var subString = data.Substring(startIndex);

                    var length = subString.IndexOf("\"");

                    var oRetPatientId = data.Substring(startIndex, length);
                    return oRetPatientId;
                }
                //Find Patient Id in response - Ends
            }

            return "";
        }



        private static KeyValuePair<string, string> CreateSingleSignOnCodeAndUserIdVerify()
        {
            string SingleSignOnClinicId = "664";
            string ClinicKey = "qeF5FJef6T6FNTanQS9HuvvuNdkTvvZT";
            int SingleSignOnUserId = 2844;
            var Create32CharPhrase = DoseSpot.EncryptionLibrary.EncryptionCommon.CreatePhrase();

            var oResSignOnCode = DoseSpot.EncryptionLibrary.EncryptionCommon.CreatePhraseEncryptedCombinedString(Create32CharPhrase, ClinicKey);
            var oResUserIdVerify = DoseSpot.EncryptionLibrary.EncryptionCommon.EncryptUserId(Create32CharPhrase, SingleSignOnUserId, ClinicKey);

            return new KeyValuePair<string, string>(HttpContext.Current.Server.HtmlEncode(oResSignOnCode), HttpContext.Current.Server.HtmlEncode(oResUserIdVerify));
        }


        public class DoseSpotPatient
        {

            public int? PatientId { get; set; }


            public string Prefix { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Suffix { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string PrimaryPhone { get; set; }
            public string PrimaryPhoneType { get; set; }
            public string PhoneAdditional1 { get; set; }
            public string PhoneAdditionalType1 { get; set; }
            public string PhoneAdditional2 { get; set; }
            public string PhoneAdditionalType2 { get; set; }

            public float? Height { get; set; }
            public float? Weight { get; set; }
            public float? HeightMetric { get; set; }
            public float? WeightMetric { get; set; }

            public List<ValidationResult> Validate()
            {
                List<ValidationResult> validationResults = new List<ValidationResult>();
                DoseSpotPatientValidation.CheckLength(validationResults, "Prefix", Prefix, 10, false);
                DoseSpotPatientValidation.CheckLength(validationResults, "First Name", FirstName, 35, true);
                DoseSpotPatientValidation.CheckLength(validationResults, "Middle Name", MiddleName, 35, false);
                DoseSpotPatientValidation.CheckLength(validationResults, "LastName", LastName, 35, true);
                DoseSpotPatientValidation.CheckLength(validationResults, "Suffix", Suffix, 10, false);

                DoseSpotPatientValidation.DateOfBirthIsValid(validationResults, "Date of Birth", DateOfBirth, true);
                DoseSpotPatientValidation.GenderIsValid(validationResults, "Gender", Gender, true);

                DoseSpotPatientValidation.CheckLength(validationResults, "Address Line 1", Address1, 35, true);
                DoseSpotPatientValidation.CheckLength(validationResults, "Address Line 2", Address2, 35, false);
                DoseSpotPatientValidation.CheckLength(validationResults, "City", City, 35, true);
                DoseSpotPatientValidation.ZipCodeIsValid(validationResults, "Zip Code", ZipCode, true);

                DoseSpotPatientValidation.PhoneNumberIsValid(validationResults, "Primary Phone", PrimaryPhone, true);
                DoseSpotPatientValidation.PhoneTypeIsValid(validationResults, "Primary Phone Type", PrimaryPhoneType, true);

                DoseSpotPatientValidation.PhoneNumberIsValid(validationResults, "Phone Additional 1", PhoneAdditional1, false);
                DoseSpotPatientValidation.PhoneTypeIsValid(validationResults, "Phone Additional Type 1", PhoneAdditionalType1, false);
                DoseSpotPatientValidation.PhoneNumberIsValid(validationResults, "Phone Additional 2", PhoneAdditional2, false);
                DoseSpotPatientValidation.PhoneTypeIsValid(validationResults, "Phone Additional Type 2", PhoneAdditionalType2, false);
                return validationResults;
            }

            public string ToQueryString()
            {
                StringBuilder sb = new StringBuilder();
                if (PatientId != 0)
                    sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PatientId", PatientId);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Prefix", Prefix);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "FirstName", FirstName);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "MiddleName", MiddleName);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "LastName", LastName);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Suffix", Suffix);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "DateOfBirth", DateOfBirth.ToShortDateString());
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Gender", Gender);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Address1", Address1);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Address2", Address2);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "City", City);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "State", State);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "ZipCode", ZipCode);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PrimaryPhone", PrimaryPhone);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PrimaryPhoneType", PrimaryPhoneType);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PhoneAdditional1", PhoneAdditional1);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PhoneAdditionalType1", PhoneAdditionalType1);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PhoneAdditional2", PhoneAdditional2);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "PhoneAdditionalType2", PhoneAdditionalType2);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Height", Height);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "Weight", Weight);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "HeightMetric", HeightMetric);
                sb = SingleSignOnUtils.QueryStringAddParameter(sb, "WeightMetric", WeightMetric);
                return sb.ToString();
            }
            //public void SetPatientIdFromUri(Uri uri)
            //{
            //    if (uri != null)
            //    {
            //        SetPatientIdFromQueryString(uri.Query);
            //    }
            //}
            //public void SetPatientIdFromUrl(Uri uri)
            //{
            //    SetPatientIdFromUri(uri);
            //}

            //public void SetPatientIdFromQueryString(string QueryString)
            //{
            //    if (!string.IsNullOrEmpty(QueryString))
            //    {
            //        int myPatientId;
            //        string myPatientIdAttempt = WebUtility.GetQueryParameterFromQueryString(QueryString, "PatientId");
            //        if ((!string.IsNullOrEmpty(myPatientIdAttempt)) && Int32.TryParse(myPatientIdAttempt.Trim(), out myPatientId))
            //        {
            //            if (myPatientId != 0)
            //                PatientId = myPatientId;
            //        }
            //    }
            //}
        }

        public class ValidationResult
        {
            protected bool isValid;
            public bool IsValid
            {
                get
                {
                    return isValid;
                }
                set
                {
                    isValid = value;
                }
            }
            public string FieldName = string.Empty;
            public string ErrorDescription = string.Empty;
            public ValidationResult()
            {
                IsValid = true;
            }
            public ValidationResult(string fieldName, string errorDescription)
            {
                IsValid = false;
                FieldName = fieldName;
                ErrorDescription = errorDescription;
            }
        }


        public class SingleSignOnUtils
        {
            public static StringBuilder QueryStringAddParameter(StringBuilder QueryString, string NewParamName, string NewParamValue)
            {
                QueryString.Append("&" + NewParamName + "=");
                if (NewParamName != null)
                    QueryString.Append(Uri.EscapeDataString(NewParamValue));
                return QueryString;
            }
            public static StringBuilder QueryStringAddParameter(StringBuilder QueryString, string NewParamName, int NewParamValue)
            {
                return QueryStringAddParameter(QueryString, NewParamName, NewParamValue.ToString());
            }
            public static StringBuilder QueryStringAddParameter(StringBuilder QueryString, string NewParamName, int? NewParamValue)
            {
                if (!NewParamValue.HasValue)
                    return QueryString;
                else
                    return QueryStringAddParameter(QueryString, NewParamName, NewParamValue.Value.ToString());
            }
            public static StringBuilder QueryStringAddParameter(StringBuilder QueryString, string NewParamName, float? NewParamValue)
            {
                if (!NewParamValue.HasValue)
                    return QueryString;
                else
                    return QueryStringAddParameter(QueryString, NewParamName, NewParamValue.Value.ToString());
            }
            public static StringBuilder QueryStringAddParameter(StringBuilder QueryString, string NewParamName, DateTime NewParamValue)
            {
                return QueryStringAddParameter(QueryString, NewParamName, NewParamValue.ToShortDateString());
            }
            public static string GetSingleSignOnQueryStringForPatient(string ClinicKey, int ClinicId, int UserId, DoseSpotPatient patient)
            {
                string Phrase = EncryptionCommon.CreatePhrase();
                string SingleSignOnCode = EncryptionCommon.CreatePhraseEncryptedCombinedString(Phrase, ClinicKey);
                string SingleSignOnUserIdVerify = EncryptionCommon.EncryptUserId(Phrase, UserId, ClinicKey);
                StringBuilder sb = new StringBuilder();
                QueryStringAddParameter(sb, "SingleSignOnCode", SingleSignOnCode);
                QueryStringAddParameter(sb, "SingleSignOnUserId", UserId);
                QueryStringAddParameter(sb, "SingleSignOnUserIdVerify", SingleSignOnUserIdVerify);
                QueryStringAddParameter(sb, "SingleSignOnClinicId", ClinicId);
                sb.Append(patient.ToQueryString());
                return sb.ToString();
            }
            public static string GetSingleSignOnPageLocation(string ServerName, bool isSecure)
            {
                string returnString = ServerName + "/LoginSingleSignOn.aspx?b=2";
                if (isSecure)
                    return "https://" + returnString;
                else
                    return "http://" + returnString;
            }
        }

        public class DoseSpotPatientValidation
        {
            protected static List<string> myPhoneTypes = new List<string>() { "beeper", "cell", "fax", "home", "work", "night", "primary" };

            public static ValidationResult PhoneTypeIsValid(string paramName, string PhoneTypeValue, bool isRequired)
            {
                if (isRequired && (PhoneTypeValue == null || PhoneTypeValue.Trim().Length == 0))
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                if (PhoneTypeValue != null && PhoneTypeValue.Trim().Length > 0 && (!myPhoneTypes.Contains(PhoneTypeValue.ToLower())))
                    return new ValidationResult(paramName, paramName + " is not Valid");
                return new ValidationResult();
            }
            public static List<ValidationResult> PhoneTypeIsValid(List<ValidationResult> currentResults, string paramName, string PhoneTypeValue, bool isRequired)
            {
                ValidationResult myResult = PhoneTypeIsValid(paramName, PhoneTypeValue, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            public static ValidationResult ZipCodeIsValid(string paramName, string ZipCodeValue, bool isRequired)
            {
                string patternZipCode = @"([0-9]{9})|([0-9]{5}-[0-9]{4})|([0-9]{5})";
                if (isRequired && (ZipCodeValue == null || ZipCodeValue.Trim().Length == 0))
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                return ItemIsValidUsingRegEx(paramName, ZipCodeValue, patternZipCode, "02451", isRequired);
            }
            public static List<ValidationResult> ZipCodeIsValid(List<ValidationResult> currentResults, string paramName, string ZipCodeValue, bool isRequired)
            {
                ValidationResult myResult = ZipCodeIsValid(paramName, ZipCodeValue, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            protected static List<string> myGenders = new List<string>() { "female", "male", "unknown" };
            public static ValidationResult GenderIsValid(string paramName, string Gender, bool isRequired)
            {
                if (isRequired && (Gender == null || Gender.Trim().Length == 0))
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                if (Gender != null && (!myGenders.Contains(Gender.ToLower())))
                    return new ValidationResult(paramName, paramName + " is not Valid");
                return new ValidationResult();
            }
            public static List<ValidationResult> GenderIsValid(List<ValidationResult> currentResults, string paramName, string Gender, bool isRequired)
            {
                ValidationResult myResult = GenderIsValid(paramName, Gender, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            public static ValidationResult DateOfBirthIsValid(string paramName, DateTime DateOfBirth, bool isRequired)
            {
                if (isRequired && DateOfBirth == DateTime.MinValue)
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                if (DateOfBirth > DateTime.Now.Date.AddDays(1))
                    return new ValidationResult(paramName, "'" + paramName + "' is after today which isn't possible.");
                return new ValidationResult();
            }

            public static List<ValidationResult> DateOfBirthIsValid(List<ValidationResult> currentResults, string paramName, DateTime DateOfBirth, bool isRequired)
            {
                ValidationResult myResult = DateOfBirthIsValid(paramName, DateOfBirth, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            public static List<ValidationResult> PhoneNumberIsValid(List<ValidationResult> currentResults, string paramName, string PhoneNumber, bool isRequired)
            {
                string patternPhone = @"^1?\s*-?\s*(\d{3}|\(\s*\d{3}\s*\))\s*-?\s*\d{3}\s*-?\s*\d{4}(X\d{0,9})?";
                if (PhoneNumber != null)
                    PhoneNumber = PhoneNumber.Trim();
                ValidationResult lengthCheck = CheckLength(paramName, PhoneNumber, 35, isRequired);
                if (!lengthCheck.IsValid)
                    return CombineValidationResults(currentResults, lengthCheck);
                if (PhoneNumber == null || PhoneNumber.Trim().Length == 0) // already validated for required fields in previous step.
                    return currentResults;
                ValidationResult myResult = ItemIsValidUsingRegEx(paramName, PhoneNumber, patternPhone, PhoneNumber, isRequired);
                if (!myResult.IsValid)
                    return CombineValidationResults(currentResults, myResult);
                if (PhoneNumber != null && (!CheckAreaCode(PhoneNumber)))
                    return CombineValidationResults(currentResults, new ValidationResult(paramName, paramName + " does not have a Valid Area Code."));
                return currentResults;
            }
            public static bool CheckAreaCode(string PhoneNumber)
            {
                if (PhoneNumber == null || PhoneNumber.Trim().Length == 0)
                    return false;
                Regex re = new Regex(@"/[^\d]/");
                PhoneNumber = re.Replace(PhoneNumber, "");
                string AreaCode = null;
                if (PhoneNumber.Length > 3)
                {
                    if (PhoneNumber.Substring(0, 1) == "1")
                        AreaCode = PhoneNumber.Substring(1, 3);
                    else
                        AreaCode = PhoneNumber.Substring(0, 3);
                    if (!areaCodeList.Contains(AreaCode))
                        return false;
                }
                return true;
            }

            public static ValidationResult ItemIsValidUsingRegEx(string paramName, string paramValue, string RegExPattern, string SampleFormat, bool isRequired)
            {
                if ((paramValue == null || paramValue.Trim().Length == 0) && isRequired)
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                Regex pattern = new Regex(RegExPattern, RegexOptions.IgnoreCase);
                if (paramValue != null && (!pattern.IsMatch(paramValue)))
                    return new ValidationResult(paramName, "'" + paramName + "' is not in the correct format.  (example: " + SampleFormat + ")");

                return new ValidationResult();
            }
            public static List<ValidationResult> ItemIsValidUsingRegEx(List<ValidationResult> currentResults, string paramName, string paramValue, string RegExPattern, string SampleFormat, bool isRequired)
            {
                ValidationResult myResult = ItemIsValidUsingRegEx(paramName, paramValue, RegExPattern, SampleFormat, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            public static ValidationResult CheckLength(string paramName, string paramValue, int maxLength, bool isRequired)
            {
                if ((paramValue == null || paramValue.Trim().Length == 0) && isRequired)
                    return new ValidationResult(paramName, "'" + paramName + "' is required.");
                if (paramValue != null && paramValue.Trim().Length > maxLength)
                    return new ValidationResult(paramName, "'" + paramName + "' is too long.  The maximum length is " + maxLength.ToString() + " characters.");

                return new ValidationResult();
            }
            public static List<ValidationResult> CheckLength(List<ValidationResult> currentResults, string paramName, string paramValue, int maxLength, bool isRequired)
            {
                ValidationResult myResult = CheckLength(paramName, paramValue, maxLength, isRequired);
                return CombineValidationResults(currentResults, myResult);
            }
            public static List<ValidationResult> CombineValidationResults(List<ValidationResult> currentResults, ValidationResult newResult)
            {
                if (currentResults == null)
                    currentResults = new List<ValidationResult>();
                if (!newResult.IsValid)
                    currentResults.Add(newResult);
                return currentResults;
            }
            public static bool IsValid(List<ValidationResult> currentResults)
            {
                if (currentResults == null || currentResults.Count == 0)
                    return true;
                return false;
            }
            public static string GetValidationMessage(List<ValidationResult> currentResults)
            {
                StringBuilder sbError = new StringBuilder();
                if (currentResults == null || currentResults.Count == 0)
                    return String.Empty;
                foreach (ValidationResult r in currentResults)
                {
                    if (!r.IsValid)
                        sbError.AppendLine(r.ErrorDescription);
                }
                return sbError.ToString();
            }

            public static List<string> areaCodeList = new List<string>() { "201", "202", "203", "204", "205", "206", "207", "208", "209", "210", "212", "213", "214", "215", "216", "217", "218", "219", "224", "225", "226", "228", "229", "231", "234", "239", "240", "242", "246", "248", "250", "251", "252", "253", "254", "256", "260", "262", "264", "267", "268", "269", "270", "276", "281", "284", "289", "301", "302", "303", "304", "305", "306", "307", "308", "309", "310", "312", "313", "314", "315", "316", "317", "318", "319", "320", "321", "323", "325", "330", "331", "334", "336", "337", "339", "340", "343", "345", "347", "351", "352", "360", "361", "385", "386", "401", "402", "403", "404", "405", "406", "407", "408", "409", "410", "412", "413", "414", "415", "416", "417", "418", "419", "423", "424", "425", "430", "432", "434", "435", "438", "440", "441", "442", "443", "450", "456", "458", "469", "470", "473", "475", "478", "479", "480", "484", "500", "501", "502", "503", "504", "505", "506", "507", "508", "509", "510", "512", "513", "514", "515", "516", "517", "518", "519", "520", "530", "533", "534", "539", "540", "541", "551", "559", "561", "562", "563", "567", "570", "571", "573", "574", "575", "579", "580", "581", "585", "586", "587", "600", "601", "602", "603", "604", "605", "606", "607", "608", "609", "610", "612", "613", "614", "615", "616", "617", "618", "619", "620", "623", "626", "630", "631", "636", "641", "646", "647", "649", "650", "651", "657", "660", "661", "662", "664", "670", "671", "678", "681", "682", "684", "700", "701", "702", "703", "704", "705", "706", "707", "708", "709", "710", "712", "713", "714", "715", "716", "717", "718", "719", "720", "724", "727", "731", "732", "734", "740", "747", "754", "757", "758", "760", "762", "763", "765", "767", "769", "770", "772", "773", "774", "775", "778", "779", "780", "781", "784", "785", "786", "787", "800", "801", "802", "803", "804", "805", "806", "807", "808", "809", "810", "812", "813", "814", "815", "816", "817", "818", "819", "828", "829", "830", "831", "832", "843", "845", "847", "848", "849", "850", "855", "856", "857", "858", "859", "860", "862", "863", "864", "865", "866", "867", "868", "869", "870", "872", "876", "877", "878", "888", "900", "901", "902", "903", "904", "905", "906", "907", "908", "909", "910", "912", "913", "914", "915", "916", "917", "918", "919", "920", "925", "928", "929", "931", "936", "937", "938", "939", "940", "941", "947", "949", "951", "952", "954", "956", "970", "971", "972", "973", "978", "979", "980", "985", "989" };
        }

    }
}
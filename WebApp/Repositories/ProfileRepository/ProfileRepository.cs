using System;
using System.Linq;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;
using WebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using DataAccess.CustomModels;

namespace WebApp.Repositories.ProfileRepositories
{
    public class ProfileRepository
    {

        public DoctorProfileInitialValues GetDoctorProfileInitialValues()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getDoctorProfileInitialValues");
                var result = JsonConvert.DeserializeObject<DoctorProfileInitialValues>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public DoctorProfileVM GetDoctorProfileWithAllValues(long doctorID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getDoctorProfileWithAllValues?doctorID=" + doctorID);
                var result = JsonConvert.DeserializeObject<DoctorProfileVM>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }


        public ApiResultModel UpdateDoctorProfileWithAllValues(DoctorProfileVM model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorProfileWithAllValues?doctorID=" + model.DoctorID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }






        public IEnumerable<SecretQuestionVM> GetSecretQuestionList()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getSecretQuestionList");
                var result = JsonConvert.DeserializeObject<IEnumerable<SecretQuestionVM>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public IEnumerable<TimeZoneVM> GetTimeZones()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getTimeZones");
                var result = JsonConvert.DeserializeObject<IEnumerable<TimeZoneVM>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public IEnumerable<CityVM> GetCities()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getCities");
                var result = JsonConvert.DeserializeObject<IEnumerable<CityVM>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public IEnumerable<StateVM> GetStates()
        {
            try
            {
                string emailId = SessionHandler.UserInfo.Email;

                var request = ApiConsumerHelper.GetResponseString("api/getStates");
                var result = JsonConvert.DeserializeObject<IEnumerable<StateVM>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public ApiResultModel UpdateDoctorPicture(UpdateDoctorPicture model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorPicture", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public ApiResultModel ChangePassword(DoctorPasswordModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/changePassword", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel UpdateConsultCharges(UpdateConsultCharges model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateConsultCharges", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel AddUpdateDoctorSecretAnswers(UpdateSecretQuestions model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorSecretAnswers?doctorId=" + model.doctorID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public ApiResultModel UpdateDoctorProfile(long doctorID, UpdateDoctorProfileModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorProfile?doctorID=" + doctorID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public DoctorProfileModel GetDoctorProfile(long doctorID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getDoctorProfile?doctorID=" + doctorID);
                var result = JsonConvert.DeserializeObject<DoctorProfileModel>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public DoctorProfileModel ViewDoctorProfile(long doctorID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/viewDoctorProfile?doctorID=" + doctorID);
                var result = JsonConvert.DeserializeObject<DoctorProfileModel>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public SP_ViewPatientProfile_Result ViewPatientProfile(long patientID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/viewPatientProfile?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<SP_ViewPatientProfile_Result>>(request).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }


        public ApiResultModel UpdatePatientProfile(long patientID, PatientProfileModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updatePatientProfile?patientID=" + patientID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public PatientProfileModel GetPatientProfile(long patientID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getPatientProfile?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<PatientProfileModel>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }


        public ApiResultModel UpdatePatientPicture(UpdatePatientPicture model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updatePatientPicture", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel UpdatePatientLanguages(long patlangID, PatientLanguages model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updatePatientLanguages", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel InsertPatientLanguages(PatientLanguages model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/insertPatientLanguages", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public void DeletePatientLanguages(long langID)
        {
            var request = ApiConsumerHelper.PostData("api/deletePatientLanguages?langID=" + langID, "");

            //var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            //return result;
        }

        public ApiResultModel UpdateDoctorLanguages(long doclangID, DoctorLanguages model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorLanguages?doclangID=" + doclangID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel InsertDoctorLanguages(DoctorLanguages model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/insertDoctorLanguages", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public void DeleteDoctorLanguages(long langID)
        {
            var request = ApiConsumerHelper.PostData("api/deleteDoctorLanguages?langID=" + langID, "");

            //var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            //return result;
        }

        public ApiResultModel InsertDoctorLicensedStates(DoctorLicStatesModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/insertDoctorLicensedStates", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel UpdateDoctorLicensedStates(long licstateID, DoctorLicStatesModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updateDoctorLicensedStates?licstateID=" + licstateID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public void DeleteDoctorLicensedStates(long lsID)
        {
            var request = ApiConsumerHelper.PostData("api/deleteDoctorLicensedStates?lsID=" + lsID, "");

            //var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            //return result;
        }

    }

}

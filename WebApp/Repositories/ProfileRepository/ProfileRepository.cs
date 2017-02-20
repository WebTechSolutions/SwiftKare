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
        #region Doctor Profile Methods

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
        public TimeZoneModel GetDoctorTimeZone(string userid)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getDoctorTimezone?userid=" + userid);
                var result = JsonConvert.DeserializeObject<TimeZoneModel>(request);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
        public TimeZoneModel GetPatientTimeZone(string userid)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getPatientTimezone?userid=" + userid);
                var result = JsonConvert.DeserializeObject<TimeZoneModel>(request);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        #endregion

        #region Patient Profile Methods

        public PatientProfileInitialValues GetPatientProfileInitialValues()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getPatientProfileInitialValues");
                var result = JsonConvert.DeserializeObject<PatientProfileInitialValues>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public PatientProfileVM GetPatientProfileWithAllValues(long patientID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getPatientProfileWithAllValues?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<PatientProfileVM>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public PatientProfileWithExtraInfoVM GetPatientProfileViewOnly(long patientID)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getPatientProfileViewOnly?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<PatientProfileWithExtraInfoVM>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }


        public ApiResultModel UpdatePatientProfileWithAllValues(PatientProfileVM model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updatePatientProfileWithAllValues?patientID=" + model.PatientID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel ChangePatientPassword(DoctorPasswordModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/changePatientPassword", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel updatePatientSecretAnswers(long patientId, UpdateSecretQuestions model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/updatePatientSecretAnswers?patientId=" + patientId, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel UpdateTimezone(TimezoneModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/UpdateTimezone", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }
        #endregion

    }
}
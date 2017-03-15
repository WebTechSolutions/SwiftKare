using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class ConsultationRepository
    {
        public ConsultationModel GetConsultationDetail(long cID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getConsultationDetails?consultID=" + cID);
                var result = JsonConvert.DeserializeObject<ConsultationModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public List<ChatLogModel> GetChat(long cID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getConsultationChat?consultID=" + cID);
                var result = JsonConvert.DeserializeObject<List<ChatLogModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<SP_GetPatientConsultations_Result> GetPatientConsultations(long pID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getPatientConsultations?patientID=" + pID);
                var result = JsonConvert.DeserializeObject<List<SP_GetPatientConsultations_Result>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<DoctorReviewodel> GetDoctorReviews(long dID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getDoctorReviews?doctorID=" + dID);
                var result = JsonConvert.DeserializeObject<List<DoctorReviewodel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<SP_GetPatientAllConsultations_Result> GetAllConsultations(long pID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getPatientAllConsultations?patientID=" + pID);
                var result = JsonConvert.DeserializeObject<List<SP_GetPatientAllConsultations_Result>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        
        public ApiResultModel WriteReview(AddConsultReviewodel model)
        {

            try
            {

                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/addConsultReview", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public ApiResultModel WaiveBilling(long consultID)
        {

            try
            {

                var response = ApiConsumerHelper.PostData("api/WaiveBillingRequest/?consultID=" + consultID, "");
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public AddConsultReviewodel GetConsultationReview(long consultID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getConsultationReview?consultID=" + consultID);
                var result = JsonConvert.DeserializeObject<AddConsultReviewodel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public ApiResultModel CompleteConsult(CompleteConsultPatient model)
        {

            try
            {

                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/CompleteConsultByPatient", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public ApiResultModel CreateConsult(CreateConsultModel model)
        {

            try
            {

                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/createConsult", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
    }
}
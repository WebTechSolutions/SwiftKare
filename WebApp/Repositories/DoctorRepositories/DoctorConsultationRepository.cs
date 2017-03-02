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

namespace WebApp.Repositories.DoctorRepositories
{
    public class DoctorConsultationRepository
    {
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
        
        public List<SP_GetDcotorConsultations_Result> GetDoctorConsultations(long dID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getDcotorConsultations?doctorID=" + dID);
                var result = JsonConvert.DeserializeObject<List<SP_GetDcotorConsultations_Result>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public List<SP_GetDcotorAllConsultations_Result> GetDoctorAllConsultations(long dID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getDcotorAllConsultations?doctorID=" + dID);
                var result = JsonConvert.DeserializeObject<List<SP_GetDcotorAllConsultations_Result>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public ApiResultModel CompleteConsult(CompleteConsultDoctor model)
        {

            try
            {

                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/CompleteConsultByDoctor", strContent);
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
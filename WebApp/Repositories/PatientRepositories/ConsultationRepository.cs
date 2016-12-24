﻿using DataAccess;
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

       
    }
}
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
    public class LifeStyleRepository
    {

        public List<GetLifeStyleQuestions> GetLifeStyle()
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getLifeStyleQuestions");
                var result = JsonConvert.DeserializeObject<List<GetLifeStyleQuestions>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

      
        public List<GetPatientLifeStyle> GetPatientLifeStyle(long patientID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getPatientLifeStyle/?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<GetPatientLifeStyle>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public ApiResultModel AddPatientLifeStyle(PatientLifeStyle_Custom model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/addPatientLifeStyle", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public ApiResultModel UpdatePatientLifeStyle(PatientLifeStyleModel model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/editPatientLifeStyle", strContent);
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
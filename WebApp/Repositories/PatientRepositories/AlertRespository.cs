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
    public class AlertRespository
    {
       
        public List<AlertModel> LoadPatientAlerts(long pid)
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/getPatientAlerts/?patientID=" + pid);
                var result = JsonConvert.DeserializeObject<List<AlertModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }
            
        }
        public ApiResultModel DeleteAlert(DeleteAlertModel model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/deletePatientAlert", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public ApiResultModel ReadAllAlerts(long id)
        {
            try
            {

                var response = ApiConsumerHelper.PostData("api/readAllPatientAlerts/?patientID=" + id, "");
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
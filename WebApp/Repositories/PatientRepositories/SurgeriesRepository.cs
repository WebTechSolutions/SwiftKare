using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class SurgeriesRepository
    {
        public List<SurgeriesModel> AutocompleteSurgery(string prefix)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getAutoCompleteSurgeries/?search="+prefix);
            var result = JsonConvert.DeserializeObject<List<SurgeriesModel>>(response);
            return result;
        }
        public List<SurgeriesModel> GetSurgeries()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getSurgeries");
            var result = JsonConvert.DeserializeObject<List<SurgeriesModel>>(response);
            return result;
        }

        public List<PSurgeries> LoadPatientSurgeries(long patientID)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getPatienSurgeries/?patientID=" + patientID);
            var result = JsonConvert.DeserializeObject<List<PSurgeries>>(response);
            return result;
        }

        public ApiResultModel AddPatientSurgery(PatientSurgery_Custom condition)
        {
           
            try
            {
                var strContent = JsonConvert.SerializeObject(condition);
                var response = ApiConsumerHelper.PostData("api/addPatientSurgery", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ApiResultModel EditPatientSurgery(long id, PatientSurgery_Custom condition)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(condition);
                var response = ApiConsumerHelper.PostData("api/editPatientSurgery?surgeryID=" + id, strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
       
        public ApiResultModel DeletePatientSurgery(long id)
        {
            try
            {
                var response = ApiConsumerHelper.PostData("api/deletePatientSurgery/?surgeryID=" + id, "");
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
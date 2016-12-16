using DataAccess.CustomModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class ConditionRepository
    {
        public ApiResultModel AddCondition(PatientConditions_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/addPatientCondition", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
            
        }
        public ApiResultModel EditCondition(long conditionID, PatientConditions_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/editPatientCondition?conditionID="+conditionID, strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel DeleteCondition(long id)
        {
           
            var response = ApiConsumerHelper.PostData("api/deletePatientCondition/?conditionId=" + id,"");
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }

        public List<GetPatientConditions> LoadHealthConditions(long pid)
        {
           
            var response = ApiConsumerHelper.GetResponseString("api/getPatienConditions/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<GetPatientConditions>>(response);
            return result;
        }
    }
}
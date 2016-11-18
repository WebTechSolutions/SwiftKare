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
        public long AddCondition(PatientConditions_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/addPatientCondition", strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
        public long EditCondition(long id,PatientConditions_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PutData("api/editPatientCondition/?Id="+id, strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
        public long DeleteCondition(long id)
        {
           
            var response = ApiConsumerHelper.DeleteData("api/deletePatientCondition/?conditionId=" + id);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }

        public List<PatientConditions_Custom> LoadHealthConditions(long pid)
        {
           
            var response = ApiConsumerHelper.GetResponseString("api/getPatienConditions/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<PatientConditions_Custom>>(response);
            return result;
        }
    }
}
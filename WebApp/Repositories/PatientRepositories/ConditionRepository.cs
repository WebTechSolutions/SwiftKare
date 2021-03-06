﻿using DataAccess.CustomModels;
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
            try
            {
                var strContent = JsonConvert.SerializeObject(condition);
                var response = ApiConsumerHelper.PostData("api/addPatientCondition", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            
        }
        public ApiResultModel EditCondition(long conditionID, PatientConditions_Custom condition)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(condition);
                var response = ApiConsumerHelper.PostData("api/editPatientCondition?conditionID=" + conditionID, strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
           
             catch (Exception ex)
            {
                throw ex;
            }
        }
        public ApiResultModel DeleteCondition(long id)
        {
            try
            {

            var response = ApiConsumerHelper.PostData("api/deletePatientCondition/?conditionId=" + id, "");
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
         catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<GetPatientConditions> LoadHealthConditions(long pid)
        {
           
            var response = ApiConsumerHelper.GetResponseString("api/getPatienConditions/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<GetPatientConditions>>(response);
            return result;
        }
    }
}
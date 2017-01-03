using DataAccess;
using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class AllergiesRepository
    {
        public List<SensitivityModel> GetSensitivities()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getSensitivity");
            var result = JsonConvert.DeserializeObject<List<SensitivityModel>>(response);
            return result;
        }

        public List<ReactionModel> GetReactions()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getReaction");
            var result = JsonConvert.DeserializeObject<List<ReactionModel>>(response);
            return result;
        }
        public List<AllergiesModel> GetAllergies(string prefix)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getAllergy/?search="+prefix);
            var result = JsonConvert.DeserializeObject<List<AllergiesModel>>(response);
            return result;
        }

        public List<GetPatientAllergies> LoadPatientAllergies(long pid)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getPatientAllergies/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<GetPatientAllergies>>(response);
            return result;
        }
        public ApiResultModel AddPatientAllergy(PatientAllergies_Custom allergy)
        {
            var strContent = JsonConvert.SerializeObject(allergy);
            var response = ApiConsumerHelper.PostData("api/addPatientAllergy", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel EditPatientAllergy(long allergiesID,PatientAllergies_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/editPatientAllergy?allergyID=" + allergiesID, strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel DeletePatientAllergy(long id)
        {

            var response = ApiConsumerHelper.PostData("api/deletePatientAllergy/?allergyID=" + id,"");
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
    }
}
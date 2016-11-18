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
        public List<AllergiesModel> GetAllergies()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getAllergy");
            var result = JsonConvert.DeserializeObject<List<AllergiesModel>>(response);
            return result;
        }

        public List<PatientAllergies_Custom> LoadPatientAllergies(long pid)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getPatientAllergies/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<PatientAllergies_Custom>>(response);
            return result;
        }
        public long AddPatientAllergy(PatientAllergies_Custom allergy)
        {
            var strContent = JsonConvert.SerializeObject(allergy);
            var response = ApiConsumerHelper.PostData("api/addPatientAllergy", strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
        public long EditPatientAllergy(long allergiesID,PatientAllergies_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PutData("api/editPatientAllergy/?allergyID=" + allergiesID, strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
        public long DeletePatientAllergy(long id)
        {

            var response = ApiConsumerHelper.DeleteData("api/deletePatientAllergy/?allergyID=" + id);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
    }
}
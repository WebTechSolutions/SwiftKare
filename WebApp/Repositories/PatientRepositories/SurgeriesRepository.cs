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
        public List<Surgeries> GetSystems()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getSurgeries");
            var result = JsonConvert.DeserializeObject<List<Surgeries>>(response);
            return result;
        }

        public List<GetPatientSurgeries> LoadPatientSurgeries(long patientID)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getPatienSurgeries/?patientID=" + patientID);
            var result = JsonConvert.DeserializeObject<List<GetPatientSurgeries>>(response);
            return result;
        }

        public long AddPatientSurgery(PatientSurgery_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/addPatientSurgery", strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }

        public long EditPatientSurgery(long id, PatientSurgery_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PutData("api/editPatientSurgery/?surgeryID" + id, strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
       
        public long DeletePatientSurgery(long id)
        {

            var response = ApiConsumerHelper.DeleteData("api/deletePatientSurgery/?surgeryID=" + id);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
    }
}
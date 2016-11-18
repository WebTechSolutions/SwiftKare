using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class MedicationRepository
    {
       

        public List<MedicineModel> GetMedicines()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getMedicine");
            var result = JsonConvert.DeserializeObject<List<MedicineModel>>(response);
            return result;
        }
      
        public List<GetMedication> LoadMedications(long pid)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getPatienMedications/?patientID=" + pid);
            var result = JsonConvert.DeserializeObject<List<GetMedication>>(response);
            return result;
        }
        public ApiResultModel AddMedication(PatientMedication_Custom condition)
        {
            var strContent = JsonConvert.SerializeObject(condition);
            var response = ApiConsumerHelper.PostData("api/addPatientMedication", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel EditMedication(long id,PatientMedication_Custom medication)
        {
            var strContent = JsonConvert.SerializeObject(medication);
            var response = ApiConsumerHelper.PostData("api/editPatientMedication?medicationID=" + id, strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel DeleteMedication(long id)
        {

            var response = ApiConsumerHelper.DeleteData("api/deletePatientMedication/?medicationID=" + id);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
    }
}
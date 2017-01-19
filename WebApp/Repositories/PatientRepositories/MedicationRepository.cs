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

        
             public List<Frequency> GetFrequency()
        {

            var response = ApiConsumerHelper.GetResponseString("api/getFrequency");
            var result = JsonConvert.DeserializeObject<List<Frequency>>(response);
            return result;
        }
        public List<MedicineModel> GetMedicines(string search)
        {

            var response = ApiConsumerHelper.GetResponseString("api/getMedicines/?search="+search);
            var result = JsonConvert.DeserializeObject<List<MedicineModel>>(response);
            return result;
        }
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
            try
            {
                var strContent = JsonConvert.SerializeObject(condition);
                var response = ApiConsumerHelper.PostData("api/addPatientMedication", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public ApiResultModel EditMedication(long id,PatientMedication_Custom medication)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(medication);
                var response = ApiConsumerHelper.PostData("api/editPatientMedication?medicationID=" + id, strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
        public ApiResultModel DeleteMedication(long id)
        {
            try
            {
                var response = ApiConsumerHelper.PostData("api/deletePatientMedication/?medicationID=" + id, "");
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
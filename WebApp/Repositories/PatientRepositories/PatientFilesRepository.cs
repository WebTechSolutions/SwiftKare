using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;
using DataAccess.CustomModels;

namespace WebApp.Repositories.PatientRepositories
{
    public class PatientFilesRepository
    {
        public IEnumerable<GetPatientUserFiles> GetPatientFiles(long patientID)
        {
            var response = ApiConsumerHelper.GetResponseString("api/getPatientFiles?patientID=" + patientID);
            var result = JsonConvert.DeserializeObject<IEnumerable<GetPatientUserFiles>>(response);
            return result;
        }

        public GetPatientUserFiles GetPatientFile(long patientID, long fileId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/getPatientFile?patientID=" + patientID + "&fileId=" + fileId);
            var result = JsonConvert.DeserializeObject<GetPatientUserFiles>(response);
            return result;
        }

        public ApiResultModel AddPatientFiles(FilesCustomModel oModel)
        {
            var strContent = JsonConvert.SerializeObject(oModel);
            var request = ApiConsumerHelper.PostData("api/addPatientFile", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel EditPatientFile(long fileID, EditFilesModel oModel)
        {
            var strContent = JsonConvert.SerializeObject(oModel);
            var request = ApiConsumerHelper.PostData("api/editPatientFile?fileID" + fileID, strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel DeletePatientFile(long fileID)
        {
            var response = ApiConsumerHelper.DeleteData("api/deletePatientFile?fileID=" + fileID);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }

    }
}

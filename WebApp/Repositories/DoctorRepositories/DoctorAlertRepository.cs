using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Helper;

namespace WebApp.Repositories.DoctorRepositories
{
    public class DoctorAlertRepository
    {
        public List<AlertModel> LoadDoctorAlerts(long docid)
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/getDoctorAlerts/?doctorID=" + docid);
                var result = JsonConvert.DeserializeObject<List<AlertModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public ApiResultModel DeleteAlert(DeleteAlertModel model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/deleteDoctorAlert", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public ApiResultModel ReadAllAlerts(long id)
        {
            try
            {

                var response = ApiConsumerHelper.PostData("api/readAllDoctorAlerts/?doctorID=" + id, "");
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
    }
}
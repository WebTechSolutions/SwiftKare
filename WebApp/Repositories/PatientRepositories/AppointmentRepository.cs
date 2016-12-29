using DataAccess.CustomModels;
using DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Helper;
using DataAccess.CommonModels;

namespace WebApp.Repositories.PatientRepositories
{
    public class AppointmentRepository
    {
        
        public ApiResultModel RescheduleApp(RescheduleAppointmentModel model)
        {

            try
            {

                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/rescheduleAppointment", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
       
        public List<RescheduleAppModel> GetRescheduleApp(long patientID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/GetRescheduleAppforPatient?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<RescheduleAppModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<ReschedulePendingAppModel> GetPendingApp(long patientID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/GetPendingAppforPatient?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<ReschedulePendingAppModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<RescheduleAppModel> GetUpcomingApp(long pateintID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/GetUpcomingAppforPatient?patientID=" + pateintID);
                var result = JsonConvert.DeserializeObject<List<RescheduleAppModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public GetAppDetail GetAppDetail(long appID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/GetAppDetail?appID=" + appID);
                var result = JsonConvert.DeserializeObject<GetAppDetail>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
    }
}
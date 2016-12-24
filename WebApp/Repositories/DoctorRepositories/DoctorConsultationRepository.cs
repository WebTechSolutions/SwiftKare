using DataAccess;
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
    public class DoctorConsultationRepository
    {
        public ConsultationModel GetConsultationDetail(long cID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getConsultationDetails?consultID=" + cID);
                var result = JsonConvert.DeserializeObject<ConsultationModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<SP_GetDcotorConsultations_Result> GetDoctorConsultations(long dID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getDcotorConsultations?doctorID=" + dID);
                var result = JsonConvert.DeserializeObject<List<SP_GetDcotorConsultations_Result>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
    }
}
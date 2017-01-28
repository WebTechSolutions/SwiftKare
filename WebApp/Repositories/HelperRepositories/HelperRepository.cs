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
    public class HelperRepository
    {
        /// <summary>
        /// Charges client
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        public void PerformStripeCharge(string tokenId, int amount)
        {
            try
            {
                var response = ApiConsumerHelper.PostData("api/performStripeCharge?tokenId=" + tokenId + "&amount=" + amount, "");
                var result = JsonConvert.DeserializeObject<string>(response);
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Generates and returns open tok session
        /// </summary>
        /// <returns></returns>
        public string GenerateOpenTokSession()
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/generateOpenTokSessionWeb");
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Generates and returns token
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GenerateOpenTokToken(string sessionId)
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/generateOpenTokTokenWeb?sessionId=" + sessionId);
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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

    }
}
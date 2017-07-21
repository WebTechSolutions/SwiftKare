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
    public class DoseSpotRepository
    {

        /// <summary>
        /// Gets Pharmacy Search
        /// </summary>
        /// <param name="oSrch"></param>
        /// <returns></returns>
        public List<PharmacyEntry> GetPharmacySearchResult(DoseSpotPharmacySearch oSrch)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(oSrch);
                var response = ApiConsumerHelper.PostData("api/SearchPharmacy", strContent);
                var result = JsonConvert.DeserializeObject<List<PharmacyEntry>>(response);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetRefillErr()
        {
            try
            {
              //  var strContent = JsonConvert.SerializeObject(oSrch);
                var response = ApiConsumerHelper.GetResponseString("api/GetRefillErr");
                // var result = JsonConvert.DeserializeObject<List<PharmacyEntry>>(response);
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Get Patient DoseSpot Url
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public string GetPatientDoseSpotUrl(long patientId)
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetPatientDoseSpotUrl?patientId=" + patientId);
               // var result = JsonConvert.DeserializeObject<string>(response);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
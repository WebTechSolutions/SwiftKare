using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Helper;

namespace WebApp.Repositories.PatientRepositories
{
    public class MyHealthRepository
    {
        public List<FamilyHX> GetFamilyHX()
        {

            try
            {
               
                var response = ApiConsumerHelper.GetResponseString("api/getFamilyHXItems");
                var result = JsonConvert.DeserializeObject<List<FamilyHX>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public List<RelationshipModel> GetRelationships()
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getRelationships");
                var result = JsonConvert.DeserializeObject<List<RelationshipModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<GetPatientFamilyHX> GetPatientFamilyHX(long patientID)
        {

            try
            {

                var response = ApiConsumerHelper.GetResponseString("api/getPatientFamilyHXItems/?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<GetPatientFamilyHX>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }

        public ApiResultModel AddFamilyHX(PatientFamilyHX_Custom model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/addPatientFamilyHX", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
          
        }
        public ApiResultModel UpdateFamilyHX(UpdateFamilyHX model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/updatePatientFamilyHX", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public ApiResultModel DeleteFamilyHX(long id)
        {
            try
            {
                var response = ApiConsumerHelper.PostData("api/deletePatientFamilyHX/?fhxID=" + id,"");
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
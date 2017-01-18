using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApp.Helper;
using WebApp.Models;

namespace WebApp.Repositories.DoctorRepositories
{
    public class SeeDoctorRepository
    {
        public List<DoctorDataset> MyCareTeam(long patientID)
        {


            try
            {
                
                var response = ApiConsumerHelper.GetResponseString("api/getMyFavDoctors/?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<DoctorDataset>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public List<DoctorDataset> SeeDoctor(SearchDoctorModel searchModel)
            {

               
                try
                {
                    var strContent = JsonConvert.SerializeObject(searchModel);
                    var response = ApiConsumerHelper.PostData("api/searchDoctor/?searchModel", strContent);
                    var result = JsonConvert.DeserializeObject<List<DoctorDataset>>(response);
                    return result;
                }
                catch (HttpResponseException ex)
                {
                    throw ex;
                 }

            }
        public SearchDoctorResult SeeDoctorWithShift(SearchDoctorModel searchModel)
        {


            try
            {
                var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.PostData("api/searchDoctorwithFav/?searchModel", strContent);
                var result = JsonConvert.DeserializeObject<SearchDoctorResult>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public List<FetchDoctorTimingModel> FetchDoctorTimes(FetchTimingsModel searchModel)
        {

            try
            {
                var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.PostData("api/fetchDoctorTime/?searchModel", strContent);
                var result = JsonConvert.DeserializeObject<List<FetchDoctorTimingModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }

        }
        public GetDoctorINFOVM GetDoctorInfo(long doctorID)
        {

            try
            {
                //var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.GetResponseString("api/getDoctorInfo/?doctorID=" + doctorID);
                var result = JsonConvert.DeserializeObject<GetDoctorINFOVM>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
      
        public PatientROV LoadROV(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/PatientPreviousROV/?patientID=" + patientid);
                var result = JsonConvert.DeserializeObject<PatientROV>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public PatientROV GetPatientChiefComplaints(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetPatientChiefComplaints/?Id=" + patientid);
                var result = JsonConvert.DeserializeObject<PatientROV>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public List<ROV_Custom> LoadROVList()
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetROVs");
                var result = JsonConvert.DeserializeObject<List<ROV_Custom>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public List<FavouriteDoctorModel> LoadFavDoctors(long patientID)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetFavouriteDoctors/?patientID=" + patientID);
                var result = JsonConvert.DeserializeObject<List<FavouriteDoctorModel>>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }


        }
        public string LoadMedications(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/Speciallities");
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }


        }
        public string LoadAllergies(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/Speciallities");
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }


        }
        public string LoadSurgeries(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/Speciallities");
                var result = JsonConvert.DeserializeObject<string>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }


        }
        public ApiResultModel AddAppointment(AppointmentModel model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/addAppointment", strContent);
                var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
                return result;
            }
            
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }

        public ApiResultModel AddFavourite(FavouriteDoctorModel model)
        {
            try { 
            var strContent = JsonConvert.SerializeObject(model);
            var response = ApiConsumerHelper.PostData("api/favouriteDoctor", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public ApiResultModel UpdateFavourite(FavouriteDoctorModel model)
        {
            try
            {
                var strContent = JsonConvert.SerializeObject(model);
                var response = ApiConsumerHelper.PostData("api/unfavouriteDoctor", strContent);
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

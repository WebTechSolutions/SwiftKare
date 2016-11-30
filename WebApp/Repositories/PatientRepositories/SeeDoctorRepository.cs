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
            public List<DoctorModel> SeeDoctor(SearchDoctorModel searchModel)
            {

               
                try
                {
                    var strContent = JsonConvert.SerializeObject(searchModel);
                    var response = ApiConsumerHelper.PostData("api/searchDoctor/?searchModel", strContent);
                    var result = JsonConvert.DeserializeObject<List<DoctorModel>>(response);
                    return result;
                }
                catch (Exception ex)
                {
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    httpResponseMessage.Content = new StringContent(ex.Message);
                    throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }

        }
        public List<DoctorInfoCustom> GetDoctorInfo(long doctorID)
        {

            try
            {
                //var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.GetResponseString("api/getDoctorInfo/?doctorID=" + doctorID);
                var result = JsonConvert.DeserializeObject<List<DoctorInfoCustom>>(response);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }

        }
        public List<FavouriteDoctorModel> LoadFavDoctors(long patientID)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetFavouriteDoctors");
                var result = JsonConvert.DeserializeObject<List<FavouriteDoctorModel>>(response);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }

        }
        public ApiResultModel AddAppointment(AppointmentModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var response = ApiConsumerHelper.PostData("api/addAppointment", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }

        public ApiResultModel AddFavourite(FavouriteDoctorModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var response = ApiConsumerHelper.PostData("api/favouriteDoctor", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
        public ApiResultModel UpdateFavourite(FavouriteDoctorModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var response = ApiConsumerHelper.PostData("api/unfavouriteDoctor", strContent);
            var result = JsonConvert.DeserializeObject<ApiResultModel>(response);
            return result;
        }
    }
    }

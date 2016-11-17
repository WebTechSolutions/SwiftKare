using DataAccess;
using DataAccess.CommonModels;
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
        public AppointmentModel LoadROV(long patientid)
        {

            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/ROV/?Id="+patientid);
                var result = JsonConvert.DeserializeObject<AppointmentModel>(response);
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

        


        public long AddAppointment(AppointmentModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var response = ApiConsumerHelper.PostData("api/addAppointment/appModel/", strContent);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }
    }
    }

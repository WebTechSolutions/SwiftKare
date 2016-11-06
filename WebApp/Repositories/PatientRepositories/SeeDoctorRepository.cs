using DataAccess;
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
            public IEnumerable SeeDoctor(SearchModel searchModel)
            {

               
                try
                {
                    var strContent = JsonConvert.SerializeObject(searchModel);
                    var response = ApiConsumerHelper.PostData("api/searchDoctor/searchModel/?searchModel", strContent);
                    var result = JsonConvert.DeserializeObject<IEnumerable<object>>(response);
                    return result;
                }
                catch (Exception ex)
                {
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    httpResponseMessage.Content = new StringContent(ex.Message);
                    throw new HttpResponseException(httpResponseMessage);
                }

            }
        public IEnumerable<BookAppointment> FetchDoctorTimes(BookAppointment searchModel)
        {


            try
            {
                var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.PostData("api/fetchDoctorTime/searchModel/?searchModel", strContent);
                var result = JsonConvert.DeserializeObject<IEnumerable<BookAppointment>>(response);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }

        }

        //public SeeDoctorDTO Add(BookAppointment t)
        //{
        //    //By Default Active
        //    //t.Doctor.App = true;
        //    //t.cb ="";
        //    var strContent = JsonConvert.SerializeObject(t);
        //    var response = ApiConsumerHelper.PostData("api/Doctors", strContent);
        //    var result = JsonConvert.DeserializeObject<Doctor>(response);
        //   return result;
        //}
    }
    }

﻿using DataAccess;
using RestAPIs.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Identity.Membership;
using Identity.Membership.Models;
using Microsoft.AspNet.Identity;
using System.Text;
using DataAccess.CommonModels;
using DataAccess.CustomModels;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class SearchDoctorController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;
        // POST: api/searchDoctor/SeeDoctorViewModel
        [Route("api/searchDoctor")]
        public HttpResponseMessage SeeDoctor(SearchDoctorModel searchModel)
        {
          

            try
            {

                var result = db.SP_SearchDoctor(searchModel.language, searchModel.speciality, searchModel.name, searchModel.appDate.DayOfWeek.ToString(),
                    searchModel.appTime, searchModel.gender).ToList();

                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {

               return ThrowError(ex, "SeeDoctor in SeacrhDoctorController.");
            }
         

        }

        [Route("api/fetchDoctorTime")]
        public HttpResponseMessage FetchDoctorTime(FetchTimingsModel searchModel)
        {
            try
            {
                var result = db.SP_FetchDoctorTimings(searchModel.doctorID,searchModel.appDate).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, result);
               return response;
               
            }
            catch (Exception ex)
            {

                return ThrowError(ex, "FetchDoctorTime in SearchDoctorController");
            }


        }
        
       
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. "+ Action+"\n"+ex.ToString(), Encoding.Unicode);
           return response;
        }
    }
}

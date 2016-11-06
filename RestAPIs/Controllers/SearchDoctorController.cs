using DataAccess;
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

namespace RestAPIs.Controllers
{
    public class SearchDoctorController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;
        // POST: api/searchDoctor/SeeDoctorViewModel
        [Route("api/searchDoctor/searchModel/")]
        public HttpResponseMessage SeeDoctor(SearchModel searchModel)
        {
          

            try
            {
               
                TimeSpan timeinterval = new TimeSpan();
                timeinterval.Add(searchModel.docTime);
                if(searchModel.Language!=null && searchModel.Speciallity!=null)
                {
                    var result = (from doctor in db.Doctors
                                  join docLang in db.DoctorLanguages on doctor.doctorID equals docLang.doctorID
                                  join docSpec in db.DoctorSpecialities on doctor.doctorID equals docSpec.doctorID
                                  join docTimings in db.DoctorTimings on doctor.doctorID equals docTimings.doctorID
                                  where (searchModel.Language != null || searchModel.Language != "" && docLang.languageName == searchModel.Language)
                                && (docSpec.specialityName == searchModel.Speciallity)
                                && docTimings.day == searchModel.AppDate.DayOfWeek.ToString()
                                && (
                                     (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.firstName == searchModel.DoctorName)
                                     || (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.lastName == searchModel.DoctorName)
                                   )
                                   && docTimings.@from >= timeinterval &&
                                  docTimings.to >= timeinterval
                                  && (searchModel.Gender != null || searchModel.Gender != "" && doctor.gender == searchModel.Gender)
                                  select new { doctorID = doctor.doctorID, firstName = doctor.firstName, lastName = doctor.lastName }).ToList();
                    if (!result.Any())
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "No Record Found");
                        return response;
                    }
                    else
                    {
                        //IEnumerable eventt_grp = result.Select(r => new { r.doctorID, r.firstName, r.lastName });
                         response = Request.CreateResponse(HttpStatusCode.OK, result);
                        return response;
                    }
                }
                if (searchModel.Language == null && searchModel.Speciallity == null)
                {
                    var result = (from doctor in db.Doctors
                                  //join docLang in db.DoctorLanguages on doctor.doctorID equals docLang.doctorID
                                  //join docSpec in db.DoctorSpecialities on doctor.doctorID equals docSpec.doctorID
                                  join docTimings in db.DoctorTimings on doctor.doctorID equals docTimings.doctorID
                                  where docTimings.day == searchModel.AppDate.DayOfWeek.ToString()
                                && (
                                     (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.firstName == searchModel.DoctorName)
                                     || (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.lastName == searchModel.DoctorName)
                                   )
                                   //&& docTimings.@from >= timeinterval && docTimings.to >= timeinterval
                                  && (searchModel.Gender != null || searchModel.Gender != "" && doctor.gender == searchModel.Gender)
                                  select new { doctorID = doctor.doctorID, firstName = doctor.firstName, lastName = doctor.lastName }).ToList();
                    if (!result.Any())
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "No Record Found");
                        return response;
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, result);
                        return response;
                    }
                }
                if (searchModel.Language != null && searchModel.Speciallity == null)
                {
                    var result = (from doctor in db.Doctors
                                  join docLang in db.DoctorLanguages on doctor.doctorID equals docLang.doctorID
                                  join docTimings in db.DoctorTimings on doctor.doctorID equals docTimings.doctorID
                                  where (searchModel.Language != null || searchModel.Language != "" && docLang.languageName == searchModel.Language)
                                  && docTimings.day == searchModel.AppDate.DayOfWeek.ToString()
                                && (
                                     (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.firstName == searchModel.DoctorName)
                                     || (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.lastName == searchModel.DoctorName)
                                   )
                                   && docTimings.@from >= timeinterval &&
                                  docTimings.to >= timeinterval
                                  && (searchModel.Gender != null || searchModel.Gender != "" && doctor.gender == searchModel.Gender)
                                  select new { doctorID = doctor.doctorID, firstName = doctor.firstName, lastName = doctor.lastName }).ToList();
                    if (!result.Any())
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "No Record Found");
                        return response;
                    }
                    else
                    {
                        //IEnumerable eventt_grp = result.Select(r => new { r.doctorID, r.firstName, r.lastName });
                        response = Request.CreateResponse(HttpStatusCode.OK, result);
                        return response;
                    }
                }
                if (searchModel.Language == null && searchModel.Speciallity != null)
                {
                    var result = (from doctor in db.Doctors
                                  join docSpec in db.DoctorSpecialities on doctor.doctorID equals docSpec.doctorID
                                  join docTimings in db.DoctorTimings on doctor.doctorID equals docTimings.doctorID
                                  where (docSpec.specialityName == searchModel.Speciallity)
                                  && docTimings.day == searchModel.AppDate.DayOfWeek.ToString()
                                && (
                                     (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.firstName == searchModel.DoctorName)
                                     || (searchModel.DoctorName != null || searchModel.DoctorName != "" && doctor.lastName == searchModel.DoctorName)
                                   )
                                   && docTimings.@from >= timeinterval &&
                                  docTimings.to >= timeinterval
                                  && (searchModel.Gender != null || searchModel.Gender != "" && doctor.gender == searchModel.Gender)
                                  select new { doctorID = doctor.doctorID, firstName = doctor.firstName, lastName = doctor.lastName }).ToList();
                    if (!result.Any())
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "No Record Found");
                        return response;
                    }
                    else
                    {
                        //var  docList = from d in result
                        //          select new {docID=d.doctorID,firstName=d.firstName,lastName=d.lastName };
                        response = Request.CreateResponse(HttpStatusCode.OK, result);
                        return response;
                    }
                }

                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Creiteria for search");
                return response;
            }
            catch (Exception ex)
            {

                //HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                //httpResponseMessage.Content = new StringContent(ex.Message);
                //throw new HttpResponseException(httpResponseMessage);
               return ThrowError(ex, "SeeDoctor in SeacrhDoctorController.");
            }
         

        }

        [Route("api/fetchDoctorTime/searchModel/")]
        public HttpResponseMessage fetchDoctorTime(BookAppointment searchModel)
        {
            try
            {
              
                var result = from docTime in db.DoctorTimings
                           join f in db.Appointments on docTime.doctorID equals f.doctorID into app
                           from docapp in app.Where(f => f.appDate == searchModel.appDate && f.active==true).DefaultIfEmpty()
                           where docTime.doctorID == searchModel.doctorID && docTime.active == true 
                           select new BookAppointment { doctorID = (long)docTime.doctorID, fromTime = (TimeSpan)docTime.@from, toTime = (TimeSpan)docTime.to, appTime = (TimeSpan)docapp.appTime };

                             
                if (!result.Any())
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "No Record Found");
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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

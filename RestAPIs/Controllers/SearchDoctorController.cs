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
using System.Data.Entity;
using System.Globalization;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class SearchDoctorController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;

        [Route("api/getMyFavDoctors")]
        public HttpResponseMessage GetMyCareTeam(long patientID)
        {
            try
            {
                //Doctor.doctorID, Doctor.firstName, Doctor.lastName,Doctor.picture
                var favdoc = (from l in db.FavouriteDoctors
                              where l.patientID == patientID && l.active == true
                              select (from doc in db.Doctors where doc.doctorID == l.doctorID
                                      select new DoctorDataset
                                      {
                                          doctorID = doc.doctorID,
                                          title=doc.title,
                                          firstName = doc.firstName,
                                          lastName = doc.lastName,
                                          ProfilePhotoBase64 = "",
                                          city=doc.city,
                                          state=doc.state,
                                          languageName = db.DoctorLanguages.Where(d => d.doctorID == doc.doctorID).Select(d => d.languageName).FirstOrDefault(),
                                          specialityName = db.DoctorSpecialities.Where(d => d.doctorID == doc.doctorID).Select(d => d.specialityName).FirstOrDefault(),
                                          reviewStar=doc.reviewStar

                                      }).FirstOrDefault()).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, favdoc);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetFavouriteDoctors in SearchDoctorController");
            }


        }
        [Route("api/GetFavouriteDoctors")]
        public HttpResponseMessage GetFavouriteDoctors(long patientID)
        {
            try
            {
                var favdoc = (from l in db.FavouriteDoctors
                                 where l.patientID==patientID && l.active == true
                                 select new FavouriteDoctorModel { docID = l.doctorID, patID = l.patientID }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, favdoc);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetFavouriteDoctors in SearchDoctorController");
            }


        }

        

       
        [Route("api/searchDoctorwithFav")]
        public HttpResponseMessage searchDoctorwithFav(SearchDoctorModel searchModel)
        {
            string timingsFrom = null;
            string timingsTo = null;
            TimeSpan? appFromtimings = null;
            TimeSpan? appTotimings = null;
            if (searchModel.language == "") { searchModel.language = null; }
            if (searchModel.speciality == "") { searchModel.speciality = null; }
            if (searchModel.name == "") { searchModel.name = null; }
            if (searchModel.appDate == "") { searchModel.appDate = null; }
            if (searchModel.appTime == "") { searchModel.appTime = null; }
            if (searchModel.gender == "") { searchModel.gender = null; }

           
                            
            try
            {
                if (searchModel.appTime != null)
                {
                    List<string> timeframe = searchModel.appTime.Split(':').ToList<string>();
                    var j = 0;
                    foreach (var item in timeframe)
                    {
                        if (j == 0)
                        {
                            //timingsFrom = item;
                            if (item.Length == 1)
                            {
                                timingsFrom = "0" + item;
                            }
                            else
                            {
                                timingsFrom = item;
                            }
                            j++;
                            continue;
                        }
                        if (j == 1)
                        {
                            if (item.Length == 1)
                            {
                                timingsTo = "0" + item;
                            }
                            else
                            {
                                timingsTo = item;
                            }
                        }

                    }
                    DateTime dateTimeFrom = DateTime.ParseExact(timingsFrom + ":00",
                                       "HH:mm", CultureInfo.InvariantCulture);
                    DateTime dateTimeTo = DateTime.ParseExact(timingsTo + ":00",
                                        "HH:mm", CultureInfo.InvariantCulture);

                    appFromtimings = dateTimeFrom.TimeOfDay;
                    appTotimings = dateTimeTo.TimeOfDay;

                }
                if (searchModel.appDate == null)
                {

                    var result = (from l in db.SearchDoctorWithShift(searchModel.language, searchModel.speciality, searchModel.name, null,
                    appFromtimings, appTotimings, searchModel.gender)
                                  select new SearchDoctorWithShift_Model {
                                      doctorID =l.doctorID, title=l.title,
                                      firstName=l.firstName,
                                      lastName=l.lastName,
                                      city=l.city,
                                      ProfilePhotoBase64=l.ProfilePhotoBase64,
                                      state=l.state,
                                      languageName=l.languageName,
                                      specialityName=l.specialityName,
                                      reviewStar=l.reviewStar
                                  }
                    ).ToList();
                    var favdoc = (from l in db.FavouriteDoctors
                                  where l.patientID == searchModel.patientID && l.active == true
                                  select new FavouriteDoctorModel { docID = l.doctorID, patID = l.patientID }).ToList();
                    SearchDoctorResult searchResult = new SearchDoctorResult();
                    searchResult.doctor =  result;
                    searchResult.favdoctor = favdoc;

                    response = Request.CreateResponse(HttpStatusCode.OK, searchResult);
                   
                }

                if (searchModel.appDate != null)
                {
                    try
                    {
                       
                        string dateString = searchModel.appDate.Trim();
                        string format = "dd/MM/yyyy";
                        CultureInfo provider = CultureInfo.InvariantCulture;
                   
                        DateTime day = DateTime.ParseExact(dateString, format, provider);
                        var result = (from l in db.SearchDoctorWithShift(searchModel.language, searchModel.speciality, searchModel.name, day.DayOfWeek.ToString(),
                    appFromtimings, appTotimings, searchModel.gender)
                                      select new SearchDoctorWithShift_Model
                                      {
                                          doctorID = l.doctorID,
                                          title = l.title,
                                          firstName = l.firstName,
                                          lastName = l.lastName,
                                          city = l.city,
                                          ProfilePhotoBase64 = "",
                                          state = l.state,
                                          languageName = l.languageName,
                                          specialityName = l.specialityName,
                                          reviewStar = l.reviewStar
                                      }
                    ).ToList();
                        var favdoc = (from l in db.FavouriteDoctors
                                      where l.patientID == searchModel.patientID && l.active == true
                                      select new FavouriteDoctorModel { docID = l.doctorID, patID = l.patientID }).ToList();
                        SearchDoctorResult searchResult = new SearchDoctorResult();
                        searchResult.doctor = result;
                        searchResult.favdoctor = favdoc;

                        response = Request.CreateResponse(HttpStatusCode.OK, searchResult);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("{0} is not in the correct format.", searchModel.appDate.Trim());
                    }
                   
                    

                }
                return response;

            }
            catch (Exception ex)
            {

                return ThrowError(ex, "searchDoctorwithFav in SeacrhDoctorController.");
            }


        }

        //[Route("api/searchDoctorwithFav")]
        //public HttpResponseMessage SeeDoctorwithFav(SearchDoctorModel searchModel)
        //{


        //    try
        //    {
        //        if (searchModel.appDate == null)
        //        {
        //            var result = db.SearchDoctorJSON(searchModel.language, searchModel.speciality, searchModel.name, null,
        //            searchModel.appTime, searchModel.gender).ToList();
        //            var favdoc = (from l in db.FavouriteDoctors
        //                          where l.patientID == searchModel.patientID && l.active == true
        //                          select new FavouriteDoctorModel { docID = l.doctorID,patID=l.patientID }).ToList();
        //            var searchResult=( new
        //            { doctor= result, favdoctor = favdoc
        //            });

        //            response = Request.CreateResponse(HttpStatusCode.OK, searchResult);

        //        }

        //        if (searchModel.appDate != null)
        //        {
        //            DateTime day = new DateTime();
        //            day = Convert.ToDateTime(searchModel.appDate);
        //            var result = db.SearchDoctorJSON(searchModel.language, searchModel.speciality, searchModel.name, day.DayOfWeek.ToString(),
        //            searchModel.appTime, searchModel.gender).ToList();
        //            var favdoc = (from l in db.FavouriteDoctors
        //                          where l.patientID == searchModel.patientID && l.active == true
        //                          select new FavouriteDoctorModel { docID = l.doctorID, patID = l.patientID }).ToList();
        //            var searchResult = (new
        //            {
        //                doctor = result,
        //                favdoctor = favdoc
        //            });

        //            response = Request.CreateResponse(HttpStatusCode.OK, searchResult);
        //        }
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {

        //        return ThrowError(ex, "SeeDoctor in SeacrhDoctorController.");
        //    }


        //}

        [Route("api/fetchDoctorTimeWeb")]
        public HttpResponseMessage FetchDoctorTimeWeb(FetchTimingsModel searchModel)
        {
            try
            {
                //var dateTime = DateTime.Parse(searchModel.appDate);
                string[] formats = { "dd/MM/yyyy" };
                var dateTime = DateTime.ParseExact(searchModel.appDate.Trim(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                string appday = dateTime.ToString("dddd");
                //var result = db.SP_FetchDoctorTimings(searchModel.doctorID, dateTime).ToList();
                var timings = (from t in db.DoctorTimings where t.doctorID == searchModel.doctorID
                              && t.utcDay == appday && t.active==true
                               select new TimingsVM { doctorID = t.doctorID, fromtime = t.@from, totime = t.to }).ToList();
                var appointments = (from app in db.Appointments where app.doctorID == searchModel.doctorID &&
                                    app.appDate == dateTime
                                    select new AppointmentsVM { appTime=app.appTime}).ToList();
                DocTimingsAndAppointment result = new DocTimingsAndAppointment();
                result.timingsVM = timings;
                result.appointmentVM = appointments;
                response = Request.CreateResponse(HttpStatusCode.OK, result);
               
               return response;
               
            }
            catch (Exception ex)
            {

                return ThrowError(ex, "FetchDoctorTimeWeb in SearchDoctorController");
            }


        }
        private List<string> createTimeSlots(DocTimingsAndAppointment appList)
        {
            List<string> timeSlots = new List<string> { };

            foreach (var item in appList.timingsVM)
            {

                TimeSpan startTime = (TimeSpan)item.fromtime;
                TimeSpan endTime = (TimeSpan)item.totime;
                if (startTime.Minutes % 15 != 0)
                {
                    TimeSpan tempp = TimeSpan.FromMinutes(15 - (startTime.Minutes % 15));
                    startTime = startTime.Add(tempp);
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
                        TimeSpan temppp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(temppp);

                    }
                }

                if (endTime.Minutes % 15 != 0)
                {
                    TimeSpan tempp = TimeSpan.FromMinutes(15 - (endTime.Minutes % 15));
                    endTime = endTime.Add(tempp);

                }

                TimeSpan itemstartTime = startTime;//(TimeSpan)item.from;

                if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                {
                    timeSlots.Add(startTime.ToString(@"hh\:mm"));
                    TimeSpan tempp = TimeSpan.FromMinutes(15);
                    startTime = startTime.Add(tempp);

                }
                bool flag = true;
                while (flag)
                {
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        //if (!(TimeSpan.Equals(slot, item.appTime)))
                        //{
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
                        TimeSpan tempp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(tempp);

                        //}
                    }
                    else
                    {
                        TimeSpan tempp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(tempp);

                    }

                    if (TimeSpan.Equals(startTime, endTime))
                    {

                        if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                        {
                            timeSlots.Add(startTime.ToString(@"hh\:mm"));
                            TimeSpan tempp = TimeSpan.FromMinutes(15);
                            startTime = startTime.Add(tempp);

                        }
                        flag = false;
                    }
                    if (startTime.Hours == endTime.Hours)
                    {
                        if (startTime.Minutes > endTime.Minutes)
                        {
                            flag = false;
                        }
                    }


                } //while end 
            }//for loop for database records.


            foreach (var app in appList.appointmentVM)
            {

                if (app.appTime.HasValue)
                {
                    TimeSpan apptime = TimeSpan.Parse(app.appTime.Value.ToString());
                    if (timeSlots.Contains(apptime.ToString(@"hh\:mm")))
                    {
                        timeSlots.Remove(apptime.ToString(@"hh\:mm"));
                    }

                }

            }
            for (var i = 0; i < timeSlots.Count; i++)
            {
                TimeSpan doctimings = TimeSpan.Parse(timeSlots[i]);
                var dateTime = new DateTime(doctimings.Ticks); // Date part is 01-01-0001
                var formattedTime = dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                timeSlots.RemoveAt(i);
                timeSlots.Insert(i, formattedTime);

            }
            if (timeSlots.Count > 0)
            {
                timeSlots.RemoveAt(timeSlots.Count - 1);
            }

            return timeSlots;
        }

        [Route("api/fetchDoctorTime")]
        public HttpResponseMessage FetchDoctorTime(FetchTimingsModel searchModel)
        {
            try
            {
                //var dateTime = DateTime.Parse(searchModel.appDate);
                string[] formats = { "dd/MM/yyyy" };
                var dateTime = DateTime.ParseExact(searchModel.appDate.Trim(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                string appday = dateTime.ToString("dddd");
                var result = db.SP_FetchDoctorTimings(searchModel.doctorID, dateTime).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, result);

                return response;

            }
            catch (Exception ex)
            {

                return ThrowError(ex, "FetchDoctorTime in SearchDoctorController");
            }


        }
        private List<string> displayTimeSlots(IEnumerable<SP_FetchDoctorTimings_Result> appList)
        {
            
            //foreach (var item in appList)
            //{
            //    DateTimeOffset targetTime, sourceTime;
            //    sourceTime = new DateTimeOffset(new DateTime(),(TimeSpan)item.from);
            //    targetTime = sourceTime.ToOffset(new TimeSpan(5, 0, 0));
            //    Console.Write("Target Time "+targetTime+"Source Time "+sourceTime);
            //}
            List<string> timeSlots = new List<string> { };

            foreach (var item in appList)
            {

                TimeSpan startTime = (TimeSpan)item.from;
                if (startTime.Minutes % 15 != 0)
                {
                    TimeSpan tempp = TimeSpan.FromMinutes(15 - (startTime.Minutes % 15));
                    startTime = startTime.Add(tempp);
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
                        TimeSpan temppp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(temppp);

                    }
                }

                TimeSpan itemstartTime = (TimeSpan)item.from;
                TimeSpan endTime = (TimeSpan)item.to;
                if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                {
                    timeSlots.Add(startTime.ToString(@"hh\:mm"));
                    TimeSpan tempp = TimeSpan.FromMinutes(15);
                    startTime = startTime.Add(tempp);

                }
                bool flag = true;
                while (flag)
                {
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        //if (!(TimeSpan.Equals(slot, item.appTime)))
                        //{
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
                        TimeSpan tempp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(tempp);

                        //}
                    }
                    else
                    {
                        TimeSpan tempp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(tempp);

                    }

                    if (TimeSpan.Equals(startTime, endTime))
                    {

                        if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                        {
                            timeSlots.Add(startTime.ToString(@"hh\:mm"));
                            TimeSpan tempp = TimeSpan.FromMinutes(15);
                            startTime = startTime.Add(tempp);

                        }
                        flag = false;
                    }
                    if (startTime.Hours == endTime.Hours)
                    {
                       
                        if (startTime.Minutes > endTime.Minutes)
                        {

                           
                            flag = false;
                        }
                    }
                } //while end 
            }//for loop for database records.



            foreach (var app in appList)
            {
                if (app.appTime.HasValue)
                {
                    TimeSpan apptime = TimeSpan.Parse(app.appTime.Value.ToString());
                    if (timeSlots.Contains(apptime.ToString(@"hh\:mm")))
                    {
                        timeSlots.Remove(apptime.ToString(@"hh\:mm"));
                    }
                }


            }
            for (var i = 0; i < timeSlots.Count; i++)
            {
                TimeSpan doctimings = TimeSpan.Parse(timeSlots[i]);
                var dateTime = new DateTime(doctimings.Ticks); // Date part is 01-01-0001
                var formattedTime = dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                timeSlots.RemoveAt(i);
                timeSlots.Insert(i, formattedTime);
                
            }
            return timeSlots;
        }
        [Route("api/fetchDoctorTimeNew")]
        public HttpResponseMessage FetchDoctorTimeNew(FetchTimingsModel searchModel)
        {
            try
            {
                //var dateTime = DateTime.Parse(searchModel.appDate);
                 string[] formats = { "dd/MM/yyyy" };
                var dateTime = DateTime.ParseExact(searchModel.appDate.Trim(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                string appday = dateTime.ToString("dddd");
                //List<SP_FetchDoctorTimings_Result> appList = new List<SP_FetchDoctorTimings_Result>();
                //appList = db.SP_FetchDoctorTimings(searchModel.doctorID, dateTime).ToList();
                var doctimings = (from t in db.DoctorTimings
                                  where t.doctorID == searchModel.doctorID
                                  && t.utcDay == appday && t.active == true
                                  select new TimingsVM
                                  {
                                      doctorID = t.doctorID,
                                      fromtime = t.@from,
                                      totime = t.to
                                  }).ToList();
                var appointments = (from app in db.Appointments
                                    where app.doctorID == searchModel.doctorID &&
                                    app.appDate == dateTime
                                    select new AppointmentsVM { appTime = app.appTime }).ToList();
                DocTimingsAndAppointment appList = new DocTimingsAndAppointment();
                appList.timingsVM = doctimings;
                appList.appointmentVM = appointments;
                List<string> timings = new List<string>();
                
                if (appList != null)
                {
                    //calculate time slots
                    timings = createTimeSlots(appList);//displayTimeSlots(appList);
                    timings = timings.OrderBy(x => DateTime.ParseExact(x, "hh:mm tt", CultureInfo.InvariantCulture)).ToList();
                    //timings = displayTimeSlots(appList);//displayTimeSlots(appList);
                }
                response = Request.CreateResponse(HttpStatusCode.OK, timings);
                return response;


            }
            catch (Exception ex)
            {

                return ThrowError(ex, "FetchDoctorTimeNew in SearchDoctorController");
            }


        }

        [Route("api/getDoctorInfo")]
        public HttpResponseMessage GetDoctorInfo(long doctorID)
        {
            try
            {
                //var doctor = db.SP_GetDoctorInfoforAppointment(doctorID).ToList();
                var doctor = (from doc in db.Doctors
                              where doc.doctorID == doctorID && doc.active == true
                              select new
                              {
                                  ProfilePhotoBase64 = doc.ProfilePhotoBase64,
                                  doctorName = doc.firstName + " " + doc.lastName,
                                  doctorGender = doc.gender,
                                  doctordob = doc.dob,
                                  dcellPhone = doc.cellPhone,
                                  dhomePhone = doc.homePhone,
                                  city = doc.city,
                                  state = doc.state,
                                  email=doc.email,
                                  consultCharges=doc.consultCharges,
                                  languages = (from l in db.DoctorLanguages
                                               where l.doctorID == doctorID && l.active == true
                                               select new { languageName = l.languageName }).ToList(),
                                  specialities = (from s in db.DoctorSpecialities
                                                  where s.doctorID == doctorID && s.active == true
                                                  select new { specialityName = s.specialityName }).ToList()

                              }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, doctor);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorInfo in SearchDoctorController");
            }
        }
        [HttpPost]
        [Route("api/favouriteDoctor")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> AddFavourite(FavouriteDoctorModel model)
        {
            FavouriteDoctor favdoc = new FavouriteDoctor();
            try
            {
                if (model.docID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.patID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                 favdoc = db.FavouriteDoctors.Where(fav=>fav.doctorID==model.docID && fav.patientID==model.patID && fav.active==false).FirstOrDefault();
                 if(favdoc!=null)
                {
                    favdoc.active = true;
                    favdoc.doctorID = model.docID;
                    favdoc.patientID = model.patID;
                    favdoc.mb = model.patID.ToString();
                    favdoc.md = System.DateTime.Now;
                    db.Entry(favdoc).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                 else
                {
                    favdoc = new FavouriteDoctor();
                    favdoc.active = true;
                    favdoc.doctorID = model.docID;
                    favdoc.patientID = model.patID;
                    favdoc.mb = model.patID.ToString();
                    favdoc.md = System.DateTime.Now;
                    db.FavouriteDoctors.Add(favdoc);
                    await db.SaveChangesAsync();
                }             
               
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddFavourite in SearchDoctorController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = favdoc.favID, message = "" });
            return response;
        }
        [HttpPost]
        [Route("api/unfavouriteDoctor")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> UpdateFavourite(FavouriteDoctorModel model)
        {
            FavouriteDoctor favdoc = new FavouriteDoctor();
            try
            {
                if (model.docID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.patID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                favdoc = db.FavouriteDoctors.Where(m => m.doctorID == model.docID && m.patientID==model.patID && m.active==true).FirstOrDefault();
                if(favdoc==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Record not found." });
                    return response;
                }
                else
                {
                    favdoc.active = false;
                    favdoc.doctorID = model.docID;
                    favdoc.patientID = model.patID;
                    favdoc.mb = model.patID.ToString();
                    favdoc.md = System.DateTime.Now;
                    db.Entry(favdoc).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
              
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateFavourite in SearchDoctorController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = favdoc.favID, message = "" });
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. "+ Action+"\n"+ex.Message, Encoding.Unicode);
            response.ReasonPhrase = ex.Message;
           return response;
        }
    }
}

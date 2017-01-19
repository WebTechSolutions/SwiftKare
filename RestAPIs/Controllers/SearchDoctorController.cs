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
                              select (from doc in db.Doctors where doc.doctorID == l.doctorID && doc.active == true
                                              select new DoctorDataset
                                              {
                                                  doctorID = doc.doctorID,
                                                  firstName = doc.firstName,
                                                  lastName = doc.lastName,
                                                  picture = doc.picture
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

        // POST: api/searchDoctor/SeeDoctorViewModel
        //[Route("api/searchDoctor")]
        //public HttpResponseMessage SeeDoctor(SearchDoctorModel searchModel)
        //{
          

        //    try
        //    {
        //        if(searchModel.appDate==null)
        //        {
        //            var result = db.SP_SearchDoctor(searchModel.language, searchModel.speciality, searchModel.name, null,
        //            searchModel.appTime, searchModel.gender).ToList();
        //            response = Request.CreateResponse(HttpStatusCode.OK, result);
        //            //return response;
        //        }

        //        if (searchModel.appDate != null)
        //        {
        //            DateTime day = new DateTime();
        //            day = Convert.ToDateTime(searchModel.appDate);
        //            var result = 
        //                db.SP_SearchDoctor(searchModel.language, searchModel.speciality, searchModel.name, day.DayOfWeek.ToString(),
        //            searchModel.appTime, searchModel.gender).ToList();
        //            response = Request.CreateResponse(HttpStatusCode.OK, result);
        //            //return response;
        //        }
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {

        //       return ThrowError(ex, "SeeDoctor in SeacrhDoctorController.");
        //    }
         

        //}

       
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
                                      picture=l.picture,
                                      state=l.state,
                                      languageName=l.languageName,
                                      specialityName=l.specialityName
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
                    DateTime day = new DateTime();
                    day = Convert.ToDateTime(searchModel.appDate);
                    var result = (from l in db.SearchDoctorWithShift(searchModel.language, searchModel.speciality, searchModel.name, null,
                    appFromtimings, appTotimings, searchModel.gender)
                                  select new SearchDoctorWithShift_Model
                                  {
                                      doctorID = l.doctorID,
                                      title = l.title,
                                      firstName = l.firstName,
                                      lastName = l.lastName,
                                      city = l.city,
                                      picture = l.picture,
                                      state = l.state,
                                      languageName = l.languageName,
                                      specialityName = l.specialityName
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

        [Route("api/fetchDoctorTime")]
        public HttpResponseMessage FetchDoctorTime(FetchTimingsModel searchModel)
        {
            try
            {
                //var dateTime = DateTime.Parse(searchModel.appDate);
                string[] formats = { "dd/MM/yyyy" };
                var dateTime = DateTime.ParseExact(searchModel.appDate.Trim(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                var result = db.SP_FetchDoctorTimings(searchModel.doctorID, dateTime).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, result);
               return response;
               
            }
            catch (Exception ex)
            {

                return ThrowError(ex, "FetchDoctorTime in SearchDoctorController");
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
                                  docPicture = doc.picture,
                                  doctorName = doc.firstName + " " + doc.lastName,
                                  doctorGender = doc.gender,
                                  doctordob = doc.dob,
                                  dcellPhone = doc.cellPhone,
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

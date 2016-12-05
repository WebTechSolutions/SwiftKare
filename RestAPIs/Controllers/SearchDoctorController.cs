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

namespace RestAPIs.Controllers
{
    [Authorize]
    public class SearchDoctorController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;

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
        [Route("api/searchDoctor")]
        public HttpResponseMessage SeeDoctor(SearchDoctorModel searchModel)
        {
          

            try
            {
                if(searchModel.appDate==null)
                {
                    var result = db.SP_SearchDoctor(searchModel.language, searchModel.speciality, searchModel.name, null,
                    searchModel.appTime, searchModel.gender).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    //return response;
                }

                if (searchModel.appDate != null)
                {
                    DateTime day = new DateTime();
                    day = Convert.ToDateTime(searchModel.appDate);
                    var result = db.SP_SearchDoctor(searchModel.language, searchModel.speciality, searchModel.name, day.DayOfWeek.ToString(),
                    searchModel.appTime, searchModel.gender).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    //return response;
                }
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
                var result = db.SP_FetchDoctorTimings(searchModel.doctorID, Convert.ToDateTime(searchModel.appDate)).ToList();
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
                var doctor = db.SP_GetDoctorInfoforAppointment(doctorID).ToList();
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
           return response;
        }
    }
}

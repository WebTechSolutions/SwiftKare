using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace RestAPIs.Controllers
{
    [Authorize]
    public class ConsultationController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

       
        [Route("api/GetConsultationDetails")]
        public HttpResponseMessage GetConsultationDetails(long consultID)
        {
            try
            {
                if (consultID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid consultation ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetConsultationDetails(consultID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetConsultationDetails in ConsultationController");
            }


        }
        [Route("api/GetROS")]
        public HttpResponseMessage GetROS(long consultID)
        {
            try
            {
                if (consultID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid consultation ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetROS(consultID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROS in ConsultationController");
            }


        }

        [Route("api/GetPatientConsultations")]
        public HttpResponseMessage GetPatientConsultations(long patientID)
        {
            try
            {
                if (patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid patient ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetPatientConsultations(patientID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientConsultations in ConsultationController");
            }


        }

        [Route("api/GetDcotorConsultations")]
        public HttpResponseMessage GetDcotorConsultations(long doctorID)
        {
            try
            {
                if (doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid doctor ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetDcotorConsultations(doctorID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
              
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDcotorConsultations in ConsultationController");
            }


        }

        [HttpPost]
        [Route("api/AddConsultReview")]
        public async Task<HttpResponseMessage> AddConsultReview(AddConsultReviewodel model)
        {
            try
            {
                if (model.consultID == 0 || model.consultID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }
                if (model.patientID == 0 || model.patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.star == 0 || model.star == null|| model.star>5||model.star<0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid review star." });
                    return response;
                }
                Consultation result = db.Consultations.Where(app => app.consultID == model.consultID && app.active == true).FirstOrDefault();
                if (result == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Consultation not found" });
                    return response;
                }
                else
                {
                   
                        result.mb = model.patientID.ToString();
                    result.md = System.DateTime.Now;
                    result.review = model.reviewText;
                        result.reviewStar = model.star;
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                   
                }

                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = model.consultID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "RescheduleRequest in AppointmentController");
            }


        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at " + Action });
            return response;

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

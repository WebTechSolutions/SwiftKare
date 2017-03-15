using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestAPIs.Controllers
{
    public class LiveRequestLogController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;

        private bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("api/addLiveReqLog")]
        public async Task<HttpResponseMessage> AddLiveReqLog(LiveReqLogModel model)
        {
            LiveReqLog lrlog = new LiveReqLog();
            try
            {
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }

                if (model.From == "" || model.From == null || !(IsValid(model.From)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for sender of message." });
                    return response;
                }

                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.message == "" || model.message == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Message text is missing." });
                    return response;
                }


                lrlog.patientID = model.patientID;
                lrlog.cd = System.DateTime.Now;
                lrlog.doctorID = model.doctorID;
                lrlog.message = model.message;
                lrlog.From = model.From;
                lrlog.cb = model.From;
              
                db.LiveReqLogs.Add(lrlog);
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = lrlog.LiveReqID, message = "" });
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddLiveReqLog in LiveRequestLogController");
            }
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            response.ReasonPhrase = ex.Message;
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

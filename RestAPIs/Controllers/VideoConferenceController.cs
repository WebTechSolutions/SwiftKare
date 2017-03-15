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
    [Authorize]

    public class VideoConferenceController : ApiController
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
        [Route("api/addVCLog")]
        public async Task<HttpResponseMessage> AddVCLog(VCLogModel model)
        {
            VCLog vclog = new VCLog();
            try
            {
                if (model.consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consult ID." });
                    return response;
                }

                if (model.endBy == "" || model.endBy == null || !(IsValid(model.endBy)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for endBy field." });
                    return response;
                }

                if (model.logBy == "" || model.logBy == null || !(IsValid(model.logBy)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for logBy field." });
                    return response;
                }
                if (model.endReason == "" || model.endReason == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide end reason." });
                    return response;
                }
                

                vclog.consultID = model.consultID;
                vclog.cd = System.DateTime.Now;
                vclog.endReason = model.endReason;
                vclog.endBy = model.endBy;
                vclog.logBy = model.logBy;
                vclog.duration = model.duration;
                db.VCLogs.Add(vclog);
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = vclog.VCLogID, message = "" });
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddVCLog in VideoConferenceController");
            }
        }

        [Route("api/getConsultationSOAP")]
        public HttpResponseMessage getConsultationSOAP(long consultID)
        {
            try
            {
                //var result = db.SP_GetConsultationDetails(consultID).ToList();
                var result = (from cn in db.Consultations
                              where cn.consultID == consultID && cn.active == true
                              select new
                              {
                                  consultID = cn.consultID,
                                  subjective = cn.subjective,
                                  objective = cn.objective,
                                  assessment = cn.assessment,
                                  plans = cn.plans,
                                  rosItems = (from ros in db.ConsultationROS
                                              where ros.consultationID == cn.consultID && ros.active == true
                                              select new { systemItemName = ros.systemItemName }).ToList(),
                                  chat = (from l in db.ChatLogs
                                         where l.consultID == cn.consultID
                                         orderby l.chatID ascending
                                         select new
                                         {
                                             sender = l.sender,
                                             reciever = l.reciever,
                                             message = l.message,
                                             cd=l.cd
                                         }).ToList()
                              }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "getConsultationSOAP in VideoConferenceController");
            }


        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            response.ReasonPhrase = ex.InnerException.Message;
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

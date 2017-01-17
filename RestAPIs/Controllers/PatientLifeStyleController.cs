using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientLifeStyleController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getLifeStyleQuestions")]
        public HttpResponseMessage GetLifeStyleQuestions()
        {
            try
            {
                var questions = (from l in db.LifeStyleQuestions
                               where l.active == true
                               select new { questionID = l.questionID, question = l.question.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, questions);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetLifeStyleQuestions in PatientLifeStyleController");
            }

        }

        [Route("api/getPatientLifeStyle")]
        public HttpResponseMessage GetPatientLifeStyle(long patientID)
        {
            try
            {
                var ptlifetstyle = (from l in db.PatientLifeStyles
                                    where l.active == true && l.patientID == patientID
                                    select new { patientlifestyleID = l.patientlifestyleID, patientID = l.patientID, question = l.question.Trim(), answer = l.answer,
                                        questionID = (from lifestyle in db.LifeStyleQuestions
                                                      where lifestyle.question == l.question select lifestyle.questionID).FirstOrDefault()
                                  }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, ptlifetstyle);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientLifeStyle in PatientLifeStyleController");
            }

        }
        [HttpPost]
        [Route("api/addPatientLifeStyle")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientLifeStyle(PatientLifeStyle_Custom model)
        {
            PatientLifeStyle plifestyle = new PatientLifeStyle();
            try
            {
                if (model.question == null || model.question == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid question." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }

                
                    plifestyle = new PatientLifeStyle();
                    plifestyle.active = true;
                    plifestyle.question = model.question;
                    plifestyle.answer = model.answer;
                    plifestyle.patientID = model.patientID;
                    plifestyle.cd = System.DateTime.Now;
                    plifestyle.cb = model.patientID.ToString();
                    db.PatientLifeStyles.Add(plifestyle);
                    await db.SaveChangesAsync();
                

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientLifeStyle in PatientLifeStyleController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = plifestyle.patientlifestyleID, message = "" });
            return response;
        }
        [HttpPost]
        [Route("api/editPatientLifeStyle")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientLifeStyle(PatientLifeStyleModel model)
        {
            PatientLifeStyle pls= new PatientLifeStyle();
            try
            {
                if (model.answer == null || model.answer == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid answer." });
                    return response;
                }
                if (model.patientlifestyleID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient life style ID." });
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                pls = db.PatientLifeStyles.Where(all => all.patientlifestyleID == model.patientlifestyleID && all.patientID == model.patientID).FirstOrDefault();
                if (pls != null)
                {
                    pls.answer = model.answer;
                    pls.md = System.DateTime.Now;
                    pls.mb = model.patientID.ToString();
                    db.Entry(pls).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                   
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "PatientLifeStyle not found." });
                    return response;
                }               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientLifeStyle in PatientLifeStyleController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.patientlifestyleID, message = "" });
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {

            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method:" + Action + " " + ex.Message });
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

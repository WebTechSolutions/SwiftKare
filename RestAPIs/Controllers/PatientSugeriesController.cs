using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientSugeriesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;


        [Route("api/getSurgeries")]
        public HttpResponseMessage GetSurgeries()
        {
            try
            {
                var surgeries = (from l in db.Surgeries
                                 where l.active == true
                                 select new SurgeriesModel { surgeryID = l.surgeryID, surgeryName = l.surgeryName.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, surgeries);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSurgeries in PatientSurgeriesController");
            }

        }

        [Route("api/getPatienSurgeries")]
        public HttpResponseMessage GetPatientSugeries(long patientID)
        {
            try
            {
                var surgeries = (from l in db.PatientSurgeries
                                 where l.active == true && l.patientID == patientID
                                 select new GetPatientSurgeries { surgeryID = l.surgeryID, bodyPart = l.bodyPart.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, surgeries);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientSugeries in PatientSurgeriesController");
            }

        }

        [Route("api/addPatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientSugery(PatientSurgery_Custom model)
        {
            PatientSurgery psurgery = new PatientSurgery();
            try
            {
                if (model.bodyPart == null || model.bodyPart == "" || !Regex.IsMatch(model.bodyPart.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid surgery.Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }
                
                psurgery = db.PatientSurgeries.Where(p => p.bodyPart.Trim() == model.bodyPart.Trim()).FirstOrDefault();
                if (psurgery!=null)
                {
                   // response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery already exists." });
                    //return response;
                    psurgery.md = System.DateTime.Now;
                    psurgery.mb = psurgery.patientID.ToString();
                    psurgery.active = true;
                    db.Entry(psurgery).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = psurgery.surgeryID, message = "" });
                    return response;
                }
                if (psurgery==null)
                {
                    psurgery = new PatientSurgery();
                    psurgery.active = true;
                    psurgery.bodyPart = model.bodyPart;
                    psurgery.patientID = model.patientID;
                    psurgery.cd = System.DateTime.Now;
                    psurgery.reportedDate = System.DateTime.Now;
                    psurgery.cb = model.patientID.ToString();

                    db.PatientSurgeries.Add(psurgery);
                    await db.SaveChangesAsync();
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = psurgery.surgeryID, message = "" });
            return response;
        }

        [Route("api/editPatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientSugery(long surgeryID,PatientSurgery_Custom model)
        {
            PatientSurgery psurgery = new PatientSurgery();
            try
            {
                if (model.bodyPart == null || model.bodyPart == "" || !Regex.IsMatch(model.bodyPart.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID =0, message = "Invalid surgery. Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }
                psurgery = db.PatientSurgeries.Where(all => all.bodyPart.Trim() == model.bodyPart.Trim() && all.surgeryID != surgeryID && all.active == true).FirstOrDefault();
                if (psurgery != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery already exists." });
                    return response;
                }
                psurgery = db.PatientSurgeries.Where(m => m.surgeryID == surgeryID).FirstOrDefault();
                if (psurgery == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery found." });
                    return response;
                }
               
                psurgery.bodyPart = model.bodyPart;
                psurgery.md = System.DateTime.Now;
                psurgery.mb = psurgery.patientID.ToString();
                db.Entry(psurgery).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = surgeryID, message = "" });
            return response;
        }

      
        [Route("api/deletePatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> RemovePatientSurgery(long surgeryID)
        {
            try
            {
                if (surgeryID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid surgery ID." });
                    return response;
                }
                PatientSurgery psurgery = await db.PatientSurgeries.FindAsync(surgeryID);
            
                if (psurgery == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery not found." });
                    return response;
                }
                else
                {
                    
                    psurgery.active = false;//Delete Operation changed
                    psurgery.mb = psurgery.patientID.ToString();
                    psurgery.md = System.DateTime.Now;
                    db.Entry(psurgery).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = surgeryID, message = "" });
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            //response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            //return response;
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method:" + Action + "\n" + ex.InnerException });
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


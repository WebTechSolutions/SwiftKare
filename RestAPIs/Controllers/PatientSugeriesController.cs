using DataAccess;
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
    public class PatientSugeriesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPatienSugeries/patientId/")]
        public HttpResponseMessage GetPatientSugeries(long patientId)
        {
            try
            {
                var newsugery = db.SP_GetPatientSurgeries(patientId);
                response = Request.CreateResponse(HttpStatusCode.OK, newsugery);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientSugeries in PatientSurgeriesController");
            }

        }

        [Route("api/addPatientSugery/sugeryModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientSugery(PatientSurgery_Custom model)
        {
            Surgery psurgery = new Surgery();
            try
            {
                if (model.bodyPart == null || model.bodyPart == "" || model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "PatientSugery Model is not valid.");
                    return response;
                }
                Patient patient = db.Patients.Where(p => p.userId == model.userID).FirstOrDefault();

                psurgery.active = true;
                psurgery.bodyPart = model.bodyPart;
                psurgery.patientID = model.patientID;
                psurgery.cd = System.DateTime.Now;
                psurgery.reportedDate = System.DateTime.Now;
                psurgery.cb = patient.email;

                db.Surgeries.Add(psurgery);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientSurgery in PatientSurgeriesController.");
            }

            var newpsurgery = db.SP_GetPatientSurgeries(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, newpsurgery);
            return response;
        }

        [Route("api/editPatientSugery/sugeryModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientSugery(PatientSurgery_Custom model)
        {
            try
            {
                if (model.bodyPart==null||model.bodyPart==""||model.patientID==null||model.patientID==0||model.surgeryID==0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Surgery Model is not valid.");
                    return response;
                }
                Surgery psurgery = db.Surgeries.Where(m => m.surgeryID == model.surgeryID).FirstOrDefault();
                if (psurgery == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Surgery record not found.");
                    return response;
                }
                Patient patient = db.Patients.Where(p => p.userId == model.userID).FirstOrDefault();
                psurgery.bodyPart = model.bodyPart;
                psurgery.md = System.DateTime.Now;
                psurgery.mb = patient.email;
                db.Entry(psurgery).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientSurgery in PatientSurgeriesController.");
            }

            var newpsurgery = db.SP_GetPatientSurgeries(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, newpsurgery);
            return response;
        }

        [HttpPost]
        [Route("api/deletePatientSurgery/surgeryModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientCondition(long surgeryId,long patientId)
        {
            try
            {
                if (surgeryId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Surgery Model is not valid.");
                    return response;
                }
                Surgery psurgery = await db.Surgeries.FindAsync(surgeryId);
                Patient patient = await db.Patients.FindAsync(patientId);
                if (psurgery == null || patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Surgery record not found.");
                    return response;
                }
                psurgery.active = false;//Delete Operation changed
                psurgery.mb = patient.email;
                psurgery.md = System.DateTime.Now;
                db.Entry(psurgery).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientSurgery in PatientSurgeriesController.");
            }

            var newpsurgery = db.SP_GetPatientSurgeries(patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newpsurgery);
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
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


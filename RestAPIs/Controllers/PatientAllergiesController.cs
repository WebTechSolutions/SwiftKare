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
    public class PatientAllergiesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPatientAllergies/patientId/")]
        public HttpResponseMessage GetPatientAllergies(long Id)
        {
            try
            {
                var newallergy = db.SP_GetPatientAllergies(Id);
                response = Request.CreateResponse(HttpStatusCode.OK, newallergy);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientAllergies in PatientAllergiesController");
            }

        }

        [Route("api/addPatientAllergies/allergyModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientAllergy(PatientAllergies_Custom model)
        {
            PatientAllergy pallergy = new PatientAllergy();
            try
            {
                if (model.allergyName == ""||model.allergyName==null||model.patientID==0||model.reaction==""||model.reaction==null||model.severity==""
                    ||model.severity==null||model.userId==null||model.userId=="")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "PatientAllergy Model is not valid.");
                    return response;
                }
                

                pallergy.active = true;
                pallergy.allergyName = model.allergyName;
                pallergy.severity = model.severity;
                pallergy.reaction = model.reaction;
                pallergy.patientID = model.patientID;
                pallergy.cd = System.DateTime.Now;
                pallergy.source = "S";
                pallergy.reportedDate = System.DateTime.Now;
                pallergy.cb = model.userId;

                db.PatientAllergies.Add(pallergy);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientAllergy in PatientAllergiesController.");
            }

            //var newpallergy = db.SP_GetPatientAllergies(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, pallergy.allergiesID);
            return response;
        }

        [Route("api/editPatientAllergy/allergyModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientAllergy(PatientAllergies_Custom model)
        {
            try
            {
                if (model.allergyName == "" || model.allergyName == null || model.patientID == 0 || model.reaction == "" || model.reaction == null || model.severity == ""
                     || model.severity == null || model.userId == null || model.userId == ""||model.allergiesID==0)
                { 
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Allergy Model is not valid.");
                    return response;
                }
                PatientAllergy pallergy = db.PatientAllergies.Where(m => m.allergiesID == model.allergiesID).FirstOrDefault();
                if (pallergy == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Allergy record not found.");
                    return response;
                }
                //Patient patient = db.Patients.Where(p => p.userId == model.userId).FirstOrDefault();
                pallergy.allergyName = model.allergyName;
                pallergy.severity = model.severity;
                pallergy.reaction = model.reaction;
                pallergy.md = System.DateTime.Now;
                pallergy.mb = model.userId;
                db.Entry(pallergy).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientAllergy in PatientAllergiesController.");
            }

            
            response = Request.CreateResponse(HttpStatusCode.OK, model.allergiesID);
            return response;
        }
        [HttpPost]
        [Route("api/deletePatientAllergy/allergyModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientCondition(long allergyId,long patientId)
        {
            try
            {
                PatientAllergy pallergy = await db.PatientAllergies.FindAsync(allergyId);
                Patient patient = await db.Patients.FindAsync(patientId);
                if (pallergy == null || patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Allergy record not found.");
                    return response;
                }
                pallergy.active = false;//Delete Operation changed
                pallergy.mb = patient.email;
                pallergy.md = System.DateTime.Now;
                db.Entry(pallergy).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientAllergy in PatientAllergiesController.");
            }

            var newpallergy = db.SP_GetPatientAllergies(patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newpallergy);
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

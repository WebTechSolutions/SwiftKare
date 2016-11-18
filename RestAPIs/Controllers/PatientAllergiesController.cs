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
    public class PatientAllergiesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getAllergy")]
        public HttpResponseMessage GetAllergies()
        {
            try
            {
                var allergies = (from l in db.Allergies
                                 where l.active == true
                                 select new AllergiesModel { allergyID = l.allergyID, allergyName = l.allergyName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, allergies);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAllergies in PatientAllergiesController");
            }

        }
        [Route("api/getSeverity")]
        public HttpResponseMessage GetSeverities()
        {
            try
            {
                var severities = (from l in db.Severities
                                 where l.active == true
                                 select new Severity { severityID  = l.severityID, severityName = l.severityName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, severities);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAllergies in PatientAllergiesController");
            }

        }
        [Route("api/getReaction")]
        public HttpResponseMessage GetReactions()
        {
            try
            {
                var reactions = (from l in db.Reactions
                                  where l.active == true
                                  select new Reaction { reactionID = l.reactionID, reactionName = l.reactionName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, reactions);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetReactions in PatientAllergiesController");
            }

        }


        [Route("api/getPatientAllergies")]
        public HttpResponseMessage GetPatientAllergies(long patientID)
        {
            try
            {
                var allergies = (from l in db.PatientAllergies
                                 where l.active == true && l.patientID == patientID
                                 select new GetPatientAllergies { allergiesID = l.allergiesID, allergyName = l.allergyName, reaction = l.reaction, severity = l.severity, reporteddate = l.reportedDate }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, allergies);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientAllergies in PatientAllergiesController");
            }

        }

        [Route("api/addPatientAllergy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientAllergy(PatientAllergies_Custom model)
        {
            PatientAllergy pallergy = new PatientAllergy();
            try
            {
                if (model.allergyName == "" || model.allergyName == null || !Regex.IsMatch(model.allergyName, @"^[a-zA-Z\s]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Allergy Name.");
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Patient ID.");
                    return response;
                }

                pallergy = db.PatientAllergies.Where(all => all.allergyName == model.allergyName).FirstOrDefault();
                if(pallergy== null)
                {
                    pallergy = new PatientAllergy();
                    pallergy.active = true;
                    pallergy.allergyName = model.allergyName;
                    pallergy.severity = model.severity;
                    pallergy.reaction = model.reaction;
                    pallergy.patientID = model.patientID;
                    pallergy.cd = System.DateTime.Now;
                    pallergy.source = "S";
                    pallergy.reportedDate = System.DateTime.Now;
                    pallergy.cb = pallergy.patientID.ToString();

                    db.PatientAllergies.Add(pallergy);
                    await db.SaveChangesAsync();
                    
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Allergy already exists.");
                    return response;
                }
               

            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientAllergy in PatientAllergiesController.");
            }

            //var newpallergy = db.SP_GetPatientAllergies(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, pallergy.allergiesID);
            return response;
        }

        [Route("api/editPatientAllergy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientAllergy(long allergyID,PatientAllergies_Custom model)
        {
            try
            {
                if (model.allergyName == "" || model.allergyName == null || !Regex.IsMatch(model.allergyName, @"^[a-zA-Z\s]+$"))
                { 
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Allergy Name.");
                    return response;
                }
                if (model.patientID == 0 )
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Patient ID.");
                    return response;
                }
                if(allergyID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Allergies ID.");
                    return response;
                }

                PatientAllergy pallergy = db.PatientAllergies.Where(m => m.allergiesID == allergyID).FirstOrDefault();
                if (pallergy == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Allergy record not found.");
                    return response;
                }
                pallergy = db.PatientAllergies.Where(all => all.allergyName == model.allergyName && all.allergiesID!= allergyID).FirstOrDefault();
                if (pallergy == null)
                {
                    pallergy = new PatientAllergy();
                    pallergy.allergyName = model.allergyName;
                    pallergy.severity = model.severity;
                    pallergy.reaction = model.reaction;
                    pallergy.md = System.DateTime.Now;
                    pallergy.mb = pallergy.patientID.ToString();
                    db.Entry(pallergy).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Allergy already exists.");
                    return response;
                }
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientAllergy in PatientAllergiesController.");
            }

            
            response = Request.CreateResponse(HttpStatusCode.OK, allergyID);
            return response;
        }
        [HttpPost]
        [Route("api/deletePatientAllergy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientCondition(long allergyID)
        {
            try
            {
                PatientAllergy pallergy = await db.PatientAllergies.FindAsync(allergyID);
                
                if (pallergy == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient Allergy record not found.");
                    return response;
                }
                pallergy.active = false;//Delete Operation changed
                pallergy.mb = pallergy.patientID.ToString();
                pallergy.md = System.DateTime.Now;
                db.Entry(pallergy).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientAllergy in PatientAllergiesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, allergyID);
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

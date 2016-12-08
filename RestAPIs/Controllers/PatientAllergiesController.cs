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
                                 select new AllergiesModel { allergyID = l.allergyID, allergyName = l.allergyName.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, allergies);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAllergies in PatientAllergiesController");
            }

        }
        [Route("api/getSensitivity")]
        public HttpResponseMessage GetSensitivites()
        {
            try
            {
                var severities = (from l in db.Severities
                                 where l.active == true
                                 select new SensitivityModel  { sensitivityID  = l.severityID, sensitivityName = l.severityName.Trim() }).ToList();
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
                                  select new ReactionModel { reactionID = l.reactionID, reactionName = l.reactionName.Trim() }).ToList();
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
                                 select new GetPatientAllergies { allergiesID = l.allergiesID, patientID=l.patientID, allergyName = l.allergyName.Trim(), reaction = l.reaction.Trim(), severity = l.severity.Trim(), reporteddate = l.reportedDate }).ToList();
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
                if (model.allergyName == "" || model.allergyName == null || !Regex.IsMatch(model.allergyName.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid allergy name. Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }

                pallergy = db.PatientAllergies.Where(all => all.patientID == model.patientID && all.allergyName.Trim() == model.allergyName.Trim() && all.active == true).FirstOrDefault();
               
                if (pallergy== null)
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
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Allergy already exists." });
                    return response;
                }
               

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientAllergy in PatientAllergiesController.");
            }

         
            response = Request.CreateResponse(HttpStatusCode.OK,  new ApiResultModel { ID = pallergy.allergiesID, message = "" });
            return response;
        }

        [Route("api/editPatientAllergy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientAllergy(long allergyID,PatientAllergies_Custom model)
        {
            PatientAllergy pallergy = new PatientAllergy();
            try
            {
                if (model.allergyName == "" || model.allergyName == null || !Regex.IsMatch(model.allergyName.Trim(), "^[0-9a-zA-Z ]+$"))
                { 
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid allergy name.Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == 0 )
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if(allergyID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid allergies ID." });
                    return response;
                }
                pallergy = db.PatientAllergies.Where(all => all.patientID == model.patientID && all.allergyName.Trim() == model.allergyName.Trim() && all.allergiesID != allergyID && all.active == true).FirstOrDefault();
                if (pallergy != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Allergy already exists." });
                    return response;
                }
                    pallergy = db.PatientAllergies.Where(m => m.allergiesID == allergyID).FirstOrDefault();
                if (pallergy == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Allergy not found." });
                    return response;
                }
                             
                  
                    pallergy.allergyName = model.allergyName;
                    pallergy.severity = model.severity;
                    pallergy.reaction = model.reaction;
                    pallergy.md = System.DateTime.Now;
                    pallergy.mb = pallergy.patientID.ToString();
                    db.Entry(pallergy).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientAllergy in PatientAllergiesController.");
            }

            
            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = allergyID, message = "" });
            return response;
        }
      
        [Route("api/deletePatientAllergy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientCondition(long allergyID)
        {
            try
            {
                PatientAllergy pallergy =  db.PatientAllergies.Where(all => all.allergiesID == allergyID && all.active == true).FirstOrDefault();

                if (pallergy == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Allergy not found." });
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

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = allergyID, message = "" });
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            //response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            //return response;
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method:" + Action + " " + ex.Message });
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

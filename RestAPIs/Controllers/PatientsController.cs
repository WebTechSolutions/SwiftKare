using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;
using DataAccess.CustomModels;
using System.Text.RegularExpressions;
using RestAPIs.Extensions;

namespace RestAPIs.Controllers
{
   
    public class PatientsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        // GET: api/Patients
        public IQueryable<Patient> GetPatients()
        {
            return db.Patients;
        }

        // GET: api/Patients/5
        [ResponseType(typeof(Patient))]
        public async Task<IHttpActionResult> GetPatient(long id)
        {
            Patient patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [ResponseType(typeof(PatientModel))]
        public PatientModel GetPatientByUserId(string userId, HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }
            try
            {

                Patient patient = db.Patients.SingleOrDefault(o => o.userId == userId);
                if (patient == null)
                    return null;

                var objModel = new PatientModel();
                objModel.patientID = patient.patientID;
                objModel.firstName = patient.firstName;
                objModel.lastName = patient.lastName;
                objModel.userId = patient.userId;
                objModel.email = patient.email;
                objModel.active = patient.active;
                objModel.picture = patient.picture;

                objModel.secretQuestion1 = patient.secretQuestion1;
                objModel.secretQuestion2 = patient.secretQuestion2;
                objModel.secretQuestion3 = patient.secretQuestion3;


                objModel.secretAnswer1 = patient.secretAnswer1;
                objModel.secretAnswer2 = patient.secretAnswer2;
                objModel.secretAnswer3 = patient.secretAnswer3;

                return objModel;
            }

            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        // PUT: api/Patients/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPatient(long id, Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patient.patientID)
            {
                return BadRequest();
            }

            db.Entry(patient).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Patients
        [ResponseType(typeof(Patient))]
        public async Task<IHttpActionResult> PostPatient(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = patient.patientID }, patient);
        }

        // DELETE: api/Patients/5
        [ResponseType(typeof(Patient))]
        public async Task<IHttpActionResult> DeletePatient(long id)
        {
            Patient patient = await db.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.active = false;//Delete Operation changed
            db.Entry(patient).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(patient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PatientExists(long id)
        {
            return db.Patients.Count(e => e.patientID == id) > 0;
        }

        //[HttpPost]
        //[Route("api/updatePatientProfile")]
        //[ResponseType(typeof(HttpResponseMessage))]
        //public async Task<HttpResponseMessage> UpdatePatientProfile(long patientID,PatientProfileModel model)
        //{

        //    Patient patient = new Patient();
        //    try
        //    {

        //        if (model.firstName == null || model.firstName == "" || !Regex.IsMatch(model.firstName, "^[0-9a-zA-Z ]+$"))
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "First name is not valid. Only letter and numbers are allowed." });
        //            return response;
        //        }
        //        if (model.lastName != null || model.lastName != "")
        //        {
        //            if (!Regex.IsMatch(model.lastName, "^[0-9a-zA-Z ]+$"))                    //@"^[a-zA-Z\s]+$"
        //            {
        //                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Last name is not valid. Only letter and numbers are allowed." });
        //                return response;
        //            }
        //        }
        //        if (patientID == 0)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
        //            return response;
        //        }
        //        patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
        //        if (patient == null)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient not found." });
        //            return response;
        //        }
        //        else
        //        {
        //            patient.active = true;
        //            patient.firstName = model.firstName;
        //            patient.lastName = model.lastName;
        //            patient.cd = System.DateTime.Now;
        //            patient.homePhone = model.homePhone;
        //            patient.cellPhone = model.cellPhone;
        //            patient.address1 = model.address1;
        //            patient.address2 = model.address2;
        //            patient.gender = model.gender;
        //            patient.dob = model.dob;
        //            patient.picture = model.picture;
        //            //patient.timezone = model.timezone;
        //            //patient.city = model.city;
        //            //patient.suffix = model.suffix;
        //            //patient.title = model.title;
        //            //patient.height = model.height;
        //            //patient.weight = model.weight;

        //            db.Entry(patient).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = patient.patientID, message = "" });
        //            return response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return ThrowError(ex, "UpdatePatientProfile in PatientController.");
        //    }
            
        //}

        //[HttpGet]
        //[Route("api/getPatientProfile")]
        //[ResponseType(typeof(HttpResponseMessage))]
        //public HttpResponseMessage GetPatientProfile(long patientID)
        //{

        //    Patient patient = new Patient();
        //    try
        //    {

        //        if (patientID == 0)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
        //            return response;
        //        }
        //        patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
        //        if (patient == null)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient does not exist." });
        //            return response;
        //        }
        //        else
        //        {
        //            var profile = (from l in db.Patients
        //        where l.active == true && l.patientID == patientID
        //        select new PatientProfileModel { firstName = l.firstName,
        //        lastName = l.lastName, gender = l.gender.Trim(), address1 = l.address1.Trim(),
        //        address2 = l.address2.Trim(),cellPhone=l.cellPhone,homePhone=l.homePhone,city=l.city,
        //        dob=l.dob,height=l.height,weight=l.weight,picture=l.picture,title=l.title,suffix=l.suffix,
        //        timezone=l.timezone,state=l.state,zip=l.zip}).FirstOrDefault();
        //            response = Request.CreateResponse(HttpStatusCode.OK, profile);
        //            return response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return ThrowError(ex, "UpdatePatientProfile in PatientController.");
        //    }

        //}
        //[Route("api/updatePatientPicture")]
        //[ResponseType(typeof(HttpResponseMessage))]
        //public async Task<HttpResponseMessage> UpdatePatientPicture(UpdatePatientPicture model)
        //{
        //    Patient patient = new Patient();
        //    try
        //    {
        //        if (model.patientID == 0)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
        //            return response;
        //        }
        //        if (model.picture == null )
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid picture." });
        //            return response;
        //        }
                
        //        //check for duplicate names
        //        patient = db.Patients.Where(m => m.patientID == model.patientID && m.active == true).FirstOrDefault();
        //        if (patient == null)
        //        {
                    
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient not found." });
        //            return response;
        //        }

        //       else
        //        {
        //            patient.picture = model.picture;
        //            patient.md = System.DateTime.Now;
        //            patient.mb = model.patientID.ToString();
        //            db.Entry(patient).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.patientID, message = "" });
        //            return response;

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return ThrowError(ex, "UpdatePatientPicture in PatientController.");
        //    }

        //}
        
        //private HttpResponseMessage ThrowError(Exception ex, string Action)
        //{
            
        //    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
        //    return response;
        //}
    }
}
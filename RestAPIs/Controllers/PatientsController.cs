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

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

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

        //[ResponseType(typeof(Patient))]
        //public async Task<IHttpActionResult> GetPatientByUserId(string userId)
        //{
        //    Patient patient = await db.Patients.SingleOrDefaultAsync(o => o.userId == userId);
        //    if (patient == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(patient);
        //}

        //[ResponseType(typeof(DataAccess.CustomModels.UserModel))]
        public DataAccess.CustomModels.PatientModel GetPatientByUserId(string userId)
        {
            Patient patient = db.Patients.SingleOrDefault(o => o.userId == userId);
            var objModel = new DataAccess.CustomModels.PatientModel();
            objModel.patientID = patient.patientID;
            objModel.firstName = patient.firstName;
            objModel.lastName = patient.lastName;
            objModel.email = patient.email;


            return objModel;
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
    }
}
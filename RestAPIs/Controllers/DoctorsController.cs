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
    public class DoctorsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

        // GET: api/Doctors
        public IQueryable<Doctor> GetDoctors()
        {
            return db.Doctors;
        }

        // GET: api/Doctors/5
        [ResponseType(typeof(Doctor))]
        public async Task<IHttpActionResult> GetDoctor(long id)
        {
            Doctor doctor = await db.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        // GET: api/Doctors/afafaf
        public Doctor GetDoctorByUserId(string userId)
        {
            
            Doctor doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);
            return doctor;
        }

        // GET: api/Doctors/afafaf
        [Route("api/Doctors/Id")]
        public long GetDoctorId(string userId)
        {
            Doctor doctor = db.Doctors.SingleOrDefault(o => o.userId == userId);
            return doctor.doctorID;
        }


        // PUT: api/Doctors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDoctor(long id, Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctor.doctorID)
            {
                return BadRequest();
            }

            db.Entry(doctor).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
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

        // POST: api/Doctors
        [ResponseType(typeof(Doctor))]
        public async Task<IHttpActionResult> PostDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Doctors.Add(doctor);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = doctor.doctorID }, doctor);
        }

        // DELETE: api/Doctors/5
        [ResponseType(typeof(Doctor))]
        public async Task<IHttpActionResult> DeleteDoctor(long id)
        {
            Doctor doctor = await db.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.active = false;//Delete Operation changed
            db.Entry(doctor).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(doctor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DoctorExists(long id)
        {
            return db.Doctors.Count(e => e.doctorID == id) > 0;
        }
    }
}
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
    public class DoctorTimingsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

        // GET: api/DoctorTimings
        public IQueryable<DoctorTiming> GetDoctorTimings()
        {
            return db.DoctorTimings;
        }

        // GET: api/DoctorTimings/5
        [ResponseType(typeof(DoctorTiming))]
        public async Task<IHttpActionResult> GetDoctorTiming(long id)
        {
            DoctorTiming doctorTiming = await db.DoctorTimings.FindAsync(id);
            if (doctorTiming == null)
            {
                return NotFound();
            }

            return Ok(doctorTiming);
        }

        //api/DoctorTimings? doctorId = { doctorId }
        public IQueryable<DoctorTiming> GetDoctorTimingByDoctorId(long doctorId)
        {
            return db.DoctorTimings.Where(o=>o.doctorID==doctorId);
        }

        // PUT: api/DoctorTimings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDoctorTiming(long id, DoctorTiming doctorTiming)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctorTiming.doctorTimingsID)
            {
                return BadRequest();
            }

            db.Entry(doctorTiming).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorTimingExists(id))
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

        // POST: api/DoctorTimings
        [ResponseType(typeof(DoctorTiming))]
        public async Task<IHttpActionResult> PostDoctorTiming(DoctorTiming doctorTiming)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DoctorTimings.Add(doctorTiming);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = doctorTiming.doctorTimingsID }, doctorTiming);
        }

        // DELETE: api/DoctorTimings/5
        [ResponseType(typeof(DoctorTiming))]
        public async Task<IHttpActionResult> DeleteDoctorTiming(long id)
        {
            DoctorTiming doctorTiming = await db.DoctorTimings.FindAsync(id);
            if (doctorTiming == null)
            {
                return NotFound();
            }

            db.DoctorTimings.Remove(doctorTiming);
            await db.SaveChangesAsync();

            return Ok(doctorTiming);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DoctorTimingExists(long id)
        {
            return db.DoctorTimings.Count(e => e.doctorTimingsID == id) > 0;
        }
    }
}
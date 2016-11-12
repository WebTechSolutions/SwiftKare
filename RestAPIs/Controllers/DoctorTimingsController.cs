using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;
using DataAccess.CustomModels;
using System.Globalization;

namespace RestAPIs.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorTimingsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();


        // GET: api/DoctorTimings/5
        [ResponseType(typeof(DoctorTimingsModel))]
        public async Task<IHttpActionResult> GetDoctorTiming(long id)
        {
            DoctorTiming doctorTiming = await db.DoctorTimings.FindAsync(id);
            if (doctorTiming == null)
                return NotFound();

            var model = new DoctorTimingsModel();
            model.doctorID = (long)doctorTiming.doctorID;
            model.doctorTimingsID = doctorTiming.doctorTimingsID;
            model.day = doctorTiming.day;
            var from = DateTime.Today.Add((TimeSpan)doctorTiming.from);
            var to = DateTime.Today.Add((TimeSpan)doctorTiming.to);
            model.from = from.ToString("hh:mm tt");
            model.to = to.ToString("hh:mm tt");

            return Ok(model);
        }

        //api/DoctorTimings? doctorId = { doctorId }
        public List<DoctorTimingsModel> GetDoctorTimingByDoctorId(long doctorId)
        {
            var timings = new List<DoctorTimingsModel>();
            var doctorTimingList = db.DoctorTimings.Where(o => o.doctorID == doctorId && o.active == true).ToList();
            foreach (var doctorTiming in doctorTimingList)
            {
                var model = new DoctorTimingsModel();
                model.doctorID = (long)doctorTiming.doctorID;
                model.doctorTimingsID = doctorTiming.doctorTimingsID;
                model.day = doctorTiming.day;
                var from = DateTime.Today.Add((TimeSpan)doctorTiming.from);
                var to = DateTime.Today.Add((TimeSpan)doctorTiming.to);
                model.from = from.ToString("hh:mm tt");
                model.to = to.ToString("hh:mm tt");
                timings.Add(model);
            }
            return timings;
        }

        // PUT: api/DoctorTimings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDoctorTiming(long id, DoctorTimingsModel doctorTimingModel)
        {
            var doctorTiming = new DoctorTiming();


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctorTimingModel.doctorTimingsID)
            {
                return BadRequest();
            }
            doctorTiming.doctorID = doctorTimingModel.doctorID;
            doctorTiming.doctorTimingsID = id;
            doctorTiming.day = doctorTimingModel.day;
            doctorTiming.active = true;
            doctorTiming.md = DateTime.Now;
            doctorTiming.mb = doctorTimingModel.username;

            DateTime dateTimeFrom = DateTime.ParseExact(doctorTimingModel.from,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime dateTimeTo = DateTime.ParseExact(doctorTimingModel.to,
                                "hh:mm tt", CultureInfo.InvariantCulture);

            doctorTiming.from = dateTimeFrom.TimeOfDay;
            doctorTiming.to = dateTimeTo.TimeOfDay;

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
        [ResponseType(typeof(DoctorTimingsModel))]
        public async Task<IHttpActionResult> PostDoctorTiming(DoctorTimingsModel doctorTimingModel)
        {
            var doctorTiming = new DoctorTiming();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            doctorTiming.doctorID = doctorTimingModel.doctorID;
            doctorTiming.doctorTimingsID = 0;
            doctorTiming.day = doctorTimingModel.day;

            DateTime dateTimeFrom = DateTime.ParseExact(doctorTimingModel.from,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime dateTimeTo = DateTime.ParseExact(doctorTimingModel.to,
                                "hh:mm tt", CultureInfo.InvariantCulture);

            doctorTiming.from = dateTimeFrom.TimeOfDay;
            doctorTiming.to = dateTimeTo.TimeOfDay;
            doctorTiming.active = true;
            doctorTiming.cd = DateTime.Now;
            doctorTiming.md = DateTime.Now;
            doctorTiming.cb = doctorTimingModel.username;

            db.DoctorTimings.Add(doctorTiming);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = doctorTiming.doctorTimingsID }, doctorTiming);
        }

        // DELETE: api/DoctorTimings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteDoctorTiming(long id)
        {

            DoctorTiming doctorTiming = await db.DoctorTimings.FindAsync(id);
            if (doctorTiming == null)
            {
                return NotFound();
            }
            doctorTiming.active = false;//Delete Operation changed
            doctorTiming.md = DateTime.Now;
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
            var model = new DoctorTimingsModel();
            model.doctorID = (long)doctorTiming.doctorID;
            model.doctorTimingsID = doctorTiming.doctorTimingsID;
            model.day = doctorTiming.day;
            var from = DateTime.Today.Add((TimeSpan)doctorTiming.from);
            var to = DateTime.Today.Add((TimeSpan)doctorTiming.to);
            model.from = from.ToString("hh:mm tt");
            model.to = to.ToString("hh:mm tt");

            return StatusCode(HttpStatusCode.OK);
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
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
using RestAPIs.Helper;
using System.Web.WebPages.Html;
using System.Net.Http;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class DoctorTimingsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

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

        [Route("api/getDoctorTimeZoneID")]
        public HttpResponseMessage getDoctorTimeZoneID(long doctorId)
        {

            var timezoneid = db.Doctors.Where(d => d.doctorID == doctorId).Select(d => d.timezone).FirstOrDefault();
            response = Request.CreateResponse(HttpStatusCode.OK, timezoneid);
            return response;

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
            var timingsList = GetDoctorTimingByDoctorId(id);
            var alreadItems = timingsList
                .Where(o => o.day == doctorTimingModel.day &&
                (o.from == doctorTimingModel.from || o.to == doctorTimingModel.to
                ||
                (
                DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay

                )
                ||
                (
                DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                )

                ||
                (
                DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                )

                )).ToList();
            if (alreadItems.Count >= 0)
            {
            }

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

            if (doctorTimingModel.from.Contains("PM"))
            {
                if (doctorTimingModel.to.Contains("AM"))
                {
                    return BadRequest("Timings should be within single day.");
                }
            }
            if (doctorTimingModel.from == doctorTimingModel.to)
            {
                return BadRequest("From Time and To Time can not be same.");

            }
            if (DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <
                DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay)
            {
                return BadRequest("From Time can not be greater than To Time.");
            }
            TimeSpan diff = DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay -
               DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            if (diff.TotalMinutes < 15)
            {
                return BadRequest("Timespan less than 15 minutes is not allowed.");
            }
            var doctorTiming = new DoctorTiming();
            var timingsList = GetDoctorTimingByDoctorId(doctorTimingModel.doctorID);
            var timezoneid = db.Doctors.Where(d => d.doctorID == doctorTimingModel.doctorID).Select(d => d.timezone).FirstOrDefault();
            TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());
            DateTime fromtimeUTC = DateTime.ParseExact(doctorTimingModel.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
            fromtimeUTC = TimeZoneInfo.ConvertTimeToUtc(fromtimeUTC, zoneInfo);
            DateTime totimeUTC = DateTime.ParseExact(doctorTimingModel.to,
                                  "hh:mm tt", CultureInfo.InvariantCulture);
            totimeUTC = TimeZoneInfo.ConvertTimeToUtc(totimeUTC, zoneInfo);
            var alreadItems = timingsList
                    .Where(o => o.day == doctorTimingModel.day &&
                    (o.from == fromtimeUTC.ToString("hh:mm tt") || o.to == totimeUTC.ToString("hh:mm tt")
                    ||
                    (
                    fromtimeUTC.TimeOfDay >=
                    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                    &&
                    fromtimeUTC.TimeOfDay <
                    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay

                    )
                    ||
                    (
                    totimeUTC.TimeOfDay >
                    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                    &&
                    totimeUTC.TimeOfDay <=
                    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                    )

                    ||
                    (
                    fromtimeUTC.TimeOfDay <=
                    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                    &&
                    totimeUTC.TimeOfDay >=
                    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                    )
                    ||
                    (
                    fromtimeUTC <
                    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture)
                    &&
                    totimeUTC >=
                    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture)
                    )
                    )).ToList();
            //var alreadItems = timingsList
            //    .Where(o => o.day == doctorTimingModel.day &&
            //    (o.from == doctorTimingModel.from || o.to == doctorTimingModel.to
            //    ||
            //    (
            //    DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
            //    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
            //    &&
            //    DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
            //    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay

            //    )
            //    ||
            //    (
            //    DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
            //    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
            //    &&
            //    DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
            //    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
            //    )

            //    ||
            //    (
            //    DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
            //    DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
            //    &&
            //    DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
            //    DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
            //    )

            //    )).ToList();

            if (alreadItems.Count > 0)
            {
                return BadRequest("Timings can not be overlapped across each other.");
                //return CreatedAtRoute("DefaultApi", new { message = "Timings can not be overlapped across each other" }, doctorTiming);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                doctorTiming.doctorID = doctorTimingModel.doctorID;
                doctorTiming.doctorTimingsID = 0;
                doctorTiming.day = doctorTimingModel.day;

                DateTime dateTimeFrom = DateTime.ParseExact(doctorTimingModel.from,
                                        "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime dateTimeTo = DateTime.ParseExact(doctorTimingModel.to,
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                //TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");//need to get zone info from db
                //get zoneid from db for current doctor
                //var timezoneid = db.Doctors.Where(d => d.doctorID == doctorTimingModel.doctorID).Select(d => d.timezone).FirstOrDefault();
                //TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());//need to get zone info from db
                doctorTiming.from = TimeZoneInfo.ConvertTimeToUtc(dateTimeFrom, zoneInfo).TimeOfDay;
                doctorTiming.to = TimeZoneInfo.ConvertTimeToUtc(dateTimeTo, zoneInfo).TimeOfDay;
                doctorTiming.active = true;
                doctorTiming.cd = DateTime.Now;
                doctorTiming.md = DateTime.Now;
                doctorTiming.cb = doctorTimingModel.username;

                db.DoctorTimings.Add(doctorTiming);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtRoute("DefaultApi", new { id = doctorTiming.doctorTimingsID }, doctorTiming);
        }

        // DELETE: api/DoctorTimings/5
        [Route("api/deleteDoctorTiming")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> RemoveDoctorTiming(long id)
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
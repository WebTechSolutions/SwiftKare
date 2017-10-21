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
            var temptimings = new List<DoctorTimingsModel>();
            var sortedtimings = new List<DoctorTimingsModel>();
            var doctorTimingList = db.DoctorTimings.Where(o => o.doctorID == doctorId && o.active == true).ToList();
            var doctz = db.Doctors.Where(p => p.doctorID == doctorId).Select(p => p.timezone).FirstOrDefault();
            TimeZoneInfo dzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(doctz.ToString());
            
            foreach (var doctorTiming in doctorTimingList)
            {
                //var model = new DoctorTimingsModel();
                //model.doctorID = (long)doctorTiming.doctorID;
                //model.doctorTimingsID = doctorTiming.doctorTimingsID;
                //model.day = doctorTiming.day;
                //DateTime? from = DateTime.UtcNow.Date;//.Add((TimeSpan)doctorTiming.from);
                //from = from + (TimeSpan)doctorTiming.from;
                //from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                //model.from = from.Value.ToString("hh:mm tt",CultureInfo.InvariantCulture);
                //DateTime? to = DateTime.UtcNow.Date;//.Add((TimeSpan)doctorTiming.to);
                //to = to + (TimeSpan)doctorTiming.to;
                //to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                //model.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                //timings.Add(model);
                var model = new DoctorTimingsModel();
                model.doctorID = (long)doctorTiming.doctorID;
                model.doctorTimingsID = doctorTiming.doctorTimingsID;
                model.day = doctorTiming.day;
                var from = DateTime.Today.Add((TimeSpan)doctorTiming.from);
                model.from = from.ToString("hh:mm tt");
                var to = DateTime.Today.Add((TimeSpan)doctorTiming.to);
                model.to = to.ToString("hh:mm tt");
                timings.Add(model);
            }

            foreach (var item in timings)
            {
                if (item.day == "Monday")
                {
                    var test = sortedtimings.Where(x => x.day == "Monday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Monday").ToList();
                        foreach(var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt",CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }
                        
                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Monday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }

                }
                if (item.day == "Tuesday")
                {
                    var test = sortedtimings.Where(x => x.day == "Tuesday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Tuesday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Tuesday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }
                if (item.day == "Wednesday")
                {
                    var test = sortedtimings.Where(x => x.day == "Wednesday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Wednesday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Wednesday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }
                if (item.day == "Thursday")
                {
                    var test = sortedtimings.Where(x => x.day == "Thursday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Thursday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Thursday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }
                if (item.day == "Friday")
                {
                    var test = sortedtimings.Where(x => x.day == "Friday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Friday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Friday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }
                if (item.day == "Saturday")
                {
                    var test = sortedtimings.Where(x => x.day == "Saturday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Saturday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Saturday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }
                if (item.day == "Sunday")
                {
                    var test = sortedtimings.Where(x => x.day == "Sunday").FirstOrDefault();
                    if (test == null)
                    {
                        temptimings = timings.Where(x => x.day == "Sunday").ToList();
                        foreach (var y in temptimings)
                        {
                            DateTime? from = DateTime.UtcNow.Date;
                            from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, dzoneInfo);
                            y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                            DateTime? to = DateTime.UtcNow.Date;
                            to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                            to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, dzoneInfo);
                            y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                        }

                        temptimings = temptimings.OrderBy(x => DateTime.ParseExact(x.from, "hh:mm tt", CultureInfo.InvariantCulture)).Where(x => x.day == "Sunday").ToList();
                        foreach (var i in temptimings)
                        {
                            var model = new DoctorTimingsModel();
                            model.doctorID = (long)i.doctorID;
                            model.doctorTimingsID = i.doctorTimingsID;
                            model.day = i.day;
                            DateTime fromtime = DateTime.ParseExact(i.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            fromtime = TimeZoneInfo.ConvertTimeToUtc(fromtime, dzoneInfo);
                            i.from = fromtime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.from = i.from;
                            DateTime totime = DateTime.ParseExact(i.to,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                            totime = TimeZoneInfo.ConvertTimeToUtc(totime, dzoneInfo);
                            i.to = totime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                            model.to = i.to;
                            sortedtimings.Add(model);

                        }
                    }
                }

            }


            return sortedtimings;
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
        public HttpResponseMessage PostDoctorTiming(DoctorTimingsModel doctorTimingModel)
        {

            if (doctorTimingModel.from.Contains("PM"))
            {
                if (doctorTimingModel.to.Contains("AM"))
                {
                    //return BadRequest("Timings should be within single day.");
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Timings should be within single day." });
                    response.ReasonPhrase = "Timings should be within single day.";
                    return response;
                }
            }
            if (doctorTimingModel.from == doctorTimingModel.to)
            {
                //return BadRequest("From Time and To Time can not be same.");
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "From Time and To Time can not be same." });
                response.ReasonPhrase = "From Time and To Time can not be same.";
                return response;

            }
            if (DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <
                DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay)
            {
                //return BadRequest("From Time can not be greater than To Time.");
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "From Time can not be greater than To Time." });
                response.ReasonPhrase = "From Time can not be greater than To Time.";
                return response;
            }
            TimeSpan diff = DateTime.ParseExact(doctorTimingModel.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay -
               DateTime.ParseExact(doctorTimingModel.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            if (diff.TotalMinutes < 15)
            {
                //return BadRequest("Timespan less than 15 minutes is not allowed.");
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Timespan less than 15 minutes is not allowed." });
                response.ReasonPhrase = "Timespan less than 15 minutes is not allowed.";
                return response;
            }
            var doctorTiming = new DoctorTiming();
            var timingsList = GetDoctorTimingByDoctorId(doctorTimingModel.doctorID);
            var timezoneid = db.Doctors.Where(d => d.doctorID == doctorTimingModel.doctorID).Select(d => d.timezone).FirstOrDefault();
            TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());
            foreach (var y in timingsList)
            {
                DateTime? from = DateTime.UtcNow.Date;
                from = from + DateTime.ParseExact(y.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                from = TimeZoneInfo.ConvertTimeFromUtc(from.Value, zoneInfo);
                y.from = from.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                DateTime? to = DateTime.UtcNow.Date;
                to = to + DateTime.ParseExact(y.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                to = TimeZoneInfo.ConvertTimeFromUtc(to.Value, zoneInfo);
                y.to = to.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            }
            DateTime fromtimeUTC = DateTime.ParseExact(doctorTimingModel.from,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
            //fromtimeUTC = TimeZoneInfo.ConvertTimeToUtc(fromtimeUTC,zoneInfo);
            DateTime totimeUTC = DateTime.ParseExact(doctorTimingModel.to,
                                  "hh:mm tt", CultureInfo.InvariantCulture);
            //totimeUTC = TimeZoneInfo.ConvertTimeToUtc(totimeUTC, zoneInfo);

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
           
            var chkpmtoam = alreadItems.Where(x => x.from.Contains("PM") && x.to.Contains("AM")).FirstOrDefault();
            //var otherthanpmtoam = alreadItems.Where(x => !(x.from.Contains("PM")) && !(x.to.Contains("AM"))).FirstOrDefault();
            var flag = 0;
            foreach (var t in alreadItems)
            {
                if(t.from.Contains("AM") && t.to.Contains("AM") || 
                    t.from.Contains("PM") && t.to.Contains("PM") ||
                    t.from.Contains("AM") && t.to.Contains("PM"))
                {
                    flag = 1;
                    break;
                }
            }
            if (alreadItems.Count > 0 )
            {
                if(flag == 1)
                {
                    //return BadRequest("Timings can not be overlapped across each other.");
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Timings can not be overlapped across each other." });
                    response.ReasonPhrase = "Timings can not be overlapped across each other.";
                    return response;
                }
               //return CreatedAtRoute("DefaultApi", new { message = "Timings can not be overlapped across each other" }, doctorTiming);
            }
            
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Model is not valid." });
                response.ReasonPhrase = "Model is not valid.";
                return response;
            }
            try
            {
                doctorTiming.doctorID = doctorTimingModel.doctorID;
                doctorTiming.doctorTimingsID = 0;
                doctorTiming.day = doctorTimingModel.day;

                string nextDay="";
                if (doctorTimingModel.day.ToString().ToLower().Equals("monday"))
                    nextDay = "Tuesday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("tuesday"))
                    nextDay = "Wednesday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("wednesday"))
                    nextDay = "Thursday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("thursday"))
                    nextDay = "Friday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("friday"))
                    nextDay = "Saturday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("saturday"))
                    nextDay = "Sunday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("Sunday"))
                    nextDay = "Monday";

                string prevDay = "";
                if (doctorTimingModel.day.ToString().ToLower().Equals("monday"))
                    prevDay = "Sunday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("tuesday"))
                    prevDay = "Monday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("wednesday"))
                    prevDay = "tuesday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("thursday"))
                    prevDay = "Wednesday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("friday"))
                    prevDay = "Thursday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("saturday"))
                    prevDay = "Friday";
                else if (doctorTimingModel.day.ToString().ToLower().Equals("Sunday"))
                    prevDay = "Saturday";

                DateTime dateTimeFrom = DateTime.ParseExact(doctorTimingModel.from,
                                        "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime dateTimeTo = DateTime.ParseExact(doctorTimingModel.to,
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                //For DayLightTimeSaving Issue at mobile devices
                TimeSpan ts = new TimeSpan(0,0, 60, 0);
                bool isDaylight = zoneInfo.IsDaylightSavingTime(dateTimeFrom);

                /*   if (isDaylight)
                   {
                       doctorTiming.from = TimeZoneInfo.ConvertTimeToUtc(dateTimeFrom, zoneInfo).Add(ts).TimeOfDay;
                       doctorTiming.to = TimeZoneInfo.ConvertTimeToUtc(dateTimeTo, zoneInfo).Add(ts).TimeOfDay;
                   }
                   else
                   {
                       doctorTiming.from = TimeZoneInfo.ConvertTimeToUtc(dateTimeFrom, zoneInfo).TimeOfDay;
                       doctorTiming.to = TimeZoneInfo.ConvertTimeToUtc(dateTimeTo, zoneInfo).TimeOfDay;
                   }*/
                DateTime from = TimeZoneInfo.ConvertTimeToUtc(dateTimeFrom, zoneInfo);
                DateTime to = TimeZoneInfo.ConvertTimeToUtc(dateTimeTo, zoneInfo);
                doctorTiming.from = from.TimeOfDay;
                doctorTiming.to = to.TimeOfDay;
                if (from.Date > dateTimeFrom.Date) doctorTiming.utcDay = nextDay;
                else if (from.Date < dateTimeFrom.Date) doctorTiming.utcDay = prevDay;
                else doctorTiming.utcDay= doctorTimingModel.day;
                doctorTiming.active = true;
                doctorTiming.cd = DateTime.Now;
                doctorTiming.md = DateTime.Now;
                doctorTiming.cb = doctorTimingModel.username;

                db.DoctorTimings.Add(doctorTiming);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                //return BadRequest(ex.Message);
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = ex.Message });
                response.ReasonPhrase = ex.Message;
                return response;
            }
            

            //return CreatedAtRoute("DefaultApi", new { id = doctorTiming.doctorTimingsID }, doctorTiming);
            response = Request.CreateResponse(HttpStatusCode.OK, new { id = doctorTiming.doctorTimingsID, doctorTiming });
            return response;
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
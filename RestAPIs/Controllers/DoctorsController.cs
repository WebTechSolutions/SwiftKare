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
using RestAPIs.Models;

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

        [HttpGet]
        [Route("api/searchDoctor/docName/")]
        //  public IEnumerable<SeeDoctorDTO> SeeDoctor(string docName)//, string gender, string langName, string specName, string weekday, TimeSpan time)
        public IEnumerable<SeeDoctorDTO> SeeDoctor(string docName)//, string gender, string langName, string specName, string weekday, TimeSpan time)
        {
            //try
            //{
            //    System.Diagnostics.Debugger.Break();
            //    var result = from doclist in db.Doctors
            //                 select new SeeDoctorDTO
            //                 {

            //                     firstName = doclist.firstName,
            //                     lastName = doclist.lastName,
            //                     gender = doclist.gender,
            //                     DoctorTimings = doclist.DoctorTimings,
            //                     DoctorLanguages = doclist.DoctorLanguages,
            //                     DoctorSpecialities = doclist.DoctorSpecialities
            //                 };

            //    if (!string.IsNullOrEmpty(docName))
            //        result = result.Where(x => x.firstName.Contains(docName));
            //    if (!string.IsNullOrEmpty(docName))
            //        result = result.Where(x => x.lastName.Contains(docName));
            //    if (gender != "All")
            //        result = result.Where(x => x.gender == gender);
            //    if (!string.IsNullOrEmpty(weekday))
            //        result = result.Include(p => p.DoctorTimings.Any(c => c.day == weekday));
            //    if (!string.IsNullOrEmpty(time.ToString()))
            //        result = result.Include(p => p.DoctorTimings.Any(c => c.@from <= time
            //        && c.to >= time));
            //    if (langName != "All")
            //        result = result.Include(p => p.DoctorLanguages.Any(c => c.languageName == langName));
            //    if (specName != "All")
            //        result = result.Where(p => p.DoctorSpecialities.Any(c => c.specialityName == specName));
            //    return result.ToList();
            //}
            //catch(Exception ex)
            //{
            //    return null;
            //}
            return null;

        }
        // POST: api/searchDoctor/SeeDoctorViewModel
       // [Route("api/searchDoctor/searchModel/")]
        //[ResponseType(typeof(Doctor))]
        //public IEnumerable<SeeDoctorDTO> SeeDoctor(SeeDoctorViewModel searchModel)
        //{
        //    try
        //    {


        //        var result = from doclist in db.Doctors
        //                     select new SeeDoctorDTO
        //                     {

        //                         firstName = doclist.firstName,
        //                         lastName = doclist.lastName,
        //                         gender = doclist.gender,
        //                         DoctorTimings = doclist.DoctorTimings,
        //                         DoctorLanguages = doclist.DoctorLanguages,
        //                         DoctorSpecialities = doclist.DoctorSpecialities
        //                     };

        //        if (searchModel != null)
        //        {
        //            if (!string.IsNullOrEmpty(searchModel.Doctor.firstName))
        //                result = result.Where(x => x.firstName.Contains(searchModel.Doctor.firstName));
        //            if (!string.IsNullOrEmpty(searchModel.Doctor.firstName))
        //                result = result.Where(x => x.lastName.Contains(searchModel.Doctor.firstName));
        //            if (searchModel.Gender != "ALL")
        //                result = result.Where(x => x.gender == searchModel.Gender);
        //            if (searchModel.AppDate != null)
        //                result = result.Where(p => p.DoctorTimings.Any(c => c.day.ToString() == searchModel.AppDate.DayOfWeek.ToString()));
        //            if (searchModel.Timing.searchTime != null)
        //                result = result.Where(p => p.DoctorTimings.Any(c => c.@from <= searchModel.Timing.searchTime
        //                && c.to >= searchModel.Timing.searchTime));
        //            if (searchModel.Language != "ALL")
        //                result = result.Where(p => p.DoctorLanguages.Any(c => c.languageName == searchModel.Language));
        //            if (searchModel.Speciallity != "ALL")
        //                result = result.Where(p => p.DoctorSpecialities.Any(c => c.specialityName == searchModel.Speciallity));


        //        }
        //        if (!result.Any())
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //             return result.ToList();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
               
        //        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //        httpResponseMessage.Content = new StringContent(ex.Message);
        //        throw new HttpResponseException(httpResponseMessage);
        //    }
          

        //}
    }
  
}
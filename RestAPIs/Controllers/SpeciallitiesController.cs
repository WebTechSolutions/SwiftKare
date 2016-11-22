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
using DataAccess.CommonModels;
using System.Text;

namespace RestAPIs.Controllers
{
    public class SpeciallitiesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;
        [Route("api/Speciallities")]
        public HttpResponseMessage GetSpeciallities()
        {
            try
            {
                var specialities = (from l in db.Speciallities
                                 where l.active == true
                                 select new Specialities { speciallityID = l.speciallityID, specialityName = l.specialityName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, specialities);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSpeciallities in SpecialitiesController");
            }
        }


        private IEnumerable<Speciallity> Get()
        {
            return db.Speciallities.ToList();
        }
        // GET: api/Speciallities/5
        [ResponseType(typeof(Speciallity))]
        public async Task<IHttpActionResult> GetSpeciallity(long id)
        {
            Speciallity speciallity = await db.Speciallities.FindAsync(id);
            if (speciallity == null)
            {
                return NotFound();
            }

            return Ok(speciallity);
        }

        // PUT: api/Speciallities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSpeciallity(long id, Speciallity speciallity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != speciallity.speciallityID)
            {
                return BadRequest();
            }

            db.Entry(speciallity).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciallityExists(id))
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

        // POST: api/Speciallities
        [ResponseType(typeof(Speciallity))]
        public async Task<IHttpActionResult> PostSpeciallity(Speciallity speciallity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Speciallities.Add(speciallity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = speciallity.speciallityID }, speciallity);
        }

        // DELETE: api/Speciallities/5
        [ResponseType(typeof(Speciallity))]
        public async Task<IHttpActionResult> DeleteSpeciallity(long id)
        {
            Speciallity speciallity = await db.Speciallities.FindAsync(id);
            if (speciallity == null)
            {
                return NotFound();
            }

            db.Speciallities.Remove(speciallity);
            await db.SaveChangesAsync();

            return Ok(speciallity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SpeciallityExists(long id)
        {
            return db.Speciallities.Count(e => e.speciallityID == id) > 0;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            return response;
        }
    }
}
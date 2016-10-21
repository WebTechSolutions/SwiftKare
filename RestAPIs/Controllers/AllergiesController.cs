using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class AllergiesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

        // GET: api/Allergies
        public IQueryable<Allergy> GetAllergies()
        {
            return db.Allergies;
        }

        // GET: api/Allergies/5
        [ResponseType(typeof(Allergy))]
        public async Task<IHttpActionResult> GetAllergy(long id)
        {
            Allergy allergy = await db.Allergies.FindAsync(id);
            if (allergy == null)
            {
                return NotFound();
            }

            return Ok(allergy);
        }

        // PUT: api/Allergies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAllergy(long id, Allergy allergy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != allergy.allergyID)
            {
                return BadRequest();
            }

            db.Entry(allergy).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllergyExists(id))
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

        // POST: api/Allergies
        [ResponseType(typeof(Allergy))]
        public async Task<IHttpActionResult> PostAllergy(Allergy allergy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Allergies.Add(allergy);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = allergy.allergyID }, allergy);
        }

        // DELETE: api/Allergies/5
        [ResponseType(typeof(Allergy))]
        public async Task<IHttpActionResult> DeleteAllergy(long id)
        {
            Allergy allergy = await db.Allergies.FindAsync(id);
            if (allergy == null)
            {
                return NotFound();
            }

            db.Allergies.Remove(allergy);
            await db.SaveChangesAsync();

            return Ok(allergy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AllergyExists(long id)
        {
            return db.Allergies.Count(e => e.allergyID == id) > 0;
        }
    }
}
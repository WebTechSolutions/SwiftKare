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
using Identity.Membership;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Identity.Membership.Models;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class RolesController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

        // GET: api/Roles
        public IQueryable<AspNetRole> GetAspNetRoles()
        {
            return db.AspNetRoles;
        }

        // GET: api/Roles/5
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> GetAspNetRole(string id)
        {
            AspNetRole aspNetRole = await db.AspNetRoles.FindAsync(id);
            if (aspNetRole == null)
            {
                return NotFound();
            }

            return Ok(aspNetRole);
        }

        // PUT: api/Roles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAspNetRole(string id, AspNetRole aspNetRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aspNetRole.Id)
            {
                return BadRequest();
            }

            db.Entry(aspNetRole).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRoleExists(id))
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

        [Route("api/Roles/AssignRole")]
        [ResponseType(typeof(Microsoft.AspNet.Identity.IdentityResult))]
        public async Task<IHttpActionResult> AssignRole(UserAssignRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await UserManager.AddToRoleAsync(model.UserId, model.Role);
            return Ok(result);
        }

        // POST: api/Roles
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> PostAspNetRole(AspNetRole aspNetRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetRoles.Add(aspNetRole);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetRoleExists(aspNetRole.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspNetRole.Id }, aspNetRole);
        }

        // DELETE: api/Roles/5
        [ResponseType(typeof(AspNetRole))]
        public async Task<IHttpActionResult> DeleteAspNetRole(string id)
        {
            AspNetRole aspNetRole = await db.AspNetRoles.FindAsync(id);
            if (aspNetRole == null)
            {
                return NotFound();
            }

            db.AspNetRoles.Remove(aspNetRole);
            await db.SaveChangesAsync();

            return Ok(aspNetRole);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetRoleExists(string id)
        {
            return db.AspNetRoles.Count(e => e.Id == id) > 0;
        }
    }
}
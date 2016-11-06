using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    public class AppointmentController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/addAppointment/appModel/")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAppointments(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            db.Appointments.Add(model);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }


            return CreatedAtRoute("DefaultApi", new { id = model.appID }, model);
        }
        [Route("api/addRovChiefComplaints/appModel/")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> AddROV(Appointment model)
        {
            try
            {
                    if (!ModelState.IsValid)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "ROV model Model is not valid.");
                    return response;
                }


                db.Appointments.Add(model);
           
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddROV in AppointmentController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, model.appID);
            return response;
        }
        [Route("api/addPatientFiles/appModel/")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> AddPatientFiles(UserFile model)
        {
            if (!ModelState.IsValid)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "UserFile model is not valid.");
                return response;
            }
            db.UserFiles.Add(model);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddROV in AppointmentController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, model.fileID);
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            return response;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}

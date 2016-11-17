using DataAccess;
using DataAccess.CommonModels;
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
        
        [Route("api/ROV")]
        public HttpResponseMessage GetROV(long id)
        {
            try
            {
                var rov = (from l in db.Appointments
                           where l.active == true && l.patientID == id
                           orderby l.appID descending
                           select new AppointmentModel{ rov = l.rov }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, rov);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROV in SearchDcotorController");
            }


        }
        [HttpPost]
        [Route("api/addAppointment/appModel/")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> AddAppointments(AppointmentModel model)
        {
            Appointment app = new Appointment();
            try
            {
                if (model.appDate==null || model.appTime==null||model.userID==null||model.doctorID==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment model is not valid.");
                    return response;
                }
                Patient patient = db.Patients.Where(p => p.userId == model.userID).FirstOrDefault();
               
                app.active = true;
                app.doctorID = model.doctorID;
                app.patientID = patient.patientID;
                app.appTime = model.appTime;
                app.appDate = model.appDate;
                app.cb = model.userID;
                app.cd = System.DateTime.Now;

                db.Appointments.Add(app);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddAppointments in AppointmentController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, app.appID);
            return response;
        }
        [HttpPost]
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

using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class AppointmentController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;
        private bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [Route("api/PatientPreviousROV")]
        public HttpResponseMessage GetROV(long patientID)
        {
            try
            {
                var rov = (from l in db.Appointments
                           where l.active == true && l.patientID == patientID
                           orderby l.appID descending
                           select new PatientROV { rov = l.rov }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, rov);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROV in SearchDcotorController");
            }


        }
        [Route("api/GetROVs")]
        public HttpResponseMessage GetROVs()
        {
            try
            {
                var rov = (from l in db.ROVs
                           where l.active == true
                           orderby l.rovID ascending
                           select new ROV_Custom { rovID = l.rovID, rov = l.name }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, rov);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROVs in SearchDcotorController");
            }


        }
        [Route("api/GetPatientChiefComplaints")]
        public HttpResponseMessage GetPatientChiefComplaints(long id)
        {
            try
            {
                var rov = (from l in db.Appointments
                           where l.active == true && l.patientID == id
                           orderby l.appID descending
                           select new PatientROV { rov=l.rov,chiefComplaints = l.chiefComplaints }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, rov);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientChiefComplaints in SearchDcotorController");
            }


        }
        [HttpPost]
        [Route("api/addAppointment")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> AddAppointments(AppointmentModel model)
        {
            Appointment app = new Appointment();
            try
            {
                if (model.appDate == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid appointment date." });
                    return response;
                }
                if (model.appTime == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid appointment time." });
                    return response;
                }
                if (model.doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                string outputTime = "";
                app.active = true;
                app.doctorID = model.doctorID;
                app.patientID = model.patientID;
                //if (model.appTime.Contains("AM"))
                //{
                //    outputTime = model.appTime.Replace("AM", "");
                //}
                //if (model.appTime.Contains("PM"))
                //{
                //    outputTime = model.appTime.Replace("PM", "");
                //}
                app.appTime = To24HrTime(model.appTime);
                app.appDate =Convert.ToDateTime(model.appDate);
                app.rov = model.rov;
                app.chiefComplaints = model.chiefComplaints;
                app.cb = model.patientID.ToString();
                app.paymentAmt = model.paymentAmt;
                Random rnd = new Random();
                app.paymentID= rnd.Next(100).ToString();
                app.cd = System.DateTime.Now;

                db.Appointments.Add(app);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddAppointments in AppointmentController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
            return response;
        }

        private TimeSpan To24HrTime(string time)
        {
            char[] delimiters = new char[] { ':', ' ' };
            string[] spltTime = time.Split(delimiters);

            int hour = Convert.ToInt32(spltTime[0]);
            int minute = Convert.ToInt32(spltTime[1]);
            int seconds = 0;

            string amORpm = spltTime[2];

            if (amORpm.ToUpper() == "PM")
            {
                hour = (hour % 12) + 12;
            }

            return new TimeSpan(hour, minute,seconds);
        }
        [Route("api/GetRescheduleAppforPatient")]
        public HttpResponseMessage GetRescheduleAppforPatient(long patientID)
        {
            try
            {
                if (patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid patient ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetRescheduleAppforPatient(patientID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetRescheduleAppforPatient in AppointmentController");
            }


        }
        [Route("api/GetRescheduleAppforDoctor")]
        public HttpResponseMessage GetRescheduleAppforDoctor(long doctorID)
        {
            try
            {
                if (doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid doctor ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetRescheduleAppforDoctor(doctorID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetRescheduleAppforDoctor in AppointmentController");
            }


        }
       
        [Route("api/GetUpcomingAppforPatient")]
        public HttpResponseMessage GetUpcomingAppforPatient(long patientID)
        {
            try
            {
                if (patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid patient ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetUpcomingAppforPatient(patientID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetUpcomingAppforPatient in AppointmentController");
            }


        }
        [Route("api/GetUpcomingAppforDoctor")]
        public HttpResponseMessage GetUpcomingAppforDoctor(long doctorID)
        {
            try
            {
                if (doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid doctor ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetUpcomingAppforDoctor(doctorID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetUpcomingAppforDoctor in AppointmentController");
            }


        }
        [Route("api/GetCancelledAppforPatient")]
        public HttpResponseMessage GetCancelledAppforPatient(long patientID)
        {
            try
            {
                if (patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid patient ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetCancelledAppforPatient(patientID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetCancelledAppforPatient in AppointmentController");
            }


        }

        [Route("api/GetCancelledAppforDoctor")]
        public HttpResponseMessage GetCancelledAppforDoctor(long doctorID)
        {
            try
            {
                if (doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid doctor ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetCancelledAppforDoctor(doctorID);
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetCancelledAppforDoctor in AppointmentController");
            }


        }

       

        [HttpPost]
        [Route("api/RescheduleRequest")]
        public async Task<HttpResponseMessage> RescheduleApp(RescheduleRequestModel model)
        {
            try
            {
                if(model.doctorID==0||model.doctorID==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.appID == 0 || model.appID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid appointment ID." });
                    return response;
                }
                Appointment result = db.Appointments.Where(app => app.appID == model.appID && app.active==true).FirstOrDefault();
                if(result==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Appointment not found" });
                    return response;
                }
                else
                {
                    string currDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string appDateTime = result.appDate.ToString() + result.appTime.ToString();
                    DateTime cdt = Convert.ToDateTime(currDateTime);
                    DateTime adt = Convert.ToDateTime(appDateTime);
                    TimeSpan timediff = cdt - adt;
                    if (timediff.TotalHours > 24)
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Appointment reschedule is not allowed after 24 hours." });
                        return response;
                    }
                    else
                    {
                        result.rescheduleRequiredBy = "D";
                        result.mb = model.doctorID.ToString();
                        result.md = System.DateTime.Now;
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = model.appID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "RescheduleRequest in AppointmentController");
            }


        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at"+Action });
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

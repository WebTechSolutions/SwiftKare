using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        [Route("api/GetAppDetail")]
        public HttpResponseMessage GetAppDetail(long appID)
        {
            try
            {
                    var result = db.SP_GetAppDetail(appID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
              
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAppDetail in AppointmentController");
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
               
                app.active = true;
                app.doctorID = model.doctorID;
                app.patientID = model.patientID;
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

        [HttpPost]
        [Route("api/rescheduleAppointment")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> RescheduleAppointments(RescheduleAppointmentModel model)
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
               
                if ( model.appID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid appointment ID." });
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                Appointment result = db.Appointments.Where(rapp => rapp.appID == model.appID && rapp.active == true).FirstOrDefault();
                string currDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime cdt = Convert.ToDateTime(currDateTime);
                DateTime ad = Convert.ToDateTime(result.appDate);
                TimeSpan at = TimeSpan.Parse(result.appTime.ToString());
                DateTime appDateTime = ad + at;

                DateTime cad = Convert.ToDateTime(model.appDate);
                TimeSpan cat = TimeSpan.Parse(To24HrTime(model.appTime.ToString()).ToString());
                DateTime cappDateTime = cad + cat;

                TimeSpan ctimediff = cappDateTime- appDateTime ;
                TimeSpan timediff = appDateTime - cdt;
                 string RescheduleLimit = ConfigurationManager.AppSettings["RescheduleLimit"].ToString();
                //if (ctimediff.TotalHours < 0 )
                //{
                //    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Reschedule is not allowed on back date." });
                //    return response;
                //}
                if (timediff.TotalHours < Convert.ToInt32(RescheduleLimit))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Reschedule is not allowed when appointment is about to start within 24 hours." });
                    return response;
                }
                else
                {

                    result.appTime = To24HrTime(model.appTime);
                    result.appDate = Convert.ToDateTime(model.appDate);
                    result.mb = model.patientID.ToString();
                    result.md = System.DateTime.Now;
                    result.consultationStatus = "C";
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    Alert alert = new Alert();
                    alert.alertFor = result.doctorID.ToString();
                    alert.alertText="Your appointment on "+ model.appDate+" at "+model.appTime + " is rescheduled by patient.";
                    alert.cd = System.DateTime.UtcNow;
                    alert.cb = model.patientID.ToString();
                    alert.active = true;
                    db.Alerts.Add(alert);
                    await db.SaveChangesAsync();
                }
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
                    var result = db.SP_GetRescheduleAppforPatient(patientID).ToList();
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
                if(model.doctorID==0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.appID == 0 )
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
                    DateTime ad = Convert.ToDateTime(result.appDate);
                    TimeSpan at = TimeSpan.Parse(result.appTime.ToString());
                    //string appDateTime = ad.ToString("g") + " "+at.ToString();
                    DateTime appDateTime = ad + at;
                    DateTime cdt = Convert.ToDateTime(currDateTime);
                    //DateTime adt = Convert.ToDateTime(appDateTime);
                    TimeSpan timediff = appDateTime-cdt ;
                    string RescheduleLimit = ConfigurationManager.AppSettings["RescheduleLimit"].ToString();
                    if (timediff.TotalHours < Convert.ToInt32(RescheduleLimit))
                    {
                        
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Appointment reschedule is not allowed when less than 24 hrs are left for appointment." });
                        return response;
                    }
                    else
                    {
                        result.rescheduleRequiredBy = "D";
                        result.consultationStatus = "R";
                        result.mb = model.doctorID.ToString();
                        result.md = System.DateTime.Now;
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.appID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "RescheduleRequest in AppointmentController");
            }


        }

        [HttpPost]
        [Route("api/CancelRescheduleRequest")]
        public async Task<HttpResponseMessage> CancelRescheduleRequest(RescheduleRequestModel model)
        {
            try
            {
                Appointment result = db.Appointments.Where(app => app.appID == model.appID && app.active == true).FirstOrDefault();
                if (result == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Appointment not found" });
                    return response;
                }
                else
                {
                        result.rescheduleRequiredBy = "";
                    result.consultationStatus = "";
                    result.mb = model.doctorID.ToString();
                        result.md = System.DateTime.Now;
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                   
                }

                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.appID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "CancelRescheduleRequest in AppointmentController");
            }


        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = ex.Message+" at "+Action });
            response.ReasonPhrase = ex.Message;
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

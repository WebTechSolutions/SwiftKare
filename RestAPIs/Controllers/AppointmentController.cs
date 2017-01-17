using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
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
                //var result = db.SP_GetAppDetail(appID).ToList();
                var result = (from cn in db.Appointments
                              where cn.appID == appID && cn.active == true
                              select new
                              {
                                  appID = cn.appID,
                                  rov = cn.rov,
                                  chiefcomplaints = cn.chiefComplaints,
                                  paymentAmt = cn.paymentAmt,
                                  appDate = cn.appDate,
                                  appTime = cn.appTime,
                                  PatientVM = (from r in db.Patients
                                               where r.patientID == cn.patientID && r.active == true
                                               select new
                                               {
                                                   patPicture = r.picture,
                                                   patientName = r.firstName + " " + r.lastName,
                                                   patientGender = r.gender,
                                                   pharmacy = r.pharmacy,
                                                   patientDOB = r.dob,
                                                   pcellPhone = r.cellPhone,
                                                   city = r.city,
                                                   state = r.state,
                                                   patlanguages = (from l in db.PatientLanguages
                                                                   where l.patientID == r.patientID && l.active == true
                                                                   select new { languageName = l.languageName }).ToList()
                                               }).FirstOrDefault(),
                                  DoctorVM = (from doc in db.Doctors
                                              where doc.doctorID == cn.doctorID && doc.active == true
                                              select new
                                              {
                                                  docPicture = doc.picture,
                                                  doctorName = doc.firstName + " " + doc.lastName,
                                                  doctorGender = doc.gender,
                                                  doctordob = doc.dob,
                                                  dcellPhone = doc.cellPhone,
                                                  city = doc.city,
                                                  state = doc.state,
                                                  languages = (from l in db.DoctorLanguages
                                                               where l.doctorID == cn.doctorID && l.active == true
                                                               select new { languageName = l.languageName }).ToList(),
                                                  specialities = (from s in db.DoctorSpecialities
                                                                  where s.doctorID == cn.doctorID && s.active == true
                                                                  select new { specialityName = s.specialityName }).ToList()

                                              }).FirstOrDefault()
                              }).FirstOrDefault();
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
                    response.ReasonPhrase = "Invalid appointment date.";
                    return response;
                }
                if (model.appTime == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid appointment time." });
                    response.ReasonPhrase = "Invalid appointment time.";
                    return response;
                }
                if (model.doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    response.ReasonPhrase = "Invalid doctorID.";
                    return response;
                }
                if (model.patientID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    response.ReasonPhrase = "Invalid patientID.";
                    return response;
                }
                if (model.appTime.Length < 8)
                {
                    model.appTime = "0" + model.appTime;
                }
                   
                DateTime myDateTime = DateTime.ParseExact(model.appTime,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                app.appointmentStatus = "C";
                app.active = true;
                app.doctorID = model.doctorID;
                app.patientID = model.patientID;
                //app.appTime = To24HrTime(model.appTime);
                          
                app.appTime= myDateTime.ToUniversalTime().TimeOfDay;
                app.appDate = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.appDate));
                app.rov = model.rov;
                app.chiefComplaints = model.chiefComplaints;
                app.cb = db.Patients.Where(p => p.patientID == model.patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault(); model.patientID.ToString();
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
            DateTime? tempappdate;
            TimeSpan? tempapptime;
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
                if (model.userID == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
                    return response;
                }
                Appointment result = db.Appointments.Where(rapp => rapp.appID == model.appID && rapp.active == true).FirstOrDefault();
                string currDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                DateTime cdt = Convert.ToDateTime(currDateTime);
                DateTime ad = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", result.appDate));
                TimeSpan at = TimeSpan.Parse(result.appTime.ToString());
                DateTime appDateTime = ad + at;

                //DateTime cad = Convert.ToDateTime(model.appDate);
                //TimeSpan cat = TimeSpan.Parse(To24HrTime(model.appTime.ToString()).ToString());
                //DateTime cappDateTime = cad + cat;

                //TimeSpan ctimediff = appDateTime-cappDateTime;
                TimeSpan timediff = appDateTime - cdt;
                string RescheduleLimit = ConfigurationManager.AppSettings["RescheduleLimit"].ToString();
                if (model.appType=="U")
                {
                    if (timediff.TotalHours < Convert.ToInt32(RescheduleLimit))
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Reschedule not allowed when appointment is about to start within 24 hours." });
                        return response;
                    }
                    else
                    {
                        tempappdate = result.appDate.Value.Date;
                        var formattedDate = string.Format("{0:dd/MM/yyyy}", tempappdate);
                        tempapptime = result.appTime;
                        var formattedTime = DateTime.Now.Date.Add(tempapptime.Value).ToString(@"hh\:mm\:tt");

                        result.appTime = To24HrTime(model.appTime);
                        result.appDate = Convert.ToDateTime(model.appDate);
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
                        result.appointmentStatus = "C";
                        result.rescheduleRequiredBy = "P";
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        Alert alert = new Alert();
                        alert.alertFor = result.doctorID;
                        alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + formattedDate  + " at " + formattedTime + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + model.appDate + " at " + model.appTime;
                        alert.cd = System.DateTime.Now;
                        alert.cb = model.userID;
                        alert.active = true;
                        alert.active = true;
                        db.Alerts.Add(alert);
                        await db.SaveChangesAsync();
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
                        return response;
                    }
                }
                if (model.appType == "R")
                {
                    tempappdate = result.appDate.Value.Date;
                    var formattedDate = string.Format("{0:dd/MM/yyyy}", tempappdate);
                    tempapptime = result.appTime;
                    var formattedTime = DateTime.Now.Date.Add(tempapptime.Value).ToString(@"hh\:mm\:tt");
                    result.appTime = To24HrTime(model.appTime);
                        result.appDate = Convert.ToDateTime(model.appDate);
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
                        result.appointmentStatus = "C";
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        Alert alert = new Alert();
                        alert.alertFor = result.doctorID;
                        alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + formattedDate + " at " + formattedTime + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + model.appDate + " at " + model.appTime;
                    alert.cd = System.DateTime.Now;
                        alert.cb = model.userID;
                        alert.active = true;
                        alert.active = true;
                        db.Alerts.Add(alert);
                        await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
                    return response;
                }
                if (model.appType == "P")
                {
                    tempappdate = result.appDate.Value.Date;
                    var formattedDate = string.Format("{0:dd/MM/yyyy}", tempappdate);
                    tempapptime = result.appTime;
                    var formattedTime = DateTime.Now.Date.Add(tempapptime.Value).ToString(@"hh\:mm\:tt");
                    result.appTime = To24HrTime(model.appTime);
                    result.appDate = Convert.ToDateTime(model.appDate);
                    result.mb = model.userID;
                    result.md = System.DateTime.Now;
                    result.appointmentStatus = "C";
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    Alert alert = new Alert();
                    alert.alertFor = result.doctorID;
                    alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + formattedDate + " at " + formattedDate + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + model.appDate + " at " + model.appTime;
                    alert.cd = System.DateTime.Now;
                    alert.cb = model.userID;
                    alert.active = true;
                    alert.active = true;
                    db.Alerts.Add(alert);
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "Provide appointment type." });
                    return response;
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddAppointments in AppointmentController.");
            }

           
        }

        [Route("api/GetPendingAppforPatient")]
        public HttpResponseMessage GetPendingAppforPatient(long patientID)
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
                    var result = db.SP_GetPendingAppforPatient(patientID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPendingAppforPatient in AppointmentController");
            }


        }

        [Route("api/GetPendingAppforDoctor")]
        public HttpResponseMessage GetPendingAppforDoctor(long doctorID)
        {
            try
            {
                if (doctorID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid patient ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetPendingAppforDoctor(doctorID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPendingAppforPatient in AppointmentController");
            }


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
                if(model.userID=="")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
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
                    if(model.appType=="P")
                    {
                        result.rescheduleRequiredBy = "D";
                        result.appointmentStatus = "R";
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        
                        await db.SaveChangesAsync();
                    }
                    if (model.appType == "U")
                    {
                        string currDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        DateTime ad = Convert.ToDateTime(result.appDate);
                        TimeSpan at = TimeSpan.Parse(result.appTime.ToString());
                        //string appDateTime = ad.ToString("g") + " "+at.ToString();
                        DateTime appDateTime = ad + at;
                        DateTime cdt = Convert.ToDateTime(currDateTime);
                        //DateTime adt = Convert.ToDateTime(appDateTime);
                        TimeSpan timediff = appDateTime - cdt;
                        string RescheduleLimit = ConfigurationManager.AppSettings["RescheduleLimit"].ToString();
                        if (timediff.TotalHours < Convert.ToInt32(RescheduleLimit))
                        {

                            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Reschedule not allowed when appointment is about to start within 24 hours." });
                            response.ReasonPhrase = "Reschedule not allowed when appointment is about to start within 24 hours.";
                            return response;
                        }
                        else
                        {
                            result.rescheduleRequiredBy = "D";
                            result.appointmentStatus = "R";
                            result.mb = model.userID;
                            result.md = System.DateTime.Now;
                            db.Entry(result).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
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
        public async Task<HttpResponseMessage> CancelRescheduleRequest(CancelRescheduleRequestModel model)
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
                    result.appointmentStatus = "C";
                    result.mb = model.userID;
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
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.Message, Encoding.Unicode);
            response.ReasonPhrase = ex.Message;
            return response;
            //response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = ex.Message+" at "+Action });
            //response.ReasonPhrase = ex.Message;
            //return response;
          
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

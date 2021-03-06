﻿using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using RestAPIs.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
                                  appDate = cn.appDate.ToString(),
                                  appTime = cn.appTime,
                                  PatientVM = (from r in db.Patients
                                               where r.patientID == cn.patientID
                                               select new
                                               {
                                                   patientid = r.patientID,
                                                   ProfilePhotoBase64 = r.ProfilePhotoBase64,
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
                                              where doc.doctorID == cn.doctorID
                                              select new
                                              {
                                                  docID = doc.doctorID,
                                                  ProfilePhotoBase64 = doc.ProfilePhotoBase64,
                                                  doctorName = "Dr. "+doc.firstName + " " + doc.lastName,
                                                  doctorGender = doc.gender,
                                                  doctordob = doc.dob,
                                                  dcellPhone = doc.cellPhone,
                                                  city = doc.city,
                                                  state = doc.state,
                                                  consultCharges=doc.consultCharges,
                                                  rating=doc.reviewStar,
                                                  languages = (from l in db.DoctorLanguages
                                                               where l.doctorID == cn.doctorID && l.active == true
                                                               select new { languageName = l.languageName }).ToList(),
                                                  specialities = (from s in db.DoctorSpecialities
                                                                  where s.doctorID == cn.doctorID && s.active == true
                                                                  select new { specialityName = s.specialityName }).ToList()

                                              }).FirstOrDefault(),
                                  AppFiles= (from l in db.UserFiles
                                             where l.active == true && l.AppID == cn.appID
                                             orderby l.fileID ascending
                                             select new AppFilesCustom
                                             {
                                                 fileID = l.fileID,
                                                 FileName = l.FileName.Trim(),
                                                 fileContentbytearray = l.fileContent,
                                                 documentType = l.documentType
                                             }).ToList()

                                 }).FirstOrDefault();

                foreach(var f in result.AppFiles)
                {
                    f.fileContent = "data:image/png;base64,"+Convert.ToBase64String(f.fileContentbytearray);
                    f.fileContentbytearray = null;
                }
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
                
                if (model.appTime.Trim().Length < 8)
                {
                    model.appTime = "0" + model.appTime.Trim();
                }

                string model_appdatetime_string = model.appDate.Trim() +" "+ model.appTime.Trim();
                DateTime model_appdatetime_DateTime = DateTime.ParseExact(model_appdatetime_string,
                                  "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime myDateTime = DateTime.ParseExact(model.appTime,
                                   "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime utcappdatecheck = myDateTime;
                app.appointmentStatus = "C";
                app.active = true;
                app.doctorID = model.doctorID;
                app.patientID = model.patientID;
                //app.appTime = To24HrTime(model.appTime);

                var timezoneid = db.Patients.Where(d => d.patientID == model.patientID).Select(d => d.timezone).FirstOrDefault();
                if (timezoneid.Trim() == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "User Timezone is missing." });
                    response.ReasonPhrase = "User Timezone is missing.";
                    return response;
                }
                TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());//need to get zone info from db

                TimeSpan ts = new TimeSpan(0, 1, 0, 0);
                bool isDaylight = zoneInfo.IsDaylightSavingTime(DateTime.Now);

                //For DayLightTimeSaving Issue at mobile devices
              /*  if (isDaylight)
                {
                    model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo).Add(ts);
                }
                else
                {
                    model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo);
                }*/

                model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo);

                app.appTime = model_appdatetime_DateTime.TimeOfDay;
                //app.appTime = TimeZoneInfo.ConvertTimeToUtc(myDateTime, zoneInfo).TimeOfDay;
                
                
                //app.appTime= myDateTime.ToUniversalTime().TimeOfDay;
                // app.appDate = DateTime.ParseExact(model.appDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //  app.appDate = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.appDate.Trim()));
                string dateString = model.appDate.Trim();
                string format = "dd/MM/yyyy";
                CultureInfo provider = CultureInfo.InvariantCulture;
                try
                {
                    DateTime result = DateTime.ParseExact(dateString, format, provider);
                    Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
                    //app.appDate = result;
                    DateTime utcappDateTime = result + utcappdatecheck.TimeOfDay;
                    //app.appDate = utcappDateTime.ToUniversalTime().Date;
                    app.appDate = model_appdatetime_DateTime.Date;
                }
                catch (FormatException)
                {
                    Console.WriteLine("{0} is not in the correct format.", dateString);
                }

                app.rov = model.rov;
                app.chiefComplaints = model.chiefComplaints;
                app.cb = db.Patients.Where(p => p.patientID == model.patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault(); model.patientID.ToString();
                app.mb = db.Patients.Where(p => p.patientID == model.patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault(); model.patientID.ToString();
                app.paymentAmt = model.paymentAmt;
                Random rnd = new Random();
                app.paymentID = rnd.Next(100).ToString();
                app.cd = System.DateTime.Now;
                app.md = System.DateTime.Now;
                db.Appointments.Add(app);
                await db.SaveChangesAsync();

                //Save Appointment files in database - Starts
                List<KeyValuePair<byte[], string>> lstFiles = new List<KeyValuePair<byte[], string>>();
                try
                {

                    if (!string.IsNullOrEmpty(model.rovFile1Base64))
                    {
                        var retBase64 = model.rovFile1Base64.Substring(model.rovFile1Base64.IndexOf("base64,") + 7);
                        byte[] filecontent;
                        filecontent = Convert.FromBase64String(retBase64);
                        lstFiles.Add(new KeyValuePair<byte[], string>(filecontent, model.rovFile1Name));
                    }

                    if (!string.IsNullOrEmpty(model.rovFile2Base64))
                    {
                        var retBase64 = model.rovFile2Base64.Substring(model.rovFile2Base64.IndexOf("base64,") + 7);
                        byte[] filecontent;
                        filecontent = Convert.FromBase64String(retBase64);
                        lstFiles.Add(new KeyValuePair<byte[], string>(filecontent, model.rovFile2Name));
                    }

                    if (!string.IsNullOrEmpty(model.rovFile3Base64))
                    {
                        var retBase64 = model.rovFile3Base64.Substring(model.rovFile3Base64.IndexOf("base64,") + 7);
                        byte[] filecontent;
                        filecontent = Convert.FromBase64String(retBase64);
                        lstFiles.Add(new KeyValuePair<byte[], string>(filecontent, model.rovFile3Name));
                    }

                    foreach (var itmFile in lstFiles)
                    {
                        var patfile = new UserFile();
                        patfile.active = true;
                        patfile.FileName = itmFile.Value;
                        patfile.patientID = model.patientID;
                        patfile.cd = System.DateTime.Now;
                        patfile.md = System.DateTime.Now;
                        patfile.doctorID = model.doctorID == -1 ? null : model.doctorID;
                        patfile.fileContent = itmFile.Key;
                        patfile.documentType = "Appointment";
                        patfile.AppID = app.appID;
                        patfile.cb = model.patientID.ToString();
                        db.UserFiles.Add(patfile);
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    return ThrowError(ex, "Unable to upload File. Appointment created successfully.");
                }

                //Save Appointment files in database - Ends

                //Send Email on new appointment
                //Get Email and iOSToken and Android Token of doctor and patient
                //pushNotificationHelper.SendPushNotification(diOSToken,dAndroidToken,piOSToken,pAndroidToken,"Push Title","Push Message",doctorID,patientID);

                var docemail = db.Doctors.Where(d => d.doctorID == model.doctorID).Select(d => d.email).FirstOrDefault();
                var patemail = db.Patients.Where(p => p.patientID == model.patientID).Select(p => p.email).FirstOrDefault();
                TimeZoneHelper tzh = new TimeZoneHelper();
               
                try
                {
                   
                    //DateTime ad = DateTime.ParseExact(dateString, format, provider);
                    //DateTime newappDateTime = ad + myDateTime.TimeOfDay;
                    EmailHelper oHelper = new EmailHelper(docemail, "New appointment.", "You have new appointment on " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, model.doctorID, 1) + ".");
                    oHelper.SendMessage();
                    oHelper = new EmailHelper(patemail, "New appointment.", "Your appointment has been scheduled successfully on " + tzh.convertTimeZone(model_appdatetime_DateTime, model.patientID, 0, 1) + ".");
                    oHelper.SendMessage();

                    pushModel pm = new pushModel();
                    pm.PPushTitle = "New Appointment";
                    pm.PPushMessage = "Patient has scheduled appointment with you on " + tzh.convertTimeZone(model_appdatetime_DateTime, model.patientID, 0, 1);
                    pm.DPushTitle = "New Appointment";
                    pm.DPushMessage = "Patient has scheduled appointment with you on " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, model.doctorID, 1);
                    pm.sendtoDoctor = true;
                    pm.sendtoPatient = false;
                    pm.doctorID = model.doctorID;
                    pm.patientID = model.patientID;

                    PushHelper ph = new PushHelper();
                    ph.sendPush(pm);

                }
                catch (FormatException)
                {
                    Console.WriteLine("{0} is not in the correct format.", dateString);
                }
                //DateTime ad = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.appDate.Trim()));
                //DateTime mydateTime = DateTime.ParseExact(model.appTime.Trim(),
                //                            "hh:mm tt", CultureInfo.InvariantCulture);
                //DateTime newappDateTime = ad + mydateTime.TimeOfDay;
                              
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddAppointments in AppointmentController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
            return response;
        }

        private static string MakeBase64Valid(string data)
        {
            data = data.Replace(" ", "+");

            int mod4 = data.Length % 4;
            if (mod4 > 0)
            {
                data += new string('=', 4 - mod4);
            }

            
            return data;
        }

        [HttpPost]
        [Route("api/rescheduleAppointment")]
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> RescheduleAppointments(RescheduleAppointmentModel model)
        {
            Appointment app = new Appointment();
            TimeZoneHelper tzh = new TimeZoneHelper();
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
                string format = "dd/MM/yyyy";
                CultureInfo provider = CultureInfo.InvariantCulture;
                string dtformat = "dd-MM-yyyy HH:mm:ss";
                DateTime cdt = DateTime.ParseExact(currDateTime, dtformat, provider);
               
               DateTime ad;
                ad = Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", result.appDate.Value.ToShortDateString()));
               
                TimeSpan at = TimeSpan.Parse(result.appTime.ToString());
                DateTime appDateTime = ad + at;

                //DateTime cad = Convert.ToDateTime(model.appDate);
                //TimeSpan cat = TimeSpan.Parse(To24HrTime(model.appTime.ToString()).ToString());
                //DateTime cappDateTime = cad + cat;

                //TimeSpan ctimediff = appDateTime-cappDateTime;
                TimeSpan timediff = appDateTime - cdt;
                string RescheduleLimit = ConfigurationManager.AppSettings["RescheduleLimit"].ToString();
                if (model.appTime.Length < 8)
                {
                    model.appTime = "0" + model.appTime;
                }

                var patient = (from p in db.Patients
                               where p.userId == model.userID
                               select new { patemail = p.email, patientID = p.patientID,timezone=p.timezone }).FirstOrDefault();
                var App= ((from a in db.Appointments
                            where a.appID == model.appID
                            select new { doctorID = a.doctorID}).FirstOrDefault());

                var doctor = (from d in db.Doctors
                              where d.doctorID == App.doctorID
                              select new { docemail = d.email, doctorID = d.doctorID,timezone=d.timezone }).FirstOrDefault();
                //Used for timezone conversion
               
                DateTime oldAppDateTime = result.appDate.Value.Date + result.appTime.Value;
                DateTime newaappDate = DateTime.ParseExact(model.appDate.Trim(), format, provider);//Convert.ToDateTime(String.Format("{0:dd/MM/yyyy}", model.appDate.Trim()));
                DateTime newappTime = DateTime.ParseExact(model.appTime.Trim(),
                                            "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime newappDateTime = newaappDate + newappTime.TimeOfDay;
                //Used for timezone conversion

                string model_appdatetime_string = model.appDate.Trim() + " " + model.appTime.Trim();
                DateTime model_appdatetime_DateTime = DateTime.ParseExact(model_appdatetime_string,
                                  "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
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

                       
                        DateTime mydateTime = DateTime.ParseExact(model.appTime,
                                             "hh:mm tt", CultureInfo.InvariantCulture);
                        DateTime utcappdatecheck = mydateTime;
                        var timezoneid = db.Patients.Where(d => d.userId == model.userID).Select(d => d.timezone).FirstOrDefault();
                        TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());//need to get zone info from db
                        model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo);
                        result.appTime = model_appdatetime_DateTime.TimeOfDay;
                        //DateTime localdateTime = DateTime.ParseExact(result.appTime.ToString(),
                        //                     "hh:mm tt", CultureInfo.InvariantCulture);
                        //var plocalapptime= TimeZoneInfo.ConvertTimeToUtc(localdateTime, zoneInfo).TimeOfDay;
                        //TimeZoneInfo dzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(doctor.timezone);
                        //var dlocalapptime = TimeZoneInfo.ConvertTimeToUtc(localdateTime, dzoneInfo).TimeOfDay;
                        
                        //date format start
                        string dateString = model.appDate.Trim();
                        string dateformat = "dd/MM/yyyy";
                        CultureInfo culture= CultureInfo.InvariantCulture;
                        try
                        {
                            DateTime resultedDate = DateTime.ParseExact(dateString, dateformat, culture);
                            Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
                            //app.appDate = result;
                            DateTime utcappDateTime = resultedDate + utcappdatecheck.TimeOfDay;

                            //result.appDate = utcappDateTime.ToUniversalTime().Date;
                            result.appDate= model_appdatetime_DateTime.Date;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("{0} is not in the correct format.", dateString);
                        }
                        //result.appDate = Convert.ToDateTime(model.appDate);
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
                        result.appointmentStatus = "C";
                        result.rescheduleRequiredBy = "P";
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        Alert alert = new Alert();
                        alert.alertFor = result.doctorID;
                        alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + tzh.convertTimeZone(oldAppDateTime,0, result.doctorID,1) + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, result.doctorID,1);
                        alert.cd = System.DateTime.Now;
                        alert.cb = model.userID;
                        alert.active = true;
                        db.Alerts.Add(alert);
                        await db.SaveChangesAsync();
                        //Send Email on new appointment
                       
                       
                        if (doctor.docemail != null)
                        {
                            
                            EmailHelper oHelper = new EmailHelper(doctor.docemail, "Reschedule appointment.", "Your appointment on " + tzh.convertTimeZone(oldAppDateTime, 0, result.doctorID, 1) + " has been rescheduled by patient to " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, result.doctorID, 1) + ".");
                            oHelper.SendMessage();
                        }
                        if (patient.patemail != null)
                        {
                            EmailHelper oHelper = new EmailHelper(patient.patemail, "Reschedule appointment.", "Your appointment on " + tzh.convertTimeZone(oldAppDateTime, result.patientID, 0,1) + " is rescheduled successfully to " + tzh.convertTimeZone(model_appdatetime_DateTime, result.patientID, 0,1) + ".");
                            oHelper.SendMessage();
                        }

                        pushModel pm = new pushModel();
                        pm.DPushTitle = "Reschedule Appointment";
                        pm.DPushMessage = "Your appointment on " + tzh.convertTimeZone(oldAppDateTime, 0, result.doctorID,1) + " has been rescheduled by patient to " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, result.doctorID,1) + ".";
                        pm.sendtoDoctor = true;
                        pm.sendtoPatient = false;
                        pm.doctorID = doctor.doctorID;
                        pm.patientID = patient.patientID;

                        PushHelper ph = new PushHelper();
                        ph.sendPush(pm);

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

                   
                    DateTime mydateTime = DateTime.ParseExact(model.appTime,
                                             "hh:mm tt", CultureInfo.InvariantCulture);
                    DateTime utcappdatecheck = mydateTime;
                    var timezoneid = db.Patients.Where(d => d.userId == model.userID).Select(d => d.timezone).FirstOrDefault();
                    TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());//need to get zone info from db
                    model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo);

                    result.appTime = model_appdatetime_DateTime.TimeOfDay;
                    //result.appTime = mydateTime.ToUniversalTime().TimeOfDay;
                    //date format start
                    string dateString = model.appDate.Trim();
                   // string format = "dd/MM/yyyy";
                  //  CultureInfo provider = CultureInfo.InvariantCulture;
                    try
                    {
                        DateTime resultedDate = DateTime.ParseExact(dateString, format, provider);
                        Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
                        //app.appDate = result;
                       DateTime utcappDateTime = resultedDate + utcappdatecheck.TimeOfDay;
                        result.appDate = model_appdatetime_DateTime.Date;
                        // result.appDate = utcappDateTime.ToUniversalTime().Date;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("{0} is not in the correct format.", dateString);
                    }
                    //result.appDate = Convert.ToDateTime(model.appDate);
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
                        result.appointmentStatus = "C";
                        db.Entry(result).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        Alert alert = new Alert();
                        alert.alertFor = result.doctorID;
                    alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + tzh.convertTimeZone(oldAppDateTime, 0, result.doctorID, 1) + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, result.doctorID,1);
                    alert.cd = System.DateTime.Now;
                        alert.cb = model.userID;
                        alert.active = true;
                        alert.active = true;
                        db.Alerts.Add(alert);
                        await db.SaveChangesAsync();
                    //Send Email on new appointment
                    if (doctor.docemail != null)
                    {

                        EmailHelper oHelper = new EmailHelper(doctor.docemail, "Reschedule appointment.", "Your appointment on " + formattedDate + " has been rescheduled by patient.");
                        oHelper.SendMessage();
                    }
                    if (patient.patemail != null)
                    {
                        EmailHelper oHelper = new EmailHelper(patient.patemail, "Reschedule appointment.", "Your appointment on " + formattedDate + " is rescheduled successfully.");
                        oHelper.SendMessage();
                    }
                    pushModel pm = new pushModel();
                    pm.DPushTitle = "Reschedule Appointment";
                    pm.DPushMessage = "Your appointment of " + formattedDate + " has been rescheduled successfully by Patient.";
                    pm.sendtoDoctor = true;
                    pm.sendtoPatient = false;
                    pm.doctorID = doctor.doctorID;
                    pm.patientID = patient.patientID;

                    PushHelper ph = new PushHelper();
                    ph.sendPush(pm);


                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = app.appID, message = "" });
                    return response;
                }
                if (model.appType == "P")
                {
                    tempappdate = result.appDate.Value.Date;
                    var formattedDate = string.Format("{0:dd/MM/yyyy}", tempappdate);
                    tempapptime = result.appTime;
                    var formattedTime = DateTime.Now.Date.Add(tempapptime.Value).ToString(@"hh\:mm\:tt");
                    DateTime mydateTime = DateTime.ParseExact(model.appTime,
                                             "hh:mm tt", CultureInfo.InvariantCulture);
                    DateTime utcappdatecheck = mydateTime;
                    var timezoneid = db.Patients.Where(d => d.userId == model.userID).Select(d => d.timezone).FirstOrDefault();
                    TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid.ToString());//need to get zone info from db
                    model_appdatetime_DateTime = TimeZoneInfo.ConvertTimeToUtc(model_appdatetime_DateTime, zoneInfo);

                    result.appTime = model_appdatetime_DateTime.TimeOfDay;
                    //result.appTime = mydateTime.ToUniversalTime().TimeOfDay;
                    //date format start
                    string dateString = model.appDate.Trim();
                  //  string format = "dd/MM/yyyy";
                  //  CultureInfo provider = CultureInfo.InvariantCulture;
                    try
                    {
                        DateTime resultedDate = DateTime.ParseExact(dateString, format, provider);
                        Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
                        //app.appDate = result;
                        DateTime utcappDateTime = resultedDate + utcappdatecheck.TimeOfDay;
                        result.appDate = model_appdatetime_DateTime.Date;
                        //result.appDate = utcappDateTime.ToUniversalTime().Date;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("{0} is not in the correct format.", dateString);
                    }
                    //result.appDate = Convert.ToDateTime(model.appDate);
                    result.mb = model.userID;
                    result.md = System.DateTime.Now;
                    result.appointmentStatus = "C";
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    Alert alert = new Alert();
                    alert.alertFor = result.doctorID;
                    alert.alertText = alert.alertText = ConfigurationManager.AppSettings["AlertPartBeforeDateTime"].ToString() + " " + tzh.convertTimeZone(oldAppDateTime, 0, result.doctorID, 1) + " " + ConfigurationManager.AppSettings["AlertPartBeforeNewDateTime"].ToString() + " " + tzh.convertTimeZone(model_appdatetime_DateTime, 0, result.doctorID, 1);
                    alert.cd = System.DateTime.Now;
                    alert.cb = model.userID;
                    alert.active = true;
                   db.Alerts.Add(alert);
                    await db.SaveChangesAsync();

                    //Send Email on new appointment
                   if (doctor.docemail != null)
                        {
                           
                            EmailHelper oHelper = new EmailHelper(doctor.docemail, "Reschedule appointment.", "Your appointment on " + formattedDate + " has been rescheduled by patient.");
                            oHelper.SendMessage();
                        }
                        if (patient.patemail != null)
                        {
                            EmailHelper oHelper = new EmailHelper(patient.patemail, "Reschedule appointment.", "Your appointment on " + formattedDate + " is rescheduled successfully.");
                            oHelper.SendMessage();
                        }

                        pushModel pm = new pushModel();
                        pm.DPushTitle = "Reschedule Appointment";
                        pm.DPushMessage = "Your appointment of " + formattedDate + " has been rescheduled successfully by Patient.";
                        pm.sendtoDoctor = true;
                        pm.sendtoPatient = false;
                        pm.doctorID = doctor.doctorID;
                        pm.patientID = patient.patientID;

                        PushHelper ph = new PushHelper();
                        ph.sendPush(pm);

                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.appID, message = "" });
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
                return ThrowError(ex, "RescheduleAppointment in AppointmentController.");
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
                    response.ReasonPhrase = "Appointment not found";
                    return response;
                }
                else
                {
                    var patient = (from pp in db.Patients
                                   where pp.patientID == result.patientID
                                   select new { patemail = pp.email, patientID = pp.patientID }).FirstOrDefault();
                    var doctor = (from p in db.Doctors
                                  where p.doctorID == result.doctorID
                                  select new { docemail = p.email, doctorID = p.doctorID }).FirstOrDefault();
                    if (model.appType=="P")
                    {
                        result.rescheduleRequiredBy = "D";
                        result.appointmentStatus = "R";
                        result.mb = model.userID;
                        result.md = System.DateTime.Now;
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
                            //db.Entry(result).State = EntityState.Modified;
                            //await db.SaveChangesAsync();
                            
                        }
                    }

                    DateTime? tempappdate = result.appDate.Value.Date;
                    var formattedDate = string.Format("{0:dd/MM/yyyy}", tempappdate);
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    Alert alert = new Alert();
                    alert.alertFor = result.patientID;
                    alert.alertText = " Doctor has requested to reschedule the appointment.";
                    alert.cd = System.DateTime.Now;
                    alert.md = System.DateTime.Now;
                    alert.mb = model.userID;
                    alert.cb = model.userID;
                    alert.active = true;
                    db.Alerts.Add(alert);
                    await db.SaveChangesAsync();
                    pushModel pm = new pushModel();
                    pm.PPushTitle = "Reschedule Request";
                    pm.PPushMessage = "Doctor has requested for appointment reschedule ";
                    pm.sendtoDoctor = false;
                    pm.sendtoPatient = true;
                    pm.doctorID = doctor.doctorID;
                    pm.patientID = patient.patientID;

                    PushHelper ph = new PushHelper();
                    ph.sendPush(pm);
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
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            //response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.Message, Encoding.Unicode);
            //response.ReasonPhrase = ex.Message;
            //return response;
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = ex.Message + " at " + Action });
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

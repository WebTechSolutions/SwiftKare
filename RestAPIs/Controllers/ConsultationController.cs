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
    public class ConsultationController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        //add consultROS
        //conid,sysitemitemname,sysid,userId

        [HttpPost]
        [Route("api/addconsultROS")]
        public async Task<HttpResponseMessage> AddconsultROS(ConsultROSModel model)
        {
            try
            {
                if (model.userID == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
                    return response;
                }

                if (model.sysitemname == "" || model.sysitemname == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "System item name is required." });
                    return response;
                }
                if (model.sysitemid == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid system item ID." });
                    return response;
                }

                Consultation conres = db.Consultations.Where(a => a.consultID == model.consultID).FirstOrDefault();
                if (conres == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }
                else
                {
                    long consultRosId = 0;

                    //Check if already exists
                    var oConsultRos = db.ConsultationROS.FirstOrDefault
                        (x => x.consultationID == model.consultID && x.systemItemName == model.sysitemname);

                    if (oConsultRos != null)
                    {
                        //If, yes then update existing entry
                        oConsultRos.active = true;
                        db.Entry(oConsultRos).State = EntityState.Modified;

                        consultRosId = oConsultRos.consultationRosID;
                    }
                    else {
                        //Else, add new entry
                        ConsultationRO cons = new ConsultationRO();
                        cons.active = true;
                        cons.cd = System.DateTime.Now;
                        cons.cb = model.userID;
                        cons.systemItemID = model.sysitemid;
                        cons.systemItemName = model.sysitemname;
                        cons.consultationID = model.consultID;
                        db.ConsultationROS.Add(cons);

                        consultRosId = cons.consultationRosID;
                    }

                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = consultRosId, message = "" });
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddconsultROS in ConsultController");
            }
        }

        [HttpPost]
        [Route("api/removeconsultROS")]
        public async Task<HttpResponseMessage> RemoveconsultROS(ConsultROSModel model)
        {
            try
            {
                if (model.userID == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
                    return response;
                }

                if (model.sysitemname == "" || model.sysitemname == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "System item name is required." });
                    return response;
                }
                if (model.sysitemid == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid system item ID." });
                    return response;
                }

                var oConsultRos = db.ConsultationROS.FirstOrDefault(x => x.consultationID == model.consultID && x.systemItemName == model.sysitemname);
                if (oConsultRos != null)
                {
                    oConsultRos.active = false;

                    db.Entry(oConsultRos).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = oConsultRos.consultationRosID, message = "" });
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "" });
                }
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "RemoveConsultROS in ConsultController");
            }
        }

        //
        [Route("api/getConsultationChat")]
        public HttpResponseMessage GetConsultationChat(long consultID)
        {
            try
            {
                var chat = (from c in db.ChatLogs
                                where c.consultID == consultID
                            select new 
                                {
                                    sender = c.sender,
                                reciever = c.reciever,
                                    message=c.message
                                }).ToList();


                response = Request.CreateResponse(HttpStatusCode.OK, chat);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetConsultationChat in ConsultationController");
            }


        }

        [Route("api/getConsultationDetails")]
        public HttpResponseMessage GetConsultationDetails(long consultID)
        {
            try
            {
                //var result = db.SP_GetConsultationDetails(consultID).ToList();
                var result = (from cn in db.Consultations
                              where cn.consultID == consultID && cn.active == true
                              select new
                              {
                                  consultID = cn.consultID,
                                  subjective = cn.subjective,
                                  objective = cn.objective,
                                  assessment = cn.assessment,
                                  plans = cn.plans,
                                  rosItems = (from ros in db.ConsultationROS
                                              where ros.consultationID == cn.consultID && ros.active == true
                                              select new { systemItemName = ros.systemItemName }).ToList(),
                                  AppointmentVM = (from app in db.Appointments
                                                   where app.appID == cn.appID && app.active == true
                                                   select new
                                                   {
                                                       appID = app.appID,
                                                       rov = app.rov,
                                                       chiefComplaints = app.chiefComplaints,
                                                       payment = app.paymentAmt,
                                                       appDate = app.appDate,
                                                       appTime = app.appTime
                                                   }).FirstOrDefault(),
                                  PatientVM = (from r in db.Patients
                                               where r.patientID == cn.patientID
                                               select new
                                               {
                                                   patientID = r.patientID,
                                                   ProfilePhotoBase64 = r.ProfilePhotoBase64,
                                                   patientName = r.firstName + " " + r.lastName,
                                                   patientGender = r.gender,
                                                   pharmacy = r.pharmacy,
                                                   dob = r.dob,
                                                   languages = (from l in db.PatientLanguages
                                                                where l.patientID == r.patientID && l.active == true
                                                                select new { languageName = l.languageName }).ToList()
                                               }).FirstOrDefault(),
                                  DoctorVM = (from doc in db.Doctors
                                              where doc.doctorID == cn.doctorID
                                              select new
                                              {
                                                  doctorID = doc.doctorID,
                                                  ProfilePhotoBase64 = doc.ProfilePhotoBase64,
                                                  doctorName = doc.firstName + " " + doc.lastName,
                                                  doctorGender = doc.gender,
                                                  dob = doc.dob,
                                                  city = doc.city,
                                                  state = doc.state,
                                                  languages = (from l in db.DoctorLanguages
                                                               where l.doctorID == doc.doctorID && l.active == true
                                                               select new { languageName = l.languageName }).ToList(),
                                                  specialities = (from s in db.DoctorSpecialities
                                                                  where s.doctorID == doc.doctorID && s.active == true
                                                                  select new { specialityName = s.specialityName }).ToList()
                                              }).FirstOrDefault(),
                                  AppFiles = (from l in db.UserFiles
                                              where l.active == true && l.AppID == cn.appID
                                              orderby l.fileID descending
                                              select new
                                              {
                                                  fileID = l.fileID,
                                                  FileName = l.FileName.Trim(),
                                                  fileContent = l.fileContent,
                                                  documentType = l.documentType
                                              }).ToList()
                              }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetConsultationDetails in ConsultationController");
            }


        }

        [Route("api/getPatientConsultations")]
        public HttpResponseMessage GetPatientConsultations(long patientID)
        {
            try
            {
                var result = db.SP_GetPatientConsultations(patientID);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientConsultations in ConsultationController");
            }


        }

        [Route("api/getDcotorConsultations")]
        public HttpResponseMessage GetDcotorConsultations(long doctorID)
        {
            try
            {
                var result = db.SP_GetDcotorConsultations(doctorID);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DcotorConsultations in ConsultationController");
            }


        }

        [Route("api/getConsultationROS")]
        public HttpResponseMessage GetROS(long consultID)
        {
            try
            {
                if (consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Invalid consultation ID" });
                    return response;
                }
                else
                {
                    var result = db.SP_GetROS(consultID).ToList();
                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROS in ConsultationController");
            }


        }
        [HttpPost]
        [Route("api/addConsultReview")]
        public async Task<HttpResponseMessage> AddConsultReview(AddConsultReviewodel model)
        {
            try
            {
                if (model.consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.star == 0 || model.star > 5 || model.star < 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid review star." });
                    return response;
                }
                Consultation result = db.Consultations.Where(app => app.consultID == model.consultID && app.active == true).FirstOrDefault();
                if (result == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Consultation not found" });
                    return response;
                }
                else
                {

                    result.mb = db.Patients.Where(p => p.patientID == model.patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault();
                    result.md = System.DateTime.Now;
                    result.review = model.reviewText;
                    result.reviewStar = model.star;
                    db.Entry(result).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    var sampledocEmailBody = @"
                    <h3>Consultation Review</h3>
                    <p>You have been reviewed by patient.</p>
                    <p>"+ model.reviewText + @"</p>
                    <p>&nbsp;</p>
                    <p><strong>Best Regards,<br/>SwiftKare</strong></p>
                    ";
                    var samplepatEmailBody = @"
                    <h3>Consultation Review</h3>
                    <p>You have given review successfully.</p>
                    <p>&nbsp;</p>
                    <p><strong>Best Regards,<br/>SwiftKare</strong></p>
                    ";

                    var docemail = (from d in db.Consultations
                                    where d.consultID == model.consultID
                                    select db.Doctors.Where(doc => doc.doctorID == d.doctorID).Select(doc => doc.email).FirstOrDefault()
                                    ).FirstOrDefault();
                    var oSimpleEmail = new Helper.EmailHelper(docemail.ToString(), "Consultation Review", sampledocEmailBody);
                    oSimpleEmail.SendMessage();

                    var patemail = db.Patients.Where(p => p.patientID == model.patientID).Select(p => p.email).FirstOrDefault();
                    oSimpleEmail = new Helper.EmailHelper(patemail.ToString(), "Consultation Review", samplepatEmailBody);
                    oSimpleEmail.SendMessage();
                }

                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.consultID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddConsultReview in ConsultationController");
            }


        }

        [Route("api/getROSItems")]
        public HttpResponseMessage GetROSItems()
        {
            try
            {
                var rosItems = (from ps in db.PatientSystems
                                where ps.active == true
                                select new ROSItem
                                {
                                    systemID = ps.systemID,
                                    systemName = ps.systemName,
                                    systemItems = (from si in db.SystemItemsses
                                                   where si.systemID == ps.systemID && si.active == true
                                                   select new ROSItemDetail { systemItemID = si.systemItemID, systemItemName = si.systemItemName }).ToList()
                                }).ToList();
                

                response = Request.CreateResponse(HttpStatusCode.OK, rosItems);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetROSItems in ConsultationController");
            }


        }

        [Route("api/getConsultationNotes")]
        public HttpResponseMessage GetConsultationNotes(long consultID)
        {
            try
            {
                var consNotes = (from cn in db.Consultations
                                 where cn.consultID == consultID && cn.active == true
                                 select new { consultID = cn.consultID, subjective = cn.subjective, objective = cn.objective, assessment = cn.assessment, plans = cn.plans }).ToList();

                response = Request.CreateResponse(HttpStatusCode.OK, consNotes);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetConsultationNotes in ConsultationController");
            }
        }

        [Route("api/getConsultationInfo")]
        public HttpResponseMessage GetConsultationInfo(long consultID)
        {
            List<ConsultationRO> ros = new List<ConsultationRO>();
            try
            {
                var consNotes = (from cn in db.Consultations
                                 where cn.consultID == consultID && cn.active == true
                                 select new
                                 {
                                     consultID = cn.consultID,
                                     subjective = cn.subjective,
                                     objective = cn.objective,
                                     assessment = cn.assessment,
                                     plans = cn.plans,
                                     consultTime = cn.duration,
                                     ros = (from r in db.ConsultationROS
                                            where r.consultationID == consultID && r.active == true
                                            select new { id = r.consultationRosID, systemItemID = r.systemItemID, systemItemName = r.systemItemName }).ToList()
                                 }).ToList();

                response = Request.CreateResponse(HttpStatusCode.OK, consNotes);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetConsultationNotes in ConsultationController");
            }


        }
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

        [HttpPost]
        [Route("api/createConsult")]
        public async Task<HttpResponseMessage> CreateConsult(CreateConsultModel model)
        {
            try
            {
                if (model.userID == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
                    return response;
                }

                if (model.sessionID == "" || model.sessionID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Seesion ID is missing." });
                    return response;
                }
                if (model.token == "" || model.token == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Token is missing." });
                    return response;
                }

                Appointment appres = db.Appointments.Where(a => a.appID == model.appID).FirstOrDefault();
                if (appres == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Appointment not found." });
                    return response;
                }
                else
                {
                    Consultation Conres = db.Consultations.Where(a => a.appID == model.appID).FirstOrDefault();
                    if (Conres == null)
                    {
                        Consultation cons = new Consultation();
                        cons.active = true;
                        cons.appID = model.appID;
                        cons.cd = System.DateTime.Now;
                        cons.cb = model.userID;
                        cons.seesionID = model.sessionID;
                        cons.token = model.token;
                        cons.doctorID = model.doctorId;
                        cons.patientID = model.patientId;
                        db.Consultations.Add(cons);
                        await db.SaveChangesAsync();
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = cons.consultID, message = "" });
                    }
                    else {
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = Conres.consultID, message = "" });
                    }
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "CreateConsult in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/createConsultWithoutAppointment")]
        public async Task<HttpResponseMessage> CreateConsultWithoutAppointment(CreateConsultModel model)
        {
            try
            {
                if (model.userID == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid user ID." });
                    return response;
                }

                if (model.sessionID == "" || model.sessionID == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Seesion ID is missing." });
                    return response;
                }
                if (model.token == "" || model.token == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Token is missing." });
                    return response;
                }

                Consultation cons = new Consultation();
                cons.active = true;
                cons.appID = null;

                cons.doctorID = model.doctorId;
                cons.patientID = model.patientId;

                cons.cd = System.DateTime.Now;
                cons.cb = model.userID;
                cons.seesionID = model.sessionID;
                cons.token = model.token;
                db.Consultations.Add(cons);
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = cons.consultID, message = "" });
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "CreateConsult in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/addConsultStartTime")]
        public async Task<HttpResponseMessage> AddConsultStartTime(AddConsultTimeModel model)
        {
            try
            {
                if (model.consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consult ID." });
                    return response;
                }

                if (model.userEmail == "" || model.userEmail == null || !(IsValid(model.userEmail)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for user." });
                    return response;
                }

                Consultation cons = db.Consultations.Where(c => c.consultID == model.consultID && c.active == true).FirstOrDefault();
                if (cons != null)
                {
                    cons.mb = model.userEmail;
                    cons.md = System.DateTime.Now;
                    cons.startTime = TimeSpan.Parse(System.DateTime.Now.ToString("HH:mm"));
                    db.Entry(cons).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = cons.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddConsultStartTime in ConsultController");
            }
        }

        [HttpPost]
        [Route("api/addConsultEndTime")]
        public async Task<HttpResponseMessage> AddConsultEndTime(AddConsultTimeModel model)
        {
            try
            {
                if (model.consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consult ID." });
                    return response;
                }

                if (model.userEmail == "" || model.userEmail == null || !(IsValid(model.userEmail)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for user." });
                    return response;
                }

                Consultation cons = db.Consultations.Where(c => c.consultID == model.consultID && c.active == true).FirstOrDefault();
                if (cons != null)
                {
                    cons.mb = model.userEmail;
                    cons.md = System.DateTime.Now;

                    cons.endTime = TimeSpan.Parse(System.DateTime.Now.ToString("HH:mm"));
                    if (cons.startTime ==null) cons.endTime = TimeSpan.Parse(System.DateTime.Now.ToString("HH:mm"));
                    TimeSpan st = TimeSpan.Parse(cons.startTime.ToString());
                    TimeSpan et = TimeSpan.Parse(cons.endTime.ToString());
                    TimeSpan duration = et.Subtract(st);
                    cons.duration = Convert.ToInt32(duration.TotalSeconds);
                    
                    cons.endby = model.userEmail;
                    cons.status = "C";
                    db.Entry(cons).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #region 
                    var sampleEmailBody = @"
                    <h3>Consultation Completed</h3>
                    <p>Your consultation is completed successfully.</p>
                    <p>&nbsp;</p>
                    <p><strong>Best Regards,<br/>SwiftKare</strong></p>
                    ";
                    
                    var docemail = (from d in db.Consultations
                                    where d.consultID == cons.consultID
                                    select db.Doctors.Where(doc => doc.doctorID == d.doctorID).Select(doc => doc.email).FirstOrDefault()
                                    ).FirstOrDefault();
                    var oSimpleEmail = new Helper.EmailHelper(docemail.ToString(), "Consultation Completed", sampleEmailBody);
                    oSimpleEmail.SendMessage();

                    var patemail = (from p in db.Consultations
                                    where p.consultID == cons.consultID
                                    select db.Patients.Where(pat => pat.patientID == p.patientID).Select(pat => pat.email).FirstOrDefault()
                                    ).FirstOrDefault();
                    oSimpleEmail = new Helper.EmailHelper(patemail.ToString(), "Consultation Completed", sampleEmailBody);
                    oSimpleEmail.SendMessage();
                    #endregion

                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = cons.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddConsultEndTime in ConsultController");
            }
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
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

        [HttpPost]
        [Route("api/addDoctorNotes")]
        public async Task<HttpResponseMessage> AddDoctorNotes(DoctorNotes model)
        {
            try
            {
                if (model.consutID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }

                if (model.subjective == "" || model.subjective == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide subject details." });
                    return response;
                }
                if (model.objective == "" || model.objective == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide object details." });
                    return response;
                }
                if (model.assessment == "" || model.assessment == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide assessment details." });
                    return response;
                }
                if (model.plans == "" || model.plans == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide plans details." });
                    return response;
                }
                if (model.userEmail == "" || model.userEmail == null || !(IsValid(model.userEmail)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email address of doctor." });
                    return response;
                }
                Consultation conss = db.Consultations.Where(c => c.consultID == model.consutID && c.active == true).FirstOrDefault();
                if (conss != null)
                {
                    conss.mb = model.userEmail;
                    conss.md = System.DateTime.Now;
                    conss.subjective = model.subjective;
                    conss.objective = model.objective;
                    conss.assessment = model.assessment;
                    conss.plans = model.plans;
                    db.Entry(conss).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conss.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "addDoctorNotes in ConsultController");
            }
        }



        [HttpPost]
        [Route("api/addDoctorNotesSubjective")]
        public async Task<HttpResponseMessage> AddDoctorNotesSubjective(long consultId, string subjective)
        {
            try
            {
                if (consultId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }

                if (subjective == "" || subjective == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide subject details." });
                    return response;
                }

                Consultation conss = db.Consultations.Where(c => c.consultID == consultId && c.active == true).FirstOrDefault();
                if (conss != null)
                {
                    conss.md = System.DateTime.Now;
                    conss.subjective = subjective;
                    db.Entry(conss).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conss.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "addDoctorNotesSubjective in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/addDoctorNotesObjective")]
        public async Task<HttpResponseMessage> AddDoctorNotesObjective(long consultId, string objective)
        {
            try
            {
                if (consultId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }

                if (objective == "" || objective == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide subject details." });
                    return response;
                }

                Consultation conss = db.Consultations.Where(c => c.consultID == consultId && c.active == true).FirstOrDefault();
                if (conss != null)
                {
                    conss.md = System.DateTime.Now;
                    conss.objective = objective;
                    db.Entry(conss).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conss.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "addDoctorNotesObjective in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/addDoctorNotesAssessment")]
        public async Task<HttpResponseMessage> AddDoctorNotesAssessment(long consultId, string assessment)
        {
            try
            {
                if (consultId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }

                if (assessment == "" || assessment == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide subject details." });
                    return response;
                }

                Consultation conss = db.Consultations.Where(c => c.consultID == consultId && c.active == true).FirstOrDefault();
                if (conss != null)
                {
                    conss.md = System.DateTime.Now;
                    conss.assessment = assessment;
                    db.Entry(conss).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conss.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "addDoctorNotesAssessment in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/addDoctorNotesPlans")]
        public async Task<HttpResponseMessage> AddDoctorNotesPlans(long consultId, string plans)
        {
            try
            {
                if (consultId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consultation ID." });
                    return response;
                }

                if (plans == "" || plans == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide subject details." });
                    return response;
                }

                Consultation conss = db.Consultations.Where(c => c.consultID == consultId && c.active == true).FirstOrDefault();
                if (conss != null)
                {
                    conss.md = System.DateTime.Now;
                    conss.plans = plans;
                    db.Entry(conss).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conss.consultID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Consultation not found." });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "addDoctorNotesPlans in ConsultController");
            }
        }


        [HttpPost]
        [Route("api/CompleteConsultByPatient")]
        public async Task<HttpResponseMessage> CompleteConsultByPatient(CompleteConsultPatient model)
        {
            try
            {
                Consultation con = db.Consultations.Where(app => app.appID == model.appID && app.active == true).FirstOrDefault();
                if (con == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Consultation not found" });
                    return response;
                }

                else
                {
                    con.status = "C";
                    con.mb = model.userID;
                    con.md = System.DateTime.Now;
                    db.Entry(con).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }

                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.appID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "CompleteConsultByPatient in ConsultController");
            }


        }
        [HttpPost]
        [Route("api/CompleteConsultByDoctor")]
        public async Task<HttpResponseMessage> CompleteConsultByDoctor(CompleteConsultDoctor model)
        {
            try
            {
                Consultation con = db.Consultations.Where(app => app.appID == model.appID && app.active == true).FirstOrDefault();
                if (con == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Consultation not found" });
                    return response;
                }

                else
                {
                    con.status = "C";
                    con.mb = model.userID;
                    con.md = System.DateTime.Now;
                    db.Entry(con).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }

                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.appID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "CompleteConsultByDoctor in ConsultController");
            }


        }

    }
}

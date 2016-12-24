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
                                                         cheifcomplaints = app.chiefComplaints,
                                                         payment = app.paymentAmt,
                                                         appDate = app.appDate,
                                                         appTime = app.appTime
                                                     }).FirstOrDefault(),
                                      PatientVM = (from r in db.Patients
                                                 where r.patientID == cn.patientID && r.active == true
                                                 select new
                                                 {
                                                     patientID = r.patientID,
                                                     PatPicture = r.picture,
                                                     patientName = r.firstName + " " + r.lastName,
                                                     patientGender = r.gender,
                                                     pharmacy = r.pharmacy,
                                                     dob = r.dob,
                                                     languages = (from l in db.PatientLanguages
                                                                  where l.patientID == r.patientID && l.active == true
                                                                  select new { languageName = l.languageName }).ToList()
                                                 }).FirstOrDefault(),
                                      DoctorVM = (from doc in db.Doctors
                                                where doc.doctorID == cn.doctorID && doc.active == true
                                                select new
                                                {
                                                    doctorID = doc.doctorID,
                                                    docPicture = doc.picture,
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
                                                }).FirstOrDefault()
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
                if (model.patientID == 0 )
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.star == 0  || model.star > 5 || model.star < 0)
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
                                select new {
                                    systemID= ps.systemID,
                                    systemName=ps.systemName,
                                    systemItems = (from si in db.SystemItemsses
                                        where si.systemID == ps.systemID && si.active == true
                                        select new { systemItemID = si.systemItemID, systemItemName = si.systemItemName }).ToList()
                                }).ToList();
                //select db.SystemItemsses.Where(si => si.systemID == ps.systemID && si.active == true).ToList());
                //(from si in db.SystemItemsses
                // where si.systemID == si.systemID && si.active == true
                // select new { systemItemID = si.systemItemID, systemItemName = si.systemItemName}).ToList()
                // );

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
                                 where cn.consultID==consultID && cn.active == true
                                 select new { consultID = cn.consultID, subjective = cn.subjective ,objective=cn.objective,assessment=cn.assessment,plans=cn.plans}).ToList();

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
                                 select new { consultID = cn.consultID, subjective = cn.subjective, objective = cn.objective, assessment = cn.assessment, plans = cn.plans,
                                              consultTime=cn.duration,
                                     ros = (from r in db.ConsultationROS where r.consultationID==consultID && r.active==true
                                            select new {id= r.consultationRosID, systemItemID= r.systemItemID, systemItemName= r.systemItemName }).ToList()
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
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
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
               
                //var email = (from d in db.Doctors
                //                where d.doctorID == model.doctorID && d.active == true
                //                select d.email).FirstOrDefault();
                Consultation cons = new Consultation();
                cons.active = true;
                cons.cd = System.DateTime.Now;
                cons.cb = db.Patients.Where(p => p.patientID == model.patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault();
                cons.doctorID = model.doctorID;
                cons.patientID = model.patientID;
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
                    TimeSpan st = TimeSpan.Parse(cons.startTime.ToString());
                    TimeSpan et = TimeSpan.Parse(cons.endTime.ToString());
                    TimeSpan duration=et.Subtract(st);
                    cons.duration = duration.Minutes;
                    cons.endby = model.userEmail;
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
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
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

    }
}

using DataAccess;
using DataAccess.CustomModels;
using Identity.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class ProfilesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [HttpGet]
        [Route("api/getSecretQuestionList")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetSecretQuestionList()
        {

            
            try
            {
                var secretquests = (from sq in db.SecretQuestions where sq.active == true select new { sq.secretQuestionID, sq.secretQuestionn }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, secretquests);
                return response;
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSecretQuestionList in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/getTimeZones")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetTimeZones()
        {


            try
            {
                var timezones = (from tz in db.TimeZones where tz.active == true select new { tz.zoneID, tz.timeZonee }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, timezones);
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetTimeZones in ProfilesController.");
            }

        }


        [HttpGet]
        [Route("api/getCities")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetCities()
        {


            try
            {
                var cities = (from c in db.Cities where c.active == true select new { c.cityID, c.cityName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, cities);
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetCities in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/getStates")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetStates()
        {


            try
            {
                var states = (from c in db.States where c.active == true select new { c.stateID, c.stateName }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, states);
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetStates in ProfilesController.");
            }

        }

        //Doctor Profile Section
        [Route("api/updateDoctorPicture")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorPicture(UpdateDoctorPicture model)
        {
            Doctor doctor = new Doctor();
            try
            {
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.picture == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid picture." });
                    return response;
                }

                //check for duplicate names
                doctor = db.Doctors.Where(m => m.doctorID == model.doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor not found." });
                    return response;
                }

                else
                {
                    doctor.picture = model.picture;
                    doctor.md = System.DateTime.Now;
                    doctor.mb = model.doctorID.ToString();
                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.doctorID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateDoctorPicture in DoctorController.");
            }

        }

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
        [Route("api/changePassword")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> ChangePassword(DoctorPasswordModel model)
        { 
           
            try
            {
                
                string userID = (from d in db.Doctors where d.active == true && d.doctorID == model.doctorID select d.userId).FirstOrDefault();
                var user = await UserManager.FindByIdAsync(userID);
                var code = UserManager.GeneratePasswordResetToken(user.Id);

                if (user != null)
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, code, model.password);
                    if (result.Succeeded)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.doctorID, message = "" });
                        return response;
                    }
                    else
                    {
                        string msg = "";
                        foreach (var error in result.Errors)
                        {
                            msg = error;
                        }
                        response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = msg });
                        return response;
                    }

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Doctor not found" });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ChangePassword in DoctorController.");
            }

        }

        [Route("api/updateConsultCharges")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateConsultCharges(UpdateConsultCharges model)
        {
            Doctor doctor = new Doctor();
            try
            {
                doctor = db.Doctors.Where(m => m.doctorID == model.doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor not found." });
                    return response;
                }

                else
                {
                    doctor.consultCharges = model.consultCharges;
                    doctor.md = System.DateTime.Now;
                    doctor.mb = model.doctorID.ToString();
                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.doctorID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateConsultCharges in DoctorController.");
            }

        }

        [Route("api/updateDoctorSecretAnswers")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddDoctorSecretAnswers(UpdateSecretQuestions model)
        {
            Doctor doctor = new Doctor();
            try
            {
                
                if (model.secretquestion1 == null || model.secretquestion1 == "" )
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret question 1 is not provided." });
                    return response;
                }
                if (model.secretquestion2 == null || model.secretquestion2 == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret question 2 is not provided." });
                    return response;
                }
                if (model.secretquestion3 == null || model.secretquestion3 == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret question 3 is not provided." });
                    return response;
                }

                if (model.secretanswer1 == null || model.secretanswer1 == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret answer 1 is not provided." });
                    return response;
                }
                if (model.secretanswer2 == null || model.secretanswer2 == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret answer 2 is not provided." });
                    return response;
                }
                if (model.secretanswer3 == null || model.secretanswer3 == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Secret answer 3 is not provided." });
                    return response;
                }

                doctor = (from doc in db.Doctors where doc.doctorID == model.doctorID && doc.active == true select doc).FirstOrDefault();//db.Doctors.Where(m => m.doctorID == model.doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor not found." });
                    return response;
                }

                else
                {
                    doctor.secretQuestion1 = model.secretquestion1;
                    doctor.secretQuestion2 = model.secretquestion2;
                    doctor.secretQuestion3 = model.secretquestion3;
                    doctor.secretQuestion1 = model.secretquestion1;
                    doctor.secretAnswer1 = model.secretanswer1;
                    doctor.secretAnswer2 = model.secretanswer2;
                    doctor.secretAnswer3 = model.secretanswer3;
                    doctor.md = System.DateTime.Now;
                    doctor.mb = model.doctorID.ToString();
                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.doctorID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddDoctorSecretAnswers in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/updateDoctorProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorProfile(long doctorID, UpdateDoctorProfileModel model)
        {

            Doctor doctor = new Doctor();
            try
            {

                if (model.firstName == null || model.firstName == "" || !Regex.IsMatch(model.firstName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "First name is not valid. Only letter and numbers are allowed." });
                    return response;
                }
                if (model.lastName != null || model.lastName != "")
                {
                    if (!Regex.IsMatch(model.lastName, "^[0-9a-zA-Z ]+$"))            //@"^[a-zA-Z\s]+$"
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Last name is not valid. Only letter and numbers are allowed." });
                        return response;
                    }
                }
                if (doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor ID is not valid." });
                    return response;
                }
                doctor = db.Doctors.Where(m => m.doctorID == doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor not found." });
                    return response;
                }
                if (model.zip.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Zip is too long. Keep it below ten characters." });
                    return response;
                }
                if (model.gender.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Gender is too long. Keep it below ten characters." });
                    return response;
                }
                else
                {

                    doctor.active = true;
                    doctor.firstName = model.firstName;
                    doctor.lastName = model.lastName;
                    doctor.cd = System.DateTime.Now;
                    doctor.homePhone = model.homePhone;
                    doctor.cellPhone = model.cellPhone;
                    doctor.address1 = model.address1;
                    doctor.address2 = model.address2;
                    doctor.gender = model.gender;
                    doctor.dob = model.dob;
                    doctor.picture = model.picture;
                    doctor.aboutMe = model.aboutMe;
                    doctor.city = model.city;
                    doctor.publication = model.publication;
                    doctor.state = model.state;
                    doctor.zip = model.zip;
                    doctor.education = model.education;
                    doctor.timezone = model.timezone;
                    doctor.specialization = model.specialization;
                    doctor.suffix = model.suffix;
                    doctor.title = model.title;
                    doctor.workexperience = model.workExperience;
                    doctor.consultCharges = model.consultCharges;
                    doctor.mb = doctorID.ToString();
                    doctor.md = System.DateTime.Now;
                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doctorID, message = "" });
                    return response;


                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateDoctorProfile in ProfileController.");
            }

        }
        [HttpGet]
        [Route("api/getDoctorProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetDoctorProfile(long doctorID)
        {

            Doctor doctor = new Doctor();
            try
            {

                if (doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor ID is not valid." });
                    return response;
                }
                doctor = db.Doctors.Where(m => m.doctorID == doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor does not exist." });
                    return response;
                }
                else
                {
                    var profile = (from l in db.Doctors
                                   where l.active == true && l.doctorID == doctorID
                                   select new DoctorProfileModel
                                   {
                                       firstName = l.firstName,
                                       lastName = l.lastName,
                                       gender = l.gender.Trim(),
                                       address1 = l.address1.Trim(),
                                       address2 = l.address2.Trim(),
                                       cellPhone = l.cellPhone,
                                       homePhone = l.homePhone,
                                       city = l.city,
                                       dob = l.dob,
                                       picture = l.picture,
                                       title = l.title,
                                       suffix = l.suffix,
                                       timezone = l.timezone,
                                       state = l.state,
                                       zip = l.zip,
                                       licensedState=db.DoctorLicenseStates.Where(lic=>lic.doctorID==doctorID).ToList(),
                                   }).FirstOrDefault();
                    response = Request.CreateResponse(HttpStatusCode.OK, profile);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorProfile in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/viewDoctorProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage ViewDoctorProfile(long doctorID)
        {

            Doctor doctor = new Doctor();
            try
            {

                if (doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor ID is not valid." });
                    return response;
                }
                doctor = db.Doctors.Where(m => m.doctorID == doctorID && m.active == true).FirstOrDefault();
                if (doctor == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor does not exist." });
                    return response;
                }
                else
                {
                    var profile = (from l in db.Doctors
                                   where l.active == true && l.doctorID == doctorID
                                   select new DoctorProfileModel
                                   {
                                       firstName = l.firstName,
                                       lastName = l.lastName,
                                       gender = l.gender.Trim(),
                                       address1 = l.address1.Trim(),
                                       address2 = l.address2.Trim(),
                                       cellPhone = l.cellPhone,
                                       homePhone = l.homePhone,
                                       city = l.city,
                                       dob = l.dob,
                                       picture = l.picture,
                                       title = l.title,
                                       suffix = l.suffix,
                                       timezone = l.timezone,
                                       state = l.state,
                                       zip = l.zip,
                                       licensedState = db.DoctorLicenseStates.Where(lic => lic.doctorID == doctorID).ToList(),
                                   }).FirstOrDefault();
                    response = Request.CreateResponse(HttpStatusCode.OK, profile);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ViewDoctorProfile in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/viewPatientProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage ViewPatientProfile(long patientID)
        {

            Patient patient = new Patient();
            try
            {

                if (patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor ID is not valid." });
                    return response;
                }
                patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient does not exist." });
                    return response;
                }
                else
                {
                    var patprofile = db.SP_ViewPatientProfile(patientID).ToList();
                    //var profile = (from l in db.Patients
                    //               where l.active == true && l.patientID == patientID

                    //               select new ViewPatientProfile
                    //               {
                    //                   firstName = l.firstName,
                    //                   lastName = l.lastName,
                    //                   gender = l.gender.Trim(),
                    //                   cellPhone = l.cellPhone,
                    //                   dob = l.dob,
                    //                   picture = l.picture,
                    //                   age = 12,
                    //                   //patallergy=db.PatientAllergies.Where(pall=>pall.active==true && pall.patientID==l.patientID).ToList(),
                    //                   //patcond= db.Conditions.Where(cond => cond.active == true && cond.patientID == l.patientID).ToList(),
                    //                   //patfamilyhx=db.PatientFamilyHXes.Where(f => f.active == true && f.patientID == l.patientID).ToList(),
                    //                   //patlang=db.PatientLanguages.Where(patl => patl.active == true && patl.patientID == patientID).ToList(),
                    //                   //patmedication = db.Medications.Where(med => med.active == true && med.patientId == patientID).ToList(),
                    //                   //patsurgery= db.PatientSurgeries.Where(surg => surg.active == true && surg.patientID == patientID).ToList(),
                    //                   title=l.title,
                    //                   suffix=l.suffix,
                    //                   zip = l.zip,
                    //                  }).FirstOrDefault();
                    response = Request.CreateResponse(HttpStatusCode.OK, patprofile);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorProfile in ProfilesController.");
            }

        }


        //Patient Profile Section

        [HttpPost]
        [Route("api/updatePatientProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdatePatientProfile(long patientID, PatientProfileModel model)
        {

            Patient patient = new Patient();
            try
            {

                if (model.firstName == null || model.firstName == "" || !Regex.IsMatch(model.firstName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "First name is not valid. Only letter and numbers are allowed." });
                    return response;
                }
                if (model.lastName != null || model.lastName != "")
                {
                    if (!Regex.IsMatch(model.lastName, "^[0-9a-zA-Z ]+$"))                    //@"^[a-zA-Z\s]+$"
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Last name is not valid. Only letter and numbers are allowed." });
                        return response;
                    }
                }
                if (patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
                    return response;
                }
               
                patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient not found." });
                    return response;
                }
                else
                {
                    patient.active = true;
                    patient.firstName = model.firstName;
                    patient.lastName = model.lastName;
                    patient.cd = System.DateTime.Now;
                    patient.homePhone = model.homePhone;
                    patient.cellPhone = model.cellPhone;
                    patient.address1 = model.address1;
                    patient.address2 = model.address2;
                    patient.gender = model.gender;
                    patient.dob = model.dob;
                    patient.picture = model.picture;
                    patient.timezone = model.timezone;
                    patient.city = model.city;
                    patient.suffix = model.suffix;
                    patient.title = model.title;
                    patient.height = model.height;
                    patient.weight = model.weight;
                    patient.mb = patientID.ToString();
                    patient.md = System.DateTime.Now;
                    db.Entry(patient).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patient.patientID, message = "" });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientProfile in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/getPatientProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetPatientProfile(long patientID)
        {

            Patient patient = new Patient();
            try
            {

                if (patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
                    return response;
                }
                patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient does not exist." });
                    return response;
                }
                else
                {
                    var profile = (from l in db.Patients
                                   where l.active == true && l.patientID == patientID
                                   select new PatientProfileModel
                                   {
                                       firstName = l.firstName,
                                       lastName = l.lastName,
                                       gender = l.gender.Trim(),
                                       address1 = l.address1.Trim(),
                                       address2 = l.address2.Trim(),
                                       cellPhone = l.cellPhone,
                                       homePhone = l.homePhone,
                                       city = l.city,
                                       dob = l.dob,
                                       height = l.height,
                                       weight = l.weight,
                                       picture = l.picture,
                                       title = l.title,
                                       suffix = l.suffix,
                                       timezone = l.timezone,
                                       state = l.state,
                                       zip = l.zip
                                   }).FirstOrDefault();
                    response = Request.CreateResponse(HttpStatusCode.OK, profile);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientProfile in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/updatePatientPicture")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdatePatientPicture(UpdatePatientPicture model)
        {
            Patient patient = new Patient();
            try
            {
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.picture == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid picture." });
                    return response;
                }

                //check for duplicate names
                patient = db.Patients.Where(m => m.patientID == model.patientID && m.active == true).FirstOrDefault();
                if (patient == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient not found." });
                    return response;
                }

                else
                {
                    patient.picture = model.picture;
                    patient.md = System.DateTime.Now;
                    patient.mb = model.patientID.ToString();
                    db.Entry(patient).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.patientID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientPicture in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/updatePatientLanguages")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdatePatientLanguages(long patlangID ,PatientLanguages model)
        {
            PatientLanguage patlang = new PatientLanguage();
            try
            {
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.languageName == null || model.languageName == "" || !Regex.IsMatch(model.languageName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language is not valid. Only letter and numbers are allowed." });
                    return response;
                }

                patlang = db.PatientLanguages.Where(m => m.patientLanguageID == patlangID && m.active == true).FirstOrDefault();
                if (patlang == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language not found." });
                    return response;
                }

                else
                {
                    patlang.languageName = model.languageName;
                    patlang.md = System.DateTime.Now;
                    patlang.mb = model.patientID.ToString();
                    db.Entry(patlang).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.patientID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientLanguages in ProfilesController.");
            }

        }

        [Route("api/insertPatientLanguages")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> InsertPatientLanguages(PatientLanguages model)
        {
            PatientLanguage patlang = new PatientLanguage();
            try
            {
                if (model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.languageName == null || model.languageName == "" || !Regex.IsMatch(model.languageName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language is not valid. Only letter and numbers are allowed." });
                    return response;
                }

                patlang = db.PatientLanguages.Where(m => m.patientID == model.patientID && m.languageName == model.languageName && m.active == true).FirstOrDefault();
                if (patlang != null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language already exists." });
                    return response;
                }
               
                else
                {
                    patlang = new PatientLanguage();
                    patlang.active = true;
                    patlang.languageName = model.languageName;
                    patlang.patientID = model.patientID;
                    patlang.cd = System.DateTime.Now;
                    patlang.cb = model.patientID.ToString();
                    db.PatientLanguages.Add(patlang);
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patlang.patientLanguageID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientLanguages in ProfilesController.");
            }

        }

        [Route("api/deletePatientLanguages")]
        public async Task<HttpResponseMessage> DeletePatientLanguages(long langID)
        {
            try
            {
                Patient patient = new Patient();
               
                PatientLanguage patlang = db.PatientLanguages.Where(lang => lang.patientLanguageID == langID && lang.active == true).FirstOrDefault();
                if (patlang != null) { patient = await db.Patients.FindAsync(patlang.patientID); }
                if (patlang == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language not found." });
                    return response;
                }
                patlang.active = false;//Delete Operation changed
                patlang.mb = patlang.patientID.ToString();
                patlang.md = System.DateTime.Now;
                db.Entry(patlang).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientLanguages in ProfilesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = langID, message = "" });
            return response;
        }

        [HttpPost]
        [Route("api/updateDoctorLanguages")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorLanguages(long doclangID, DoctorLanguages model)
        {
            DoctorLanguage doclang = new DoctorLanguage();
            try
            {
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.languageName == null || model.languageName == "" || !Regex.IsMatch(model.languageName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language is not valid. Only letter and numbers are allowed." });
                    return response;
                }

                doclang = db.DoctorLanguages.Where(m => m.doctorLanguageID == doclangID && m.active == true).FirstOrDefault();
                if (doclang == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language not found." });
                    return response;
                }

                else
                {
                    doclang.languageName = model.languageName;
                    doclang.md = System.DateTime.Now;
                    doclang.mb = model.doctorID.ToString();
                    db.Entry(doclang).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doclangID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateDoctorLanguages in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/insertDoctorLanguages")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> InsertDoctorLanguages(DoctorLanguages model)
        {
            DoctorLanguage doclang = new DoctorLanguage();
            try
            {
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.languageName == null || model.languageName == "" || !Regex.IsMatch(model.languageName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language is not valid. Only letter and numbers are allowed." });
                    return response;
                }

                doclang = db.DoctorLanguages.Where(m => m.doctorID == model.doctorID && m.languageName == model.languageName && m.active == true).FirstOrDefault();
                if (doclang != null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language already exists." });
                    return response;
                }

                else
                {
                    doclang = new DoctorLanguage();
                    doclang.languageName = model.languageName;
                    doclang.active = true;
                    doclang.doctorID = model.doctorID;
                    doclang.cd = System.DateTime.Now;
                    doclang.cb = model.doctorID.ToString();
                    db.DoctorLanguages.Add(doclang);
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doclang.doctorLanguageID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientLanguages in ProfilesController.");
            }

        }

        [Route("api/deleteDoctorLanguages")]
        public async Task<HttpResponseMessage> DeleteDoctorLanguages(long langID)
        {
            try
            {
                Doctor doc = new Doctor();

               DoctorLanguage doclang = db.DoctorLanguages.Where(lang => lang.doctorLanguageID == langID && lang.active == true).FirstOrDefault();
                if (doclang != null) { doc = await db.Doctors.FindAsync(doclang.doctorID); }
                if (doclang == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Language not found." });
                    return response;
                }
                doclang.active = false;//Delete Operation changed
                doclang.mb = doclang.doctorID.ToString();
                doclang.md = System.DateTime.Now;
                db.Entry(doclang).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeleteDoctorLanguages in ProfilesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = langID, message = "" });
            return response;
        }

        [HttpPost]
        [Route("api/insertDoctorLicensedStates")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> InsertDoctorLicensedStates(DoctorLicStatesModel model)
        {
            DoctorLicenseState doclic = new DoctorLicenseState();
            try
            {
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                if (model.licstateName == null || model.licstateName == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Licensed state name is not valid." });
                    return response;
                }

                doclic = db.DoctorLicenseStates.Where(m => m.doctorID == model.doctorID && m.stateName == model.licstateName && m.active == true).FirstOrDefault();
                if (doclic != null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Licensed state already exists." });
                    return response;
                }

                else
                {
                    doclic = new DoctorLicenseState();
                    doclic.active = true;
                    doclic.stateName = model.licstateName;
                    doclic.doctorID = model.doctorID;
                    doclic.cd = System.DateTime.Now;
                    doclic.cb = model.doctorID.ToString();
                    db.DoctorLicenseStates.Add(doclic);
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doclic.doctorLicenseStateID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientLanguages in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/updateDoctorLicensedStates")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorLicensedStates(long licstateID, DoctorLicStatesModel model)
        {
            DoctorLicenseState doclicst = new DoctorLicenseState();
            try
            {
                if (model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }
                if (model.licstateName == null || model.licstateName == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide state name." });
                    return response;
                }

                doclicst = db.DoctorLicenseStates.Where(m => m.doctorLicenseStateID != licstateID && m.doctorID==model.doctorID && m.active == true && m.stateName==
                model.licstateName).FirstOrDefault();
                if (doclicst != null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "State already exists." });
                    return response;
                }
                doclicst = db.DoctorLicenseStates.Where(m => m.doctorLicenseStateID == licstateID && m.active == true).FirstOrDefault();
                if (doclicst == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "State not found." });
                    return response;
                }

                else
                {
                    doclicst.stateName = model.licstateName;
                    doclicst.md = System.DateTime.Now;
                    doclicst.mb = model.doctorID.ToString();
                    db.Entry(doclicst).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = licstateID, message = "" });
                    return response;

                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientLanguages in ProfilesController.");
            }

        }

        [Route("api/deleteDoctorLicensedStates")]
        public async Task<HttpResponseMessage> DeleteDoctorLicensedStates(long lsID)
        {
            try
            {
                Doctor doctor = new Doctor();

                DoctorLicenseState docls = db.DoctorLicenseStates.Where(lics => lics.doctorLicenseStateID == lsID && lics.active == true).FirstOrDefault();
                if (docls != null) { doctor = await db.Doctors.FindAsync(docls.doctorID); }
                if (docls == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Licensed state not found." });
                    return response;
                }
                docls.active = false;//Delete Operation changed
                docls.mb = docls.doctorID.ToString();
                docls.md = System.DateTime.Now;
                db.Entry(docls).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeleteDoctorLicensedStates in ProfilesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = lsID, message = "" });
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
            return response;
        }


    }
}

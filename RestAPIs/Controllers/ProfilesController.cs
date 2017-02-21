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
using System.Text;
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

        [Route("api/UpdateTimezone")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateTimezone(TimezoneModel model)
        {

            try
            {
                if (model.timezone == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Timezone is not provided." });
                    response.ReasonPhrase = "Timezone is not provided.";
                    return response;
                }
                if (model.userid == null)
                {

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "UserId is not provided." });
                    response.ReasonPhrase = "UserId is not provided.";
                    return response;
                }
                Patient pat = db.Patients.Where(p=>p.userId==model.userid).FirstOrDefault();
                Doctor doc = db.Doctors.Where(p => p.userId == model.userid).FirstOrDefault();
                long id = 0;
                if (pat!=null)
                {
                    pat.timezone = model.timezone;
                    id = pat.patientID;
                    db.Entry(pat).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
               
                if (doc!= null)
                {

                    doc.timezone = model.timezone;
                    id = doc.doctorID;
                    db.Entry(doc).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = id, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateTimezone in ProfilesController.");
            }

        }
        #region Doctor APIs 

        [HttpGet]
        [Route("api/getDoctorProfileInitialValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetDoctorProfileInitialValues()
        {
            try
            {
                var lstSpecialityVM = (from p in db.Speciallities
                                       where p.active == true
                                       select new SpecialityVM { specialityID = p.speciallityID, specialityName = p.specialityName }).ToList();

                var lstLanguageVM = (from p in db.Languages
                                     where p.active == true
                                     select new LanguageVM { languageID = p.languageID, languageName = p.languageName }).ToList();

                var lstSecretQuestionVM = (from p in db.SecretQuestions
                                           where p.active == true
                                           select new SecretQuestionVM { secretQuestionID = p.secretQuestionID, secretQuestion = p.secretQuestionn }).ToList();

                var lstTimeZoneVM = (from p in db.TimeZones
                                     where p.active == true
                                     select new TimeZoneVM { zoneID = p.zoneID, timeZone = p.timeZonee, zoneName = p.zoneName }).ToList();

                var lstCityVM = (from c in db.Cities
                                 where c.active == true
                                 select new CityVM { cityID = c.cityID, cityName = c.cityName }).ToList();

                var lstStateVM = (from c in db.States
                                  where c.active == true
                                  select new StateVM { stateID = c.stateID, stateName = c.stateName }).ToList();

                var lstTitleVM = (from p in db.TitleMasters
                                  where p.active == true
                                  select new TitleVM { titleId = p.titleID, titleName = p.titleName }).ToList();

                var lstSuffixVM = (from p in db.SuffixMasters
                                   where p.active == true
                                   select new SuffixVM { suffixId = p.SuffixID, suffixName = p.SuffixName }).ToList();


                var oRetModel = new DoctorProfileInitialValues
                {
                    lstCityVM = lstCityVM,
                    lstLanguageVM = lstLanguageVM,
                    lstSecretQuestionVM = lstSecretQuestionVM,
                    lstSpecialityVM = lstSpecialityVM,
                    lstStateVM = lstStateVM,
                    lstTimeZoneVM = lstTimeZoneVM,
                    lstTitleVM = lstTitleVM,
                    lstSuffixVM = lstSuffixVM
                };

                response = Request.CreateResponse(HttpStatusCode.OK, oRetModel);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorProfileInitialValues in ProfilesController.");
            }

        }

        [HttpGet]
        [Route("api/getDoctorProfileWithAllValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetDoctorProfileWithAllValues(long doctorID)
        {

            Doctor doctor = new Doctor();
            var oDoctorProfileVM = new DoctorProfileVM();
            try
            {

                //if (doctorID == 0)
                //{
                //    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor ID is not valid." });
                //    return response;
                //}
                doctor = db.Doctors.Where(m => m.doctorID == doctorID && m.active == true).FirstOrDefault();
                if (doctor != null)
                {
                    //response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Doctor does not exist." });
                    //return response;
                    //}
                    //else
                    //{


                    oDoctorProfileVM = (from l in db.Doctors
                                        where l.active == true && l.doctorID == doctorID
                                        select new DoctorProfileVM
                                        {
                                            DoctorID = l.doctorID,
                                            //ProfilePhoto = ((l.picture)),
                                            ProfilePhotoBase64 = l.ProfilePhotoBase64,
                                            TitleName = l.title,
                                            Prefix = l.suffix,
                                            FirstName = l.firstName,
                                            LastName = l.lastName,
                                            Gender = l.gender,

                                            DOB = l.dob,
                                            TimeZone = l.timezone,

                                            AboutMe = l.aboutMe,
                                            Specialization = l.specialization,
                                            Publication = l.publication,
                                            Education = l.education,

                                            WorkExperience = l.workexperience,

                                            Address1 = l.address1,
                                            Address2 = l.address2,
                                            City = l.city,
                                            State = l.state,
                                            ZipCode = l.zip,
                                            HomePhone = l.homePhone,
                                            CellPhone = l.cellPhone,

                                            Latitude = l.lat,
                                            Longitude = l.lon,

                                            ConsultCharges = l.consultCharges,

                                            SectetQuestion1 = l.secretQuestion1,
                                            SectetQuestion2 = l.secretQuestion2,
                                            SectetQuestion3 = l.secretQuestion3,

                                            reviewStar=l.reviewStar
                                            //SectetAnswer1 = l.secretAnswer1,
                                            //SectetAnswer2 = l.secretAnswer2,
                                            //SectetAnswer3 = l.secretAnswer3
                                        }).FirstOrDefault();

                    if (oDoctorProfileVM != null)
                    {
                        oDoctorProfileVM.Speciality = db.DoctorSpecialities.Where(x => x.doctorID == doctorID).Select(x => x.specialityName).ToArray();
                        oDoctorProfileVM.Languages = db.DoctorLanguages.Where(x => x.doctorID == doctorID).Select(x => x.languageName).ToArray();
                        oDoctorProfileVM.LicenseStates = db.DoctorLicenseStates.Where(x => x.doctorID == doctorID).Select(x => x.stateName).ToArray();
                    }

                    
                }
                response = Request.CreateResponse(HttpStatusCode.OK, oDoctorProfileVM);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorProfileWithAllValues in ProfilesController.");
            }

        }

        [HttpPost]
        [Route("api/updateDoctorProfileWithAllValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorProfileWithAllValues(long doctorID, DoctorProfileVM model)
        {

            Doctor doctor = new Doctor();
           // var timezoneid = db.TimeZones.Where(x => x.timeZonee == model.TimeZone).Select(x => x.zoneName).FirstOrDefault();
            try
            {
                if (model.FirstName == null || model.FirstName == "" || !Regex.IsMatch(model.FirstName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "First name is not valid. Only letter and numbers are allowed." });
                    return response;
                }
                if (model.LastName != null || model.LastName != "")
                {
                    if (!Regex.IsMatch(model.LastName, "^[0-9a-zA-Z ]+$"))            //@"^[a-zA-Z\s]+$"
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
                /*if (model.ZipCode.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Zip is too long. Keep it below ten characters." });
                    return response;
                }*/
                if (model.Gender.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Gender is too long. Keep it below ten characters." });
                    return response;
                }
                else
                {
                    //Save Doctor Profile
                    doctor.active = true;

                    //doctor.picture = model.ProfilePhoto;
                    doctor.ProfilePhotoBase64 = model.ProfilePhotoBase64;
                    doctor.title = model.TitleName;
                    doctor.firstName = model.FirstName;
                    doctor.lastName = model.LastName;
                    doctor.suffix = model.Prefix;
                    doctor.gender = model.Gender;

                    doctor.dob = model.DOB;
                    doctor.timezone = model.TimeZone;
                    doctor.aboutMe = model.AboutMe;
                    doctor.specialization = model.Specialization;
                    doctor.publication = model.Publication;
                    doctor.education = model.Education;
                    doctor.workexperience = model.WorkExperience;

                    doctor.address1 = model.Address1;
                    doctor.address2 = model.Address2;
                    doctor.city = model.City;
                    doctor.state = model.State;
                    doctor.zip = model.ZipCode;

                    doctor.homePhone = model.HomePhone;
                    doctor.cellPhone = model.CellPhone;

                    doctor.lat = model.Latitude;
                    doctor.lon = model.Longitude;

                    doctor.cd = System.DateTime.Now;
                    doctor.mb = doctorID.ToString();
                    doctor.md = System.DateTime.Now;

                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    //Save Doctor Speciality
                    var allPrevSpeciality = db.DoctorSpecialities.Where(x => x.doctorID == doctorID).ToList();
                    foreach (var itemNewSpec in model.Speciality)
                    {
                        if (allPrevSpeciality.Count(x => x.specialityName == itemNewSpec) == 0)
                        {
                            db.DoctorSpecialities.Add(new DoctorSpeciality
                            {
                                doctorID = doctorID,
                                specialityName = itemNewSpec,
                                active = true,
                                CD = System.DateTime.Now,
                                CB = doctorID.ToString()
                            });
                        }
                    }
                    foreach (var itemPrvSpec in allPrevSpeciality)
                    {
                        if (!model.Speciality.Contains(itemPrvSpec.specialityName))
                        {
                            db.DoctorSpecialities.Remove(itemPrvSpec);
                        }
                    }
                    await db.SaveChangesAsync();

                    //Save Doctor Language
                    var allPrevLanguages = db.DoctorLanguages.Where(x => x.doctorID == doctorID).ToList();
                    foreach (var itemNewLang in model.Languages)
                    {
                        if (allPrevLanguages.Count(x => x.languageName == itemNewLang) == 0)
                        {
                            db.DoctorLanguages.Add(new DoctorLanguage
                            {
                                doctorID = doctorID,
                                languageName = itemNewLang,
                                active = true,
                                cd = System.DateTime.Now,
                                cb = doctorID.ToString()
                            });
                        }
                    }
                    foreach (var itemPrvLang in allPrevLanguages)
                    {
                        if (!model.Languages.Contains(itemPrvLang.languageName))
                        {
                            db.DoctorLanguages.Remove(itemPrvLang);
                        }
                    }
                    await db.SaveChangesAsync();


                    //Save Doctor License States
                    var allPrevLicStates = db.DoctorLicenseStates.Where(x => x.doctorID == doctorID).ToList();
                    foreach (var itemNewLicState in model.LicenseStates)
                    {
                        if (allPrevLicStates.Count(x => x.stateName == itemNewLicState) == 0)
                        {
                            db.DoctorLicenseStates.Add(new DoctorLicenseState
                            {
                                doctorID = doctorID,
                                stateName = itemNewLicState,
                                active = true,
                                cd = System.DateTime.Now,
                                cb = doctorID.ToString()
                            });
                        }
                    }
                    foreach (var itemPrvLicState in allPrevLicStates)
                    {
                        if (!model.LicenseStates.Contains(itemPrvLicState.stateName))
                        {
                            db.DoctorLicenseStates.Remove(itemPrvLicState);
                        }
                    }
                    await db.SaveChangesAsync();

                    //Return
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doctorID, message = "" });
                    return response;
                }
            }

            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateDoctorProfileWithAllValues in ProfileController.");
            }

        }
        
        [Route("api/getDoctorTimezone")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage getDoctorTimezone(string userid)
        {

            try
            {
                var timezone = (from l in db.Doctors
                                where l.userId == userid
                                select new
                                {
                                    zonename = l.timezone,
                                }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, timezone);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "getDoctorTimezone in ProfilesController");
            }

        }

        [Route("api/getPatientTimezone")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage getPatientTimezone(string userid)
        {

            try
            {
                var timezone = (from l in db.Patients
                                where l.userId == userid
                                select new
                                {
                                    zonename = l.timezone,
                                }).FirstOrDefault();
                response = Request.CreateResponse(HttpStatusCode.OK, timezone);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "getPatientTimezone in ProfilesController");
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
                    doctor.consultCharges = (model.consultCharges);
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

        [HttpPost]
        [Route("api/updateDoctorSecretAnswers")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> updateDoctorSecretAnswers(long doctorId, UpdateSecretQuestions model)
        {
            Doctor doctor = new Doctor();
            try
            {

                if (model.secretquestion1 == null || model.secretquestion1 == "")
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


        #endregion

        #region Patient APIS

        [HttpGet]
        [Route("api/getPatientProfileInitialValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetPatientProfileInitialValues()
        {
            try
            {
                var lstLanguageVM = (from p in db.Languages
                                     where p.active == true
                                     select new LanguageVM { languageID = p.languageID, languageName = p.languageName }).ToList();

                var lstSecretQuestionVM = (from p in db.SecretQuestions
                                           where p.active == true
                                           select new SecretQuestionVM { secretQuestionID = p.secretQuestionID, secretQuestion = p.secretQuestionn }).ToList();

                var lstTimeZoneVM = (from p in db.TimeZones
                                     where p.active == true
                                     select new TimeZoneVM { zoneID = p.zoneID, timeZone = p.timeZonee,zoneName=p.zoneName }).ToList();

                var lstCityVM = (from c in db.Cities
                                 where c.active == true
                                 select new CityVM { cityID = c.cityID, cityName = c.cityName }).ToList();

                var lstStateVM = (from c in db.States
                                  where c.active == true
                                  select new StateVM { stateID = c.stateID, stateName = c.stateName }).ToList();

                var lstTitleVM = (from p in db.TitleMasters
                                  where p.active == true
                                  select new TitleVM { titleId = p.titleID, titleName = p.titleName }).ToList();

                var lstSuffixVM = (from p in db.SuffixMasters
                                   where p.active == true
                                   select new SuffixVM { suffixId = p.SuffixID, suffixName = p.SuffixName }).ToList();

                var oRetModel = new PatientProfileInitialValues
                {
                    lstCityVM = lstCityVM,
                    lstLanguageVM = lstLanguageVM,
                    lstSecretQuestionVM = lstSecretQuestionVM,
                    lstStateVM = lstStateVM,
                    lstTimeZoneVM = lstTimeZoneVM,
                    lstTitleVM = lstTitleVM,
                    lstSuffixVM = lstSuffixVM
                };

                response = Request.CreateResponse(HttpStatusCode.OK, oRetModel);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "getPatientProfileInitialValues in ProfilesController.");
            }

        }


        [HttpGet]
        [Route("api/getPatientProfileWithAllValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetPatientProfileWithAllValues(long patientID)
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
                    var oPatientProfileVM = new PatientProfileVM();

                    oPatientProfileVM = (from l in db.Patients
                                         where l.active == true && l.patientID == patientID
                                         select new PatientProfileVM
                                         {
                                             PatientID = l.patientID,
                                             //ProfilePhoto = (l.picture),
                                             ProfilePhotoBase64 = l.ProfilePhotoBase64,
                                             TitleName = l.title,
                                             Prefix = l.suffix,
                                             FirstName = l.firstName,
                                             LastName = l.lastName,
                                             Gender = l.gender,

                                             DOB = l.dob,
                                             TimeZone = l.timezone,

                                             Height = l.height,
                                             Weight = l.weight,

                                             Address1 = l.address1,
                                             Address2 = l.address2,
                                             City = l.city,
                                             State = l.state,
                                             ZipCode = l.zip,
                                             HomePhone = l.homePhone,
                                             CellPhone = l.cellPhone,

                                             Latitude = l.lat,
                                             Longitude = l.lon,

                                             SectetQuestion1 = l.secretQuestion1,
                                             SectetQuestion2 = l.secretQuestion2,
                                             SectetQuestion3 = l.secretQuestion3

                                             //SectetAnswer1 = l.secretAnswer1,
                                             //SectetAnswer2 = l.secretAnswer2,
                                             //SectetAnswer3 = l.secretAnswer3
                                         }).FirstOrDefault();

                    if (oPatientProfileVM != null)
                    {
                        oPatientProfileVM.Languages = db.PatientLanguages.Where(x => x.patientID == patientID).Select(x => x.languageName).ToArray();
                    }

                    response = Request.CreateResponse(HttpStatusCode.OK, oPatientProfileVM);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientProfileWithAllValues in ProfilesController.");
            }
        }

        [HttpGet]
        [Route("api/getPatientProfileViewOnly")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetPatientProfileViewOnly(long patientID)
        {
            Patient patient = new Patient();
            var oPatientProfileVM = new PatientProfileWithExtraInfoVM();
            try
            {
                //if (patientID == 0)
                //{
                //    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
                //    return response;
                //}
                patient = db.Patients.Where(m => m.patientID == patientID && m.active == true).FirstOrDefault();
                if (patient != null)
                {
                //    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient does not exist." });
                //    return response;
                //}
                //else
                //{
                    

                    oPatientProfileVM = (from l in db.Patients
                                         where l.active == true && l.patientID == patientID
                                         select new PatientProfileWithExtraInfoVM
                                         {
                                             PatientID = l.patientID,
                                             //ProfilePhoto = (l.picture),
                                             ProfilePhotoBase64=l.ProfilePhotoBase64,
                                             TitleName = l.title,
                                             Prefix = l.suffix,
                                             FirstName = l.firstName,
                                             LastName = l.lastName,
                                             Gender = l.gender,

                                             Pharmacy = l.pharmacy,

                                             DOB = l.dob,
                                             TimeZone = l.timezone,

                                             Height = l.height,
                                             Weight = l.weight,

                                             Address1 = l.address1,
                                             Address2 = l.address2,
                                             City = l.city,
                                             State = l.state,
                                             ZipCode = l.zip,
                                             HomePhone = l.homePhone,
                                             CellPhone = l.cellPhone,

                                             Latitude = l.lat,
                                             Longitude = l.lon

                                         }).FirstOrDefault();

                    if (oPatientProfileVM != null)
                    {
                        oPatientProfileVM.Languages = db.PatientLanguages.Where(x => x.patientID == patientID).Select(x => x.languageName).ToArray();

                        //--My Medications
                        oPatientProfileVM.lstPatientMedicationVM = db.Medications.Where(x => x.patientId == patientID).Select(x => new PatientMedicationVM { Frequency = x.frequency, MedicineName = x.medicineName }).ToList();

                        //--My Allergies
                        oPatientProfileVM.lstPatientAllergiesVM = db.PatientAllergies.Where(x => x.patientID == patientID).Select(x => new PatientAllergiesVM { AllergyName = x.allergyName, Reaction = x.reaction, Severiaty = x.severity }).ToList();

                        //--My Surgeries
                        oPatientProfileVM.lstPatientSurgeryVM = db.PatientSurgeries.Where(x => x.patientID == patientID).Select(x => new PatientSurgeryVM { BodyPartName = x.bodyPart }).ToList();

                        //--My Family History
                        oPatientProfileVM.lstPatientFamilyHistoryVM = db.PatientFamilyHXes.Where(x => x.patientID == patientID).Select(x => new PatientFamilyHistoryVM { DeasesName = x.name, Relation = x.relationship }).ToList();

                        //Health Conditions
                        oPatientProfileVM.lstPatientHealthConditionsVM = db.Conditions.Where(x => x.active == true && x.patientID == patientID).Select(x => x.conditionName).ToList();

                        //Pharmacy
                        oPatientProfileVM.oPharmacy = oPatientProfileVM.Pharmacy;
                    }

                    var conditions = (from l in db.Conditions
                                      where l.active == true && l.patientID == patientID
                                      orderby l.conditionID descending
                                      select new GetPatientConditions
                                      {
                                          conditionID = l.conditionID,
                                          patientID = l.patientID,
                                          conditionName = l.conditionName.Trim(),
                                          reportedDate = l.reportedDate
                                      }).ToList();


                    
                }
                response = Request.CreateResponse(HttpStatusCode.OK, oPatientProfileVM);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientProfileWithAllValues in ProfilesController.");
            }
        }


        [HttpPost]
        [Route("api/updatePatientProfileWithAllValues")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdatePatientProfileWithAllValues(long patientID, PatientProfileVM model)
        {
            Patient patient = new Patient();
            //var timezoneid = db.TimeZones.Where(x => x.timeZonee == model.TimeZone).Select(x => x.zoneName).FirstOrDefault();
            try
            {
                if (model.FirstName == null || model.FirstName == "" || !Regex.IsMatch(model.FirstName, "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "First name is not valid. Only letter and numbers are allowed." });
                    return response;
                }
                if (model.LastName != null || model.LastName != "")
                {
                    if (!Regex.IsMatch(model.LastName, "^[0-9a-zA-Z ]+$"))            //@"^[a-zA-Z\s]+$"
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
             /*   if (model.ZipCode.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Zip is too long. Keep it below ten characters." });
                    return response;
                }*/
                if (model.Gender.Length > 10)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Gender is too long. Keep it below ten characters." });
                    return response;
                }
                else
                {
                    //Save Patient Profile
                    patient.active = true;

                    //patient.picture = model.ProfilePhoto;
                    patient.ProfilePhotoBase64 = Encoding.ASCII.GetString(model.ProfilePhoto);
                    patient.title = model.TitleName;
                    patient.firstName = model.FirstName;
                    patient.lastName = model.LastName;
                    patient.suffix = model.Prefix;
                    patient.gender = model.Gender;

                    patient.dob = model.DOB;
                    patient.timezone = model.TimeZone;
                    patient.height = model.Height;
                    patient.weight = model.Weight;

                    patient.address1 = model.Address1;
                    patient.address2 = model.Address2;
                    patient.city = model.City;
                    patient.state = model.State;
                    patient.zip = model.ZipCode;

                    patient.homePhone = model.HomePhone;
                    patient.cellPhone = model.CellPhone;

                    patient.lat = model.Latitude;
                    patient.lon = model.Longitude;

                    patient.cd = System.DateTime.Now;
                    patient.mb = patientID.ToString();
                    patient.md = System.DateTime.Now;

                    db.Entry(patient).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    //Save Doctor Language
                    var allPrevLanguages = db.PatientLanguages.Where(x => x.patientID == patientID).ToList();
                    foreach (var itemNewLang in model.Languages)
                    {
                        if (allPrevLanguages.Count(x => x.languageName == itemNewLang) == 0)
                        {
                            db.PatientLanguages.Add(new PatientLanguage
                            {
                                patientID = patientID,
                                languageName = itemNewLang,
                                active = true,
                                cd = System.DateTime.Now,
                                cb = patientID.ToString()
                            });
                        }
                    }
                    foreach (var itemPrvLang in allPrevLanguages)
                    {
                        if (!model.Languages.Contains(itemPrvLang.languageName))
                        {
                            db.PatientLanguages.Remove(itemPrvLang);
                        }
                    }
                    await db.SaveChangesAsync();

                    //Return
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patientID, message = "" });
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdatePatientProfileWithAllValues in ProfileController.");
            }

        }


        [HttpPost]
        [Route("api/updatePatientSecretAnswers")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> updatePatientSecretAnswers(long patientId, UpdateSecretQuestions model)
        {
            Patient patient = new Patient();
            try
            {
                if (model.secretquestion1 == null || model.secretquestion1 == "")
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

                patient = (from pat in db.Patients where pat.patientID == patientId && pat.active == true select pat).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient not found." });
                    return response;
                }

                else
                {
                    patient.secretQuestion1 = model.secretquestion1;
                    patient.secretQuestion2 = model.secretquestion2;
                    patient.secretQuestion3 = model.secretquestion3;
                    patient.secretQuestion1 = model.secretquestion1;
                    patient.secretAnswer1 = model.secretanswer1;
                    patient.secretAnswer2 = model.secretanswer2;
                    patient.secretAnswer3 = model.secretanswer3;
                    patient.md = System.DateTime.Now;
                    patient.mb = patientId.ToString();
                    db.Entry(patient).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patientId, message = "" });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddDoctorSecretAnswers in ProfilesController.");
            }

        }


        [Route("api/changePatientPassword")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> ChangePatientPassword(DoctorPasswordModel model)
        {
            try
            {
                string userID = (from d in db.Patients where d.active == true && d.patientID == model.doctorID select d.userId).FirstOrDefault();
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
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = 0, message = "Patient not found" });
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ChangePatientPassword in DoctorController.");
            }
        }

        #endregion

        #region Other APIs

        [HttpGet]
        [Route("api/getSecretQuestionList")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetSecretQuestionList()
        {


            try
            {
                var secretquests = (from sq in db.SecretQuestions where sq.active == true select new SecretQuestionVM { secretQuestionID = sq.secretQuestionID, secretQuestion = sq.secretQuestionn }).ToList();
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
                var timezones = (from tz in db.TimeZones where tz.active == true select new TimeZoneVM { zoneID = tz.zoneID, timeZone = tz.timeZonee , zoneName = tz.zoneName }).ToList();
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
                var cities = (from c in db.Cities where c.active == true select new CityVM { cityID = c.cityID, cityName = c.cityName }).ToList();
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
                var states = (from c in db.States where c.active == true select new StateVM { stateID = c.stateID, stateName = c.stateName }).ToList();
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
                                       licensedState = db.DoctorLicenseStates.Where(lic => lic.doctorID == doctorID).ToList(),
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
                   
                    var patprofile = (from l in db.Patients
                                      where l.active == true && l.patientID == patientID
                                      select new
                                      {
                                          firstName = l.firstName,
                                          lastName = l.lastName,
                                          gender = l.gender.Trim(),
                                          cellPhone = l.cellPhone,
                                          dob = l.dob,
                                          picture = l.picture,
                                          dateofbirth = l.dob,
                                          languages = (from pl in db.PatientLanguages
                                                          where pl.patientID == l.patientID && pl.active == true
                                                          select new { languageName = pl.languageName }).ToList(),
                                          allergies = (from pl in db.PatientAllergies
                                                        where pl.patientID == l.patientID && pl.active == true
                                                        select new { allergyName = pl.allergyName,severity=pl.severity, reaction=pl.reaction }).ToList(),
                                          conditions = (from pl in db.Conditions
                                                     where pl.patientID == l.patientID && pl.active == true
                                                     select new { conditionName = pl.conditionName }).ToList(),
                                          familyhx = (from pl in db.PatientFamilyHXes
                                                         where pl.patientID == l.patientID && pl.active == true
                                                         select new { conditionName = pl.name,relationship=pl.relationship }).ToList(),
                                         
                                          
                                          medication = (from pl in db.Medications
                                                           where pl.patientId == l.patientID && pl.active == true
                                                           select new { medicineName = pl.medicineName, frequency = pl.frequency }).ToList(),
                                          
                                          surgery = (from pl in db.PatientSurgeries
                                                        where pl.patientID == l.patientID && pl.active == true
                                                        select new {surgeryeName = pl.bodyPart}).ToList(),

                                          
                                          title = l.title,
                                          suffix = l.suffix,
                                          zip = l.zip
                                      }).FirstOrDefault();
                    
                    response = Request.CreateResponse(HttpStatusCode.OK, patprofile);
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientProfile in ProfilesController.");
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
        public async Task<HttpResponseMessage> UpdatePatientLanguages(long patlangID, PatientLanguages model)
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

        [HttpPost]
        [Route("api/deletePatientLanguages")]
        public async Task<HttpResponseMessage> RemovePatientLanguages(long langID)
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

        [HttpPost]
        [Route("api/deleteDoctorLanguages")]
        public async Task<HttpResponseMessage> RemoveDoctorLanguages(long langID)
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
                doclang.active = false;//Remove Operation changed
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

                doclicst = db.DoctorLicenseStates.Where(m => m.doctorLicenseStateID != licstateID && m.doctorID == model.doctorID && m.active == true && m.stateName ==
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

        [HttpPost]
        [Route("api/deleteDoctorLicensedStates")]
        public async Task<HttpResponseMessage> RemoveDoctorLicensedStates(long lsID)
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

        #endregion


        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
            response.ReasonPhrase = ex.Message;
            return response;
        }
        public string GetAge(DateTime? patientDOB)
        {
            try
            {
                if (patientDOB.HasValue)
                {

                    var today = DateTime.Today;

                    // Calculate the age.
                    var age = today.Year - patientDOB.Value.Year;

                    // Do stuff with it.
                    if (patientDOB.Value > today.AddYears(-age)) age--;

                    return string.Format("{0} Years", age);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }


    }
}
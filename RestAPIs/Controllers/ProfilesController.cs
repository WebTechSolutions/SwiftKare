using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    public class ProfilesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

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


        [HttpPost]
        [Route("api/updateDoctorProfile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdateDoctorProfile(long doctorID, DoctorProfileModel model)
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
                    doctor.cb = doctorID.ToString();
                    doctor.education = model.education;
                    doctor.timezone = model.timezone;
                    doctor.specialization = model.specialization;
                    doctor.suffix = model.suffix;
                    doctor.title = model.title;
                    doctor.workexperience = model.workExperience;
                    doctor.consultCharges = model.consultCharges;
                  

                    db.Entry(doctor).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = doctorID, message = "" });
                    return response;
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "UpdateDoctorProfile in DoctorController.");
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
                                       zip = l.zip
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
                    //patient.timezone = model.timezone;
                    //patient.city = model.city;
                    //patient.suffix = model.suffix;
                    //patient.title = model.title;
                    //patient.height = model.height;
                    //patient.weight = model.weight;

                    db.Entry(patient).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = patient.patientID, message = "" });
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
       

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
            return response;
        }
    }
}

using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess.CustomModels;
using System.Text;
using DataAccess.CommonModels;
using System.Text.RegularExpressions;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientMedicationController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getMedicine")]
        public HttpResponseMessage GetMedicines()
        {
            try
            {
                var medicines = (from l in db.Medicines
                                   where l.active == true
                                   select new MedicineModel { medicineID = l.medicineID, medicineName = l.medicineName.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, medicines);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetMedicine in PatientMedicationController");
            }

        }
        [Route("api/getFrequency")]
        public HttpResponseMessage GetFrequency()
        {
            try
            {
                var frequency =db.Frequencies.Where(f=>f.active==true).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, frequency);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetFrequency in PatientMedicationController");
            }

        }

        [Route("api/getPatienMedications")]
        public HttpResponseMessage GetPatientMedications(long patientID)
        {
            try
            {
                var medications = (from l in db.Medications
                                   where l.active == true && l.patientId == patientID
                                   orderby l.medicationID descending
                                   select new GetMedication { medicationID = l.medicationID, patientId=l.patientId, medicineName = l.medicineName.Trim(), frequency = l.frequency.Trim(), reporteddate = l.reportedDate }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, medications);
                return response;
                               
            }
            catch(Exception ex)
            {
                return ThrowError(ex, "GetPatientMedications in PatientMedicationController");
            }
         
        }

        [Route("api/addPatientMedication")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientMedication(PatientMedication_Custom model)
        {
            Medication medication = new Medication();
            try
            {
                
                if (model.medicineName == null || model.medicineName == ""|| !Regex.IsMatch(model.medicineName, "^[0-9a-zA-Z ]+$"))
                    {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel {ID=0, message="Medicine name is not valid. Only letter and numbers are allowed." } );
                    return response;
                }
                if (model.frequency != null || model.frequency != "")
                {
                    if (!Regex.IsMatch(model.frequency, "^[0-9a-zA-Z ]+$"))
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Frequency is not valid." });
                        return response;
                    }
                }
                if ( model.patientId == 0 || model.patientId==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
                    return response;
                }
                medication = db.Medications.Where(m => m.patientId == model.patientId && m.medicineName.Trim() == model.medicineName.Trim() && m.active == true).FirstOrDefault();
                if (medication == null)
                {
                    medication = new Medication();
                    medication.active = true;
                    medication.frequency = model.frequency;
                    medication.patientId = model.patientId;
                    medication.cd = System.DateTime.Now;
                    medication.source = "S";
                    medication.reportedDate = System.DateTime.Now;
                    medication.cb = medication.patientId.ToString();
                    medication.medicineName = model.medicineName;
                    db.Medications.Add(medication);
                    await db.SaveChangesAsync();
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medicine already exists." });
                    return response;
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientMedication in PatientMedicationController.");
            }
            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = medication.medicationID, message = "" });
            return response;

        }

        
        [Route("api/editPatientMedication")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientMedication(long medicationID, PatientMedication_Custom model)
        {
            try
            {
                Medication medication = new Medication();
                if (medicationID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medicine ID is not valid." });
                    return response;
                }
                if(model.frequency != null && model.frequency != "")
                {
                    if(!Regex.IsMatch(model.frequency, "^[0-9a-zA-Z ]+$"))
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Frequency is not valid." });
                        return response;
                    }
                }
                if (model.medicineName == null || model.medicineName == ""||!Regex.IsMatch(model.medicineName.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                  
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medicine name is not valid. Only letters and numbers are allowed." });
                return response;
                }
                if (model.patientId == 0 || model.patientId == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient ID is not valid." });
                    return response;
                }
                medication = db.Medications.Where(m => m.patientId == model.patientId && m.medicationID != medicationID && m.medicineName.Trim() == model.medicineName.Trim() && m.active == true).FirstOrDefault();
                if (medication != null)
                {
                    //conditionID = -1;
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medication already exists." });
                    return response;
                }

                medication = db.Medications.Where(m => m.medicationID == medicationID).FirstOrDefault();
                if (medication == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medication record not found." });
                    return response;
                }
               
                medication.frequency = model.frequency;
                medication.medicineName = model.medicineName;
                medication.md = System.DateTime.Now;
                medication.mb = model.patientId.ToString();
                db.Entry(medication).State = EntityState.Modified;
                await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return ThrowError(ex, "EditPatientMedication in PatientMedicationController.");
                }

         
            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = medicationID, message = "" });
            return response;
        }

       
        [Route("api/deletePatientMedication")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> RemovePatientMedication(long medicationID)
        {

            try
            {
                Medication medication = db.Medications.Where(med=>med.medicationID==medicationID && med.active==true).FirstOrDefault();
                Patient patient = new Patient();
               // if (medication != null) { patient = await db.Patients.FindAsync(medication.patientId); }
              
            if (medication == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Medication not found." });
                return response;
            }
            medication.active = false;//Delete Operation changed
            medication.mb = medication.patientId.ToString();
            medication.md = System.DateTime.Now;
            db.Entry(medication).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientMedication in PatientMedicationController.");
            }
           
            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = medicationID, message = "" });
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

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
          
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            return response;
        }
    }
}

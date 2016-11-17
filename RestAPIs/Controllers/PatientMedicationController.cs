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
                                   select new MedicineModel { medicineID = l.medicineID, medicineName = l.medicineName}).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, medicines);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetMedicine in PatientMedicationController");
            }

        }

        [Route("api/getPatienMedications")]
        public HttpResponseMessage GetPatientMedications(long patientID)
        {
            try
            {
                var medications = (from l in db.Medications
                                   where l.active == true && l.patientId == patientID
                                   select new GetMedication { medicationID = l.medicationID, patientId=l.patientId, medicineName = l.medicineName, frequency = l.frequency, reporteddate = l.reportedDate }).ToList();
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
                
                if (model.medicineName == null || model.medicineName == "")
                    {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medicine name is not valid.");
                    return response;
                }
                if( model.patientId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient ID is not valid.");
                    return response;
                }
                medication = db.Medications.Where(m => m.medicineName == model.medicineName).FirstOrDefault();
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
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medicine name already exists.");
                    return response;
                }
               
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientMedication in PatientMedicationController.");
            }
            response = Request.CreateResponse(HttpStatusCode.OK, medication.medicationID);
            return response;

        }

        
        [Route("api/editPatientMedication")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientMedication(long medicationID, PatientMedication_Custom model)
        {
            try
            {
                if(medicationID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medicine ID is not valid.");
                    return response;
                }
                if(model.frequency != null|| model.frequency != "")
                {
                    if(!Regex.IsMatch(model.medicineName, @"^[a-zA-Z\s]+$"))
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, "Frequency is not valid.");
                        return response;
                    }
                }
                if (model.medicineName == null || model.medicineName == ""||!Regex.IsMatch(model.medicineName, @"^[a-zA-Z\s]+$"))
                {
                  
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medicine name is not valid.");
                return response;
                }
                if(model.patientId == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient ID is not valid.");
                    return response;
                }
                Medication medication = db.Medications.Where(m => m.medicationID == medicationID).FirstOrDefault();
                if (medication == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication record not found.");
                    return response;
                }
               
                medication = db.Medications.Where(m => m.medicationID != medicationID && m.medicineName==model.medicineName).FirstOrDefault();
            if (medication != null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication already exists.");
                return response;
            }
                medication = new Medication();
                medication.frequency = model.frequency;
                medication.md = System.DateTime.Now;
                medication.mb = model.patientId.ToString();
                db.Entry(medication).State = EntityState.Modified;
                await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return ThrowError(ex, "EditPatientMedication in PatientMedicationController.");
                }

         
            response = Request.CreateResponse(HttpStatusCode.OK, medicationID);
            return response;
        }

        [HttpPost]
        [Route("api/deletePatientMedication")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientMedication(long medicationID)
        {

            try
            {
                Medication medication = await db.Medications.FindAsync(medicationID);
                Patient patient = await db.Patients.FindAsync(medication.patientId);
            if (medication == null || patient == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication record not found.");
                return response;
            }
            medication.active = false;//Delete Operation changed
            medication.mb = patient.userId;
            medication.md = System.DateTime.Now;
            db.Entry(medication).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientMedication in PatientMedicationController.");
            }
           
            response = Request.CreateResponse(HttpStatusCode.OK, medicationID);
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
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            return response;
        }
    }
}

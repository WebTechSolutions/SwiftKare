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

namespace RestAPIs.Controllers
{
    public class PatientMedicationController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        
        [Route("api/getPatientMedication/patientId")]
        public HttpResponseMessage GetPatientMedications(long Id)
        {
            try
            {
                var newmedication = db.SP_GetPatientMedication(Id);
                response = Request.CreateResponse(HttpStatusCode.OK, newmedication);
                return response;
            }
            catch(Exception ex)
            {
                return ThrowError(ex, "GetPatientMedications in PatientMedicationController");
            }
         
        }

        [Route("api/addPatientMedication/medicationModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientMedication(PatientMedication_Custom model)
        {
            Medication medication = new Medication();
            try
            {
                if (model.frequency == null || model.frequency == "" || model.medicineName == null || model.medicineName == ""
                    ||model.patientId==0||model.userId==null||model.userId=="")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication Model is not valid.");
                    return response;
                }
            Patient patient = db.Patients.Where(p=>p.userId==model.userId).FirstOrDefault();
            
            medication.active = true;
            medication.frequency = model.frequency;
            medication.patientId = model.patientId;
            medication.cd = System.DateTime.Now;
            medication.source = "S";
            medication.reportedDate = System.DateTime.Now;
            medication.cb = patient.email;
            medication.medicineName = model.medicineName;
            db.Medications.Add(medication);
            await db.SaveChangesAsync();
           
            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientMedication in PatientMedicationController.");
            }

            var newmedication = db.SP_GetPatientMedication(model.patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newmedication);
            return response;
        }

        
        [Route("api/editPatientMedication/medicationModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientMedication(PatientMedication_Custom model)
        {
            try
            {


                if (model.frequency == null || model.frequency == "" || model.medicineName == null || model.medicineName == ""
                          || model.patientId == 0 || model.userId == null || model.userId == ""||model.medicationID==0)
                {
                  
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication Model is not valid.");
                return response;
                }
            Medication medication = db.Medications.Where(m=>m.medicationID==model.medicationID).FirstOrDefault();
            if (medication==null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication record not found.");
                return response;
            }
            Patient patient = db.Patients.Where(p => p.userId == model.userId).FirstOrDefault();
            medication.frequency = model.frequency;
            medication.md = System.DateTime.Now;
            medication.mb = patient.email;
            db.Entry(medication).State = EntityState.Modified;
            await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientMedication in PatientMedicationController.");
            }

            var newmedication = db.SP_GetPatientMedication(model.patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newmedication);
            return response;
        }

        [HttpPost]
        [Route("api/deletePatientMedication/medicationModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientMedication(long medicationId,long patientId)
        {

            try
            {
                Medication medication = await db.Medications.FindAsync(medicationId);
            Patient patient = await db.Patients.FindAsync(patientId);
            if (medication == null || patient == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Medication record not found.");
                return response;
            }
            medication.active = false;//Delete Operation changed
            medication.mb = patient.email;
            medication.md = System.DateTime.Now;
            db.Entry(medication).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientMedication in PatientMedicationController.");
            }
            var newmedication = db.SP_GetPatientMedication(patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newmedication);
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

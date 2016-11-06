using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    public class PatientConditionController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPatienCondition/patientId/")]
        public HttpResponseMessage GetPatientConditions(long Id)
        {
            try
            {
                var newmedication = db.SP_GetPatientConditions(Id);
                response = Request.CreateResponse(HttpStatusCode.OK, newmedication);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientConditions in PatientConditionController");
            }

        }

        [Route("api/addPatientCondition/conditionModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientCondition(PatientConditions_Custom model)
        {
            Condition condition = new Condition();
            try
            {
                if (model.conditionName == null || model.conditionName == "" || model.patientID == null || model.patientID == 0||model.userId==null||model.userId=="")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition Model is not valid.");
                    return response;
                }
                Patient patient = db.Patients.Where(p => p.userId == model.userId).FirstOrDefault();

                condition.active = true;
                condition.conditionName = model.conditionName;
                condition.patientID = model.patientID;
                condition.cd = System.DateTime.Now;
                condition.Source = "S";
                condition.reportedDate = System.DateTime.Now;
                condition.cb = patient.email;
               
                db.Conditions.Add(condition);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientCondition in PatientConditionController.");
            }

            var newcondition = db.SP_GetPatientConditions(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, newcondition);
            return response;
        }

        [Route("api/editPatientCondition/conditionModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientCondition(PatientConditions_Custom model)
        {
            try
            {
                if (model.conditionName == null || model.conditionName == "" || model.patientID == null || model.patientID == 0 || 
                    model.userId == null || model.userId == ""||model.conditionID==0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition Model is not valid.");
                return response;
            }
            Condition condition = db.Conditions.Where(m => m.conditionID == model.conditionID).FirstOrDefault();
            if (condition == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition record not found.");
                return response;
            }
            Patient patient = db.Patients.Where(p => p.userId == model.userId).FirstOrDefault();
            condition.conditionName = model.conditionName;
            condition.md = System.DateTime.Now;
            condition.mb = patient.email;
            db.Entry(condition).State = EntityState.Modified;

           
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientCondition in PatientConditionController.");
            }

            var newcondition = db.SP_GetPatientConditions(model.patientID);
            response = Request.CreateResponse(HttpStatusCode.OK, newcondition);
            return response;
        }

        [HttpPost]
        [Route("api/deletePatientCondition/conditionModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientCondition(long conditionId,long patientId)
        {
            try
            {
                Condition condition = await db.Conditions.FindAsync(conditionId);
            Patient patient = await db.Patients.FindAsync(patientId);
            if (condition == null || patient == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition record not found.");
                return response;
            }
            condition.active = false;//Delete Operation changed
            condition.mb = patient.email;
            condition.md = System.DateTime.Now;
            db.Entry(condition).State = EntityState.Modified;

           
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientCondition in PatientConditionController.");
            }

            var newcondition = db.SP_GetPatientConditions(patientId);
            response = Request.CreateResponse(HttpStatusCode.OK, newcondition);
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
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

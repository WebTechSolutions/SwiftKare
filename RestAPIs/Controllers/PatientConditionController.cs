using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    public class PatientConditionController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPatienConditions")]
        public HttpResponseMessage GetPatientConditions(long patientID)
        {
            try
            {
                var conditions = (from l in db.Conditions
                           where l.active == true && l.patientID == patientID
                           orderby l.conditionID descending
                           select new GetPatientConditions{ conditionID = l.conditionID, patientID=l.patientID, conditionName=l.conditionName,
                           reportedDate=l.reportedDate}).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, conditions);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientConditions in PatientConditionController");
            }
        }

        [Route("api/addPatientCondition")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientCondition(PatientConditions_Custom model)
        {
            Condition condition = new Condition();
            try
            {
               
                if (model.conditionName == null || model.conditionName == "" || !Regex.IsMatch(model.conditionName, @"^[a-zA-Z\s]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition name is not valid.");
                    return response;
                }
                if(model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Patient ID.");
                    return response;
                }

                condition = db.Conditions.Where(m => m.conditionName == model.conditionName).FirstOrDefault();
                if(condition==null)
                {
                    condition = new Condition();
                    var pt = (from p in db.Patients
                                  where p.patientID == model.patientID
                                  select new { p.userId }).FirstOrDefault();
                    condition.active = true;
                    condition.conditionName = model.conditionName;
                    condition.patientID = model.patientID;
                    condition.cd = System.DateTime.Now;
                    condition.Source = "S";
                    condition.reportedDate = System.DateTime.Now;
                    condition.cb = pt.userId;

                    db.Conditions.Add(condition);
                    await db.SaveChangesAsync();
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition name already exists.");
                    return response;
                }
              

            }
            catch (Exception ex)
            {
                ThrowError(ex, "AddPatientCondition in PatientConditionController.");
            }
            response = Request.CreateResponse(HttpStatusCode.OK, condition.conditionID);
            return response;


        }

        [Route("api/editPatientCondition")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientCondition(long conditionID,PatientConditions_Custom model)
        {
            Condition condition = new Condition();
            try
            {
                if (conditionID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Condition ID.");
                    return response;
                }
                if (model.conditionName == null || model.conditionName == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition Model is not valid.");
                    return response;
                }
                if ( model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Patient ID.");
                    return response;
                }
                condition = db.Conditions.Where(m => m.conditionID == conditionID).FirstOrDefault();
                if (condition == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition record not found.");
                    return response;
                }
                condition = db.Conditions.Where(m => m.conditionID != conditionID && m.conditionName == model.conditionName).FirstOrDefault();
                if (condition == null)
                {
                    condition.conditionName = model.conditionName;
                    condition.md = System.DateTime.Now;
                    condition.mb = condition.patientID.ToString();
                    db.Entry(condition).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition name already exists.");
                    return response;
                }
           
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientCondition in PatientConditionController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, conditionID);
            return response;
        }

       
        [Route("api/deletePatientCondition")]
        public async Task<HttpResponseMessage> DeletePatientCondition(long conditionID)
        {
            try
            {
            Condition condition = await db.Conditions.FindAsync(conditionID);
            Patient patient = await db.Patients.FindAsync(condition.patientID);
            if (condition == null || patient == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Condition record not found.");
                conditionID = 0;
                return response;
            }
            condition.active = false;//Delete Operation changed
            condition.mb = patient.userId;
            condition.md = System.DateTime.Now;
            db.Entry(condition).State = EntityState.Modified;

           
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientCondition in PatientConditionController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, conditionID);
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

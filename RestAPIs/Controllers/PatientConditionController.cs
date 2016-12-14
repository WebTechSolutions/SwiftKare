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
    [Authorize]
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
                           select new GetPatientConditions{ conditionID = l.conditionID, patientID=l.patientID, conditionName=l.conditionName.Trim(),
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
               
                if (model.conditionName == null || model.conditionName == "" || !Regex.IsMatch(model.conditionName.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid condition name. Only letters and numbers are allowed." } );
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }

                condition = db.Conditions.Where(m => m.patientID == model.patientID && m.conditionName == model.conditionName.Trim() && m.active == true).FirstOrDefault();
                if (condition==null)
                {
                    condition = new Condition();
                    condition.active = true;
                    condition.conditionName = model.conditionName;
                    condition.patientID = model.patientID;
                    condition.cd = System.DateTime.Now;
                    condition.Source = "S";
                    condition.reportedDate = System.DateTime.Now;
                    condition.cb = model.patientID.ToString();

                    db.Conditions.Add(condition);
                    await db.SaveChangesAsync();
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Condition name already exists." });
                    return response;
                }
              

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientCondition in PatientConditionController.");
            }
            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = condition.conditionID, message = "" } );
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
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid condition ID." });
                    return response;
                }
                if (model.conditionName == null || model.conditionName == "" || !Regex.IsMatch(model.conditionName.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid condition name.Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                //check for duplicate names
                condition = db.Conditions.Where(m => m.patientID == model.patientID && m.conditionID != conditionID && m.conditionName == model.conditionName.Trim() && m.active==true).FirstOrDefault();
                if (condition != null)
                {
                    //conditionID = -1;
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Condition name already exists." });
                    return response;
                }
               
                condition = db.Conditions.Where(m => m.conditionID == conditionID).FirstOrDefault();
                if (condition == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Condition not found." });
                    return response;
                }
                else
                {
                        condition.conditionName = model.conditionName;
                        condition.md = System.DateTime.Now;
                        condition.mb = condition.patientID.ToString();
                        db.Entry(condition).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    
                }
               
           
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientCondition in PatientConditionController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conditionID, message = "" });
            return response;
        }

       
        [Route("api/deletePatientCondition")]
        public async Task<HttpResponseMessage> RemovePatientCondition(long conditionID)
        {
            try
            {
                Patient patient = new Patient();
                if (conditionID == null || conditionID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid condition ID." });
                    return response;
                }
                Condition condition = db.Conditions.Where(cond => cond.conditionID==conditionID && cond.active==true).FirstOrDefault();
               if (condition != null) { patient = await db.Patients.FindAsync(condition.patientID); }
            if (condition == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Condition not found." });
                return response;
            }
            condition.active = false;//Delete Operation changed
            condition.mb = condition.patientID.ToString();
            condition.md = System.DateTime.Now;
            db.Entry(condition).State = EntityState.Modified;

           
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientCondition in PatientConditionController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conditionID, message = "" });
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            //response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.ToString(), Encoding.Unicode);
            //return response;
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: "+ Action+"\n"+ex.Message });
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

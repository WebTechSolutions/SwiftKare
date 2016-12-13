using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.CustomModels;
using System.Threading.Tasks;

namespace RestAPIs.Controllers
{
    public class AlertsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getAlerts")]
        public HttpResponseMessage GetAlerts()
        {
            try
            {
                var alerts = db.Alerts.Where(al=>al.active==true).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, alerts);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAlerts in AlertsController");
            }
        }

        //[Route("api/deleteAlert")]
        //public async Task<HttpResponseMessage> RemoveAlert(long alertID)
        //{
        //    try
        //    {
                
        //        if (alertID == 0)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid alert ID." });
        //            return response;
        //        }
        //        Alert alert = db.Alerts.Where(cond => cond.conditionID == conditionID && cond.active == true).FirstOrDefault();
        //        if (condition != null) { patient = await db.Patients.FindAsync(condition.patientID); }
        //        if (condition == null)
        //        {
        //            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Condition not found." });
        //            return response;
        //        }
        //        condition.active = false;//Delete Operation changed
        //        condition.mb = condition.patientID.ToString();
        //        condition.md = System.DateTime.Now;
        //        db.Entry(condition).State = EntityState.Modified;


        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return ThrowError(ex, "DeletePatientCondition in PatientConditionController.");
        //    }

        //    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = conditionID, message = "" });
        //    return response;
        //}
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method:" + Action + "\n" + ex.Message });
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

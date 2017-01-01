using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.CustomModels;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Text;
using System.Globalization;
using System.Data.Entity.Core.Objects;

namespace RestAPIs.Controllers
{
    public class AlertsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPatientAlerts")]
        public HttpResponseMessage getPatientAlerts(long patientID)
        {
            try
            {
                //var alerts = db.Alerts.Where(al=>al.active==true && al.alertFor==patientID).ToList();
                var alerts = (from al in db.Alerts
                              where al.active == true && al.alertFor == patientID
                              select new AlertModel { alertID = al.alertID, alertText = al.alertText, alertDate = al.cd,isRead=al.read }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, alerts);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientAlerts in AlertsController");
            }
        }

        [Route("api/deletePatientAlert")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> RemovePatientAlert(DeleteAlertModel model)
        {
            try
            {
                Alert alert = db.Alerts.Where(all => all.alertID == model.alertID && all.active == true).FirstOrDefault();

                if (alert == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Alert not found." });
                    response.ReasonPhrase = "Alert not found.";
                    return response;
                }
                alert.active = false;//Delete Operation changed
                alert.mb = model.userID;
                alert.md = System.DateTime.Now;
                db.Entry(alert).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientAlert in AlertsController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.alertID, message = "" });
            return response;
        }

        [Route("api/readAllPatientAlerts")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> ReadAllAlerts(long patientID)
        {
            try
            {
                List<Alert> alert = db.Alerts.Where(all => all.read == false || all.read == null  && all.active == true && all.alertFor==patientID).ToList();

                if (alert != null)
                {
                   foreach(var item in alert)
                    {
                        item.read = true;
                        item.mb = db.Patients.Where(p => p.patientID == patientID && p.active == true).Select(pt => pt.userId).FirstOrDefault();
                        item.md = System.DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                       
                    }
                    await db.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ReadAllAlerts in AlertsController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patientID, message = "" });
            return response;
        }




        [Route("api/getDoctorAlerts")]
        public HttpResponseMessage getDoctorAlerts(long doctorID)
        {
            try
            {
               
                var alerts = (from al in db.Alerts
                              where al.active == true && al.alertFor == doctorID
                              select new AlertModel { alertID = al.alertID, alertText = al.alertText, alertDate = al.cd, isRead = al.read }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, alerts);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "getDoctorAlerts in AlertsController");
            }
        }

        [Route("api/deleteDoctorAlert")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> RemoveDoctorAlert(DeleteAlertModel model)
        {
            try
            {
                Alert alert = db.Alerts.Where(all => all.alertID == model.alertID && all.active == true).FirstOrDefault();

                if (alert == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Alert not found." });
                    response.ReasonPhrase = "Alert not found.";
                    return response;
                }
                alert.active = false;//Delete Operation changed
                alert.mb = model.userID;
                alert.md = System.DateTime.Now;
                db.Entry(alert).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeleteDoctorAlert in AlertsController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = model.alertID, message = "" });
            return response;
        }

        [Route("api/readAllDoctorAlerts")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> ReadAllDoctorAlerts(long doctorID)
        {
            try
            {
                List<Alert> alert = db.Alerts.Where(all => all.read == false || all.read == null && all.active == true && all.alertFor == doctorID).ToList();

                if (alert != null)
                {
                    foreach (var item in alert)
                    {
                        item.read = true;
                        item.mb = db.Doctors.Where(p => p.doctorID == doctorID && p.active == true).Select(pt => pt.userId).FirstOrDefault();
                        item.md = System.DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;

                    }
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ReadAllDoctorAlerts in AlertsController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = doctorID, message = "" });
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.Message, Encoding.Unicode);
            response.ReasonPhrase = ex.Message;
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

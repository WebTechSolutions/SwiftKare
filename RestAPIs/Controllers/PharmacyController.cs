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
    public class PharmacyController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getPharmacy")]
        public HttpResponseMessage GetPharmacy(long patientID)
        {
            try
            {
                var pharmacy = (from l in db.Patients
                                  where l.patientID== patientID &&  l.active == true
                                  select new PatientPharmacy_Custom { patientID = l.patientID, pharmacyid=l.pharmacyid, pharmacy = l.pharmacy.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, pharmacy);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetAllergies in PatientAllergiesController");
            }

        }

        [Route("api/addPatientPharmacy")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPharmacy(PatientPharmacy_Custom model)
        {
            Patient patient = new Patient();
            try
            {
                if (model.pharmacy == "" || model.pharmacy == null || !Regex.IsMatch(model.pharmacy.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid pharmacy name. Only letters and numbers are allowed." });
                    return response;
                }
                if(model.patientID == 0 || model.patientID==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID"});
                    return response;
                }
                patient = db.Patients.Where(m => m.patientID == model.patientID).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Patient record not found." });
                    return response;
                }
              
                patient.pharmacy = model.pharmacy;
                patient.pharmacyid = model.pharmacyid;
                patient.md = System.DateTime.Now;
                patient.mb = model.patientID.ToString();
                db.Entry(patient).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPharmacy in PharmacyController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = Convert.ToInt64(model.pharmacyid), message = "Pharmacy is added successfully." });
            return response;
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "value");
            response.Content = new StringContent("Following Error occurred at method. " + Action + "\n" + ex.Message, Encoding.Unicode);
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

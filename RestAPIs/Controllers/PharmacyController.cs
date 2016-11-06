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
    public class PharmacyController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;


        [Route("api/addPatientPharmacy/pharmacyModel/")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPharmacy(PatientPharmacy_Custom model)
        {
            try
            {
                if (model.patientID==0||model.pharmacy==""||model.pharmacy==null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient pharmacy Model is not valid.");
                    return response;
                }
                Patient patient = db.Patients.Where(m => m.patientID == model.patientID).FirstOrDefault();
                if (patient == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, "Patient record not found.");
                    return response;
                }
              
                patient.pharmacy = model.pharmacy;
                patient.pharmacyid = model.pharmacyid;
                patient.md = System.DateTime.Now;
                patient.mb = patient.email;
                db.Entry(patient).State = EntityState.Modified;


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPharmacy in PharmacyController.");
            }

            //var newpsurgery = db.SP_GetPatientMedication();
            response = Request.CreateResponse(HttpStatusCode.OK, "Pharmacy is added successfully.");
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

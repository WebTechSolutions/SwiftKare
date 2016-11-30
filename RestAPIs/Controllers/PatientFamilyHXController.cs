using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientFamilyHXController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getFamilyHXItems")]
        public HttpResponseMessage GetFamilyHXItems()
        {
            try
            {
                var hxitems = (from l in db.FamilyHXItems
                                 where l.active == true
                                 select new { familyHXItemsID = l.familyHXItemsID, name = l.name.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, hxitems);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetFamilyHXItems in PatientFamilyHXController");
            }

        }

        [Route("api/getPatientFamilyHXItems")]
        public HttpResponseMessage GetPatientFamilyHXItems(long patientID)
        {
            try
            {
                var ptFamilyHX = (from l in db.PatientFamilyHXes
                                 where l.active == true && l.patientID == patientID
                                 select new { fhxid = l.fhxid, patientID = l.patientID, name = l.name.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, ptFamilyHX);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientFamilyHXItems in PatientFamilyHXController");
            }

        }
        [Route("api/addPatientFamilyHX")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> addPatientFamilyHX(PatientFamilyHX_Custom model)
        {
            PatientFamilyHX phx = new PatientFamilyHX();
            try
            {
                if (model.name == null || model.name == "" || !Regex.IsMatch(model.name.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid familyHX. Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }

                phx = db.PatientFamilyHXes.Where(p => p.name.Trim() == model.name.Trim()).FirstOrDefault();
                if (phx != null)
                {
                    phx.relationship = model.relationship;
                    phx.md = System.DateTime.Now;
                    phx.mb = phx.patientID.ToString();
                    phx.active = true;
                    db.Entry(phx).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = phx.fhxid, message = "" });
                    return response;
                }
                if (phx == null)
                {
                    phx = new PatientFamilyHX();
                    phx.active = true;
                    phx.name = model.name;
                    phx.relationship = model.relationship;
                    phx.patientID = model.patientID;
                    phx.cd = System.DateTime.Now;
                    phx.cb = model.patientID.ToString();
                    db.PatientFamilyHXes.Add(phx);
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientFamilyHX in PatientFamilyHXController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = phx.fhxid, message = "" });
            return response;
        }

        [Route("api/updatePatientFamilyHX")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> UpdatePatientFamilyHX(long patientfamilyHXID, long patientID,string relationship)
        {
            PatientFamilyHX pls = new PatientFamilyHX();
            try
            {
                
                if (patientfamilyHXID == null || patientfamilyHXID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient familyHX ID." });
                    return response;
                }
                if (patientID == null || patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                pls = db.PatientFamilyHXes.Where(all => all.fhxid == patientfamilyHXID && all.patientID == patientID).FirstOrDefault();
                if (pls != null)
                {
                    pls.relationship = relationship;
                    pls.md = System.DateTime.Now;
                    pls.mb = patientID.ToString();
                    db.Entry(pls).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "PatientLifeStyle not found." });
                    return response;
                }
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientLifeStyle in PatientLifeStyleController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patientfamilyHXID, message = "" });
            return response;
        }
        [Route("api/deletePatientFamilyHX")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> DeletePatientFamilyHX(long fhxID)
        {
            try
            {
                if (fhxID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid FamilyHX ID." });
                    return response;
                }
                PatientFamilyHX pfhx = await db.PatientFamilyHXes.FindAsync(fhxID);

                if (pfhx == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "FamilyHX not found." });
                    return response;
                }
                else
                {

                    pfhx.active = false;//Delete Operation changed
                    pfhx.mb = pfhx.patientID.ToString();
                    pfhx.md = System.DateTime.Now;
                    db.Entry(pfhx).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientFamilyHX in PatientFamilyHXController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = fhxID, message = "" });
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
           
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method:" + Action + " " + ex.Message });
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

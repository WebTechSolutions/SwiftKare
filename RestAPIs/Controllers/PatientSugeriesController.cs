﻿using DataAccess;
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
    public class PatientSugeriesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;


        [Route("api/getAutoCompleteSurgeries")]
        public HttpResponseMessage getAutoCompleteSurgeries(string search)
        {
            try
            {
                var surgeries = (from l in db.Surgeries
                                 where l.active == true && l.surgeryName.StartsWith(search)
                                 select new SurgeriesModel { surgeryName = l.surgeryName.Trim() }).Take(10).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, surgeries);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSurgeries in PatientSurgeriesController");
            }

        }
        [Route("api/getSurgeries")]
        public HttpResponseMessage getSurgeries()
        {
            try
            {
                var surgeries = (from l in db.Surgeries
                                 where l.active == true
                                 select new SurgeriesModel { surgeryName = l.surgeryName.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, surgeries);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSurgeries in PatientSurgeriesController");
            }

        }

        [Route("api/getPatienSurgeries")]
        public HttpResponseMessage GetPatientSugeries(long patientID)
        {
            try
            {
                var surgeries = (from l in db.PatientSurgeries
                                 where l.active == true && l.patientID == patientID
                                 select new PSurgeries {surgeryID=l.surgeryID, patientID = l.patientID, bodyPart = l.bodyPart.Trim() }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, surgeries);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientSugeries in PatientSurgeriesController");
            }

        }

        [Route("api/addPatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientSugery(PatientSurgery_Custom model)
        {
            PatientSurgery psurgery = new PatientSurgery();
            try
            {
                if (model.bodyPart == null || model.bodyPart == "" )
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid surgery name." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }
                
                psurgery = db.PatientSurgeries.Where(p => p.bodyPart.Trim() == model.bodyPart.Trim() && p.patientID==model.patientID && p.active==true).FirstOrDefault();
                if (psurgery!=null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery already exists." });
                    response.ReasonPhrase = "Surgery already exists.";

                    return response;
                }
                if (psurgery==null)
                {
                    psurgery = new PatientSurgery();
                    psurgery.active = true;
                    psurgery.bodyPart = model.bodyPart;
                    psurgery.patientID = model.patientID;
                    psurgery.cd = System.DateTime.Now;
                    psurgery.reportedDate = System.DateTime.Now;
                    psurgery.cb = model.patientID.ToString();

                    db.PatientSurgeries.Add(psurgery);
                    await db.SaveChangesAsync();
                }
               
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = psurgery.surgeryID, message = "" });
            return response;
        }

        [Route("api/editPatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientSugery(long surgeryID,PatientSurgery_Custom model)
        {
            PatientSurgery psurgery = new PatientSurgery();
            try
            {
                if (model.bodyPart == null || model.bodyPart == "" || !Regex.IsMatch(model.bodyPart.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID =0, message = "Invalid surgery. Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient id." });
                    return response;
                }
                psurgery = db.PatientSurgeries.Where(all => all.bodyPart.Trim() == model.bodyPart.Trim() && all.surgeryID != surgeryID && all.active == true).FirstOrDefault();
                if (psurgery != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery already exists." });
                    response.ReasonPhrase = "Surgery already exists.";
                    return response;
                }
                psurgery = db.PatientSurgeries.Where(m => m.surgeryID == surgeryID).FirstOrDefault();
                if (psurgery == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery not found." });
                    return response;
                }
               
                psurgery.bodyPart = model.bodyPart;
                psurgery.md = System.DateTime.Now;
                psurgery.mb = psurgery.patientID.ToString();
                db.Entry(psurgery).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = surgeryID, message = "" });
            return response;
        }

      
        [Route("api/deletePatientSurgery")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> RemovePatientSurgery(long surgeryID)
        {
            try
            {
                if (surgeryID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid surgery ID." });
                    return response;
                }
                PatientSurgery psurgery = await db.PatientSurgeries.FindAsync(surgeryID);
            
                if (psurgery == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Surgery not found." });
                    return response;
                }
                else
                {
                    
                    psurgery.active = false;//Delete Operation changed
                    psurgery.mb = psurgery.patientID.ToString();
                    psurgery.md = System.DateTime.Now;
                    db.Entry(psurgery).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientSurgery in PatientSurgeriesController.");
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = surgeryID, message = "" });
            return response;
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
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


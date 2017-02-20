using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;
using RestAPIs.Models;
using DataAccess.CustomModels;
using System.Net.Http;
using RestAPIs.Extensions;
using System;
using RestAPIs.Helper;
using RestAPIs.DoseSpotApi;

namespace RestAPIs.Controllers
{
    public class DoseSpotController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();

        [HttpPost]
        [Route("api/SearchPharmacy")]
        public HttpResponseMessage GetPharmacySearchResult(DoseSpotPharmacySearch oModel)
        {
            PharmacySearchMessageResult oResult = DoseSpotHelper.SearchPharmacy(oModel);

            var oRetRes = new List<PharmacyEntry>();
            foreach (var item in oResult.Pharmacies)
            {
                oRetRes.Add(new PharmacyEntry
                {
                    PharmacyId = item.PharmacyId,
                    StoreName = item.StoreName,
                    Address1 = item.Address1,
                    Address2 = item.Address2,
                    City = item.City,
                    State = item.State,
                    ZipCode = item.ZipCode,
                    PrimaryPhone = item.PrimaryPhone,
                    PrimaryPhoneType = item.PrimaryPhoneType
                });
            }

            HttpResponseMessage oResp = Request.CreateResponse(HttpStatusCode.OK, oRetRes);
            return oResp;
        }

       
        [Route("api/GetRefillErr")]
        public HttpResponseMessage GetRefillReqErr()
        {
            RefillRequestsTransmissionErrorsMessageResult oResult = DoseSpotHelper.RefillReqErr();

            HttpResponseMessage oResp = Request.CreateResponse(HttpStatusCode.OK, oResult);
            return oResp;
        }
        //GetRefillReqURL
        
        [Route("api/GetRefillReqURL")]
        public HttpResponseMessage GetRefillReqURL()
        {
            var oResult = DoseSpotHelper.GetRefillUrl();
            HttpResponseMessage oResp = Request.CreateResponse(HttpStatusCode.OK, oResult);
            return oResp;
        }
        [HttpGet]
        [Route("api/GetPatientDoseSpotUrl")]
        public HttpResponseMessage GetPatientDoseSpotUrl(long patientId)
        {
            try
            {
                //Search if patient contains doseSpot Id
                var oPatientInfo = db.Patients.FirstOrDefault(x => x.patientID == patientId);

                int? DoseSpotPatientId = null;
                if (oPatientInfo != null)
                {
                    var oDoseSpotPatientEntry = new DoseSpotPatientEntry
                    {
                        PatientId = DoseSpotPatientId,
                        FirstName = oPatientInfo.firstName,
                        LastName = oPatientInfo.lastName,
                        MiddleName = "",
                        Address1 = oPatientInfo.address1,
                        Address2 = oPatientInfo.address2,
                        City = oPatientInfo.city,
                        State = oPatientInfo.state,
                        ZipCode = oPatientInfo.zip,
                        Gender = oPatientInfo.gender,
                        Phone = oPatientInfo.cellPhone,
                        DateOfBirth = oPatientInfo.dob.Value,
                        PharmacyId=oPatientInfo.pharmacyid
                    };

                    if (string.IsNullOrEmpty(oPatientInfo.DoseSpotPatientId))
                    {
                        var oRet = DoseSpotHelper.RegisterPatientWithDoseSpot(oDoseSpotPatientEntry);

                        int DoseSpotPatId;
                        int.TryParse(oRet, out DoseSpotPatId);

                        if (DoseSpotPatId != 0) {
                            oPatientInfo.DoseSpotPatientId = oRet;

                            db.Entry(oPatientInfo).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        oDoseSpotPatientEntry.PatientId = DoseSpotPatId;
                    }
                    else{
                        oDoseSpotPatientEntry.PatientId = Convert.ToInt32(oPatientInfo.DoseSpotPatientId);
                    }

                    //Register Patient
                    var cFinalUrl = DoseSpotHelper.GetEPrescriptionUrl(oDoseSpotPatientEntry);
                    return Request.CreateResponse(HttpStatusCode.OK, cFinalUrl);
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "This patient does not exists");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
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

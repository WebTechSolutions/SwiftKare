using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApp.DoseSpotService;

namespace WebApp.Controllers
{
    public class DoseSpotDemoController : Controller
    {
        public ActionResult Index()
        {
            return View("Index", null);
        }

        [HttpPost]
        public ActionResult Index(DoseSpotPharmacySearch oModel)
        {
            try
            {
                APISoapClient api = new DoseSpotService.APISoapClient("APISoap12");
                SingleSignOn sso = new DoseSpotService.SingleSignOn();
                sso.SingleSignOnClinicId = 664;
                sso.SingleSignOnUserId = 2844;
                sso.SingleSignOnCode = "2yQvBvwcqztNZLglP4aRy6GRQD8zBDJnusuHnyfhEYwIJHaNdGrjwnEQRwOwNwtYh2S1OagWxg5cTIEHsyiarRJe+xlXNl1LJ9YSVtnt4PzkbRNGOE/ouA";
                sso.SingleSignOnUserIdVerify = "4nOh+4l5FsZrUg/S4x4vs/mx5WLywbUghL07S5NZ30iWoepkaWC5C622DR5FswWOJQmP5jorLsJRLd2888UTuw";

                PharmacySearchMessage oSrch = new PharmacySearchMessage
                {
                    PharmacyNameSearch = oModel.Name,
                    PharmacyCity = oModel.City,
                    PharmacyStateTwoLetters = oModel.State,
                    PharmacyZipCode = oModel.Zip,
                    SingleSignOn = sso
                };

                PharmacySearchMessageResult oRes = api.PharmacySearch(oSrch);



                return View("Index", oRes);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult RegisterPatient()
        {
            var oModel = new DoseSpotPatientEntry
            {
                FirstName = "Fred",
                MiddleName = "Middle",
                LastName = "Jones",
                DateOfBirth = "09-Jan-17",
                Gender = "Male",
                Address1 = "123%20Main%20St",
                Address2 = "",
                City = "Waltham",
                State = "MA",
                ZipCode = "02451",
                Phone = "781-444-4444"
            };

            ViewBag.ModelInfo = oModel;

            return View("RegisterPatient", null);
        }

        [HttpPost]
        public ActionResult RegisterPatient(DoseSpotPatientEntry oModel)
        {
            //Default Criterias - Starts
            string SingleSignOnCode = "4YkCDmLvaXVwZqmaUHP4ekJvcLnI2wT3eosHXf5OCT60OmCTpIk5TK2F1ThcPrECwtsXPPNmti7PnRaK0xOUl7xL86iHkJ%2F7pFM6nVyGfcCO4zgQrUJx1w";
            string SingleSignOnUserId = "2844";
            string SingleSignOnUserIdVerify = "WDwQv%2BuYbmr%2FkOdY7KT3vGVs2bw3dYappSWEq54YyQ5Cn6k69Rua3t0JPMoSxajJBllSrQHUG9Eu8XyfDAm1Ug";
            string SingleSignOnClinicId = "664";
            //Default Criterias - Ends

            //Build Url - Starts
            var cUrl = string.Format("http://my.staging.dosespot.com/LoginSingleSignOn.aspx?b=2&SingleSignOnCode={0}&SingleSignOnUserId={1}&SingleSignOnUserIdVerify={2}&SingleSignOnClinicId={3}&Prefix=Prefix&FirstName={4}&MiddleName={5}&LastName={6}&Suffix=Suffix&DateOfBirth={7}&Gender={8}&Address1={9}&Address2={10}&City={11}&State={12}&ZipCode={13}&PrimaryPhone={14}&PrimaryPhoneType=Home&PhoneAdditional1=&PhoneAdditionalType1=&PhoneAdditional2=&PhoneAdditionalType2=",
                SingleSignOnCode, SingleSignOnUserId, SingleSignOnUserIdVerify, SingleSignOnClinicId, oModel.FirstName, oModel.MiddleName, oModel.LastName, oModel.DateOfBirth, oModel.Gender, oModel.Address1, oModel.Address2, oModel.City, oModel.State, oModel.ZipCode, oModel.Phone);
            //Build Url - Ends

            //Get Patient Id - Starts
            var cPatientId = GetPatientIdFromUrl(cUrl);
            //Get Patient Id - Ends

            string finalUrl = cUrl + "&PatientID=" + cPatientId;

            //string finalUrl = "http://my.staging.dosespot.com/secure/PatientDetail.aspx?PatientID=" + cPatientId;
            return View("RegisterPatient", model: finalUrl);
        }


        private string GetPatientIdFromUrl(string urlAddress)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                //Find Patient Id in response - Starts

                if (!string.IsNullOrEmpty(data))
                {
                    //%3fpatientid%3d342502
                    var startIndex = data.IndexOf("%3fpatientid%3d") + 15;
                    var subString = data.Substring(startIndex);

                    var length = subString.IndexOf("\"");

                    var oRetPatientId = data.Substring(startIndex, length);
                    return oRetPatientId;
                }
                //Find Patient Id in response - Ends
            }

            return "";
        }

    }
}
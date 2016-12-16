using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.PatientRepositories;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Patient")]
    public class MyHealthController : Controller
    {
        MyHealthRepository oMyHealthRepository;
        LifeStyleRepository oLifeStyleRepository;

        public MyHealthController()
        {
            oMyHealthRepository = new MyHealthRepository();
            oLifeStyleRepository= new LifeStyleRepository();
        }


        public ActionResult Index()
        {
            if (SessionHandler.IsExpired)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("PatientLogin", "Account"),
                    isRedirect = true
                });
            }
            else
            {
                return View();
            }
        }
        
        public PartialViewResult PartialFamilyHX()
        {
            try
            {
                var oAllFamilyHX = oMyHealthRepository.GetFamilyHX();
                var oAllPFamilyHX = oMyHealthRepository.GetPatientFamilyHX(SessionHandler.UserInfo.Id);
                var oRelationship = oMyHealthRepository.GetRelationships();
                List<PFamilyList> familyHX = new List<PFamilyList> { };
                foreach (var item in oAllPFamilyHX)
                {
                     familyHX.Add(new PFamilyList {fhxID=item.fhxID, familyHXItemsID=0,patientID= item.patientID,name=item.name,relationship=item.relationship });
                }
               foreach(var item in oAllFamilyHX)
                {
                    var flag = familyHX.Where(pfx => pfx.name == item.name).FirstOrDefault();
                    if(flag==null)
                    {
                        familyHX.Add(new PFamilyList { fhxID = 0, familyHXItemsID = item.familyHXItemsID, patientID = 0, name = item.name, relationship = "" });
                    }
                    
                }
                ViewBag.familyHX = familyHX;
                ViewBag.relationships = oRelationship;

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialFamilyHX");
        }

        public PartialViewResult PartialLifeStyle()
        {
            try
            {
                var oLifeStyleQuestions = oLifeStyleRepository.GetLifeStyle();
                var oPLifeStyle = oLifeStyleRepository.GetPatientLifeStyle(SessionHandler.UserInfo.Id);
               
                List<PLifeStyleList> lifeStyle = new List<PLifeStyleList> { };
                foreach (var item in oPLifeStyle)
                {
                    lifeStyle.Add(new PLifeStyleList { patientLifeStyleID = item.patientlifestyleID, questionID = 0, patientID = item.patientID, question = item.question,answer=item.answer });
                }
                foreach (var item in oLifeStyleQuestions)
                {
                    var flag = lifeStyle.Where(pls => pls.question == item.question).FirstOrDefault();
                    if (flag == null)
                    {
                        lifeStyle.Add(new PLifeStyleList { patientLifeStyleID = 0, questionID = item.questionID, patientID = 0, question = item.question, answer = ""});
                    }

                }
                ViewBag.lifeStyle = lifeStyle;
               
            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("PartialLifeStyle");
        }


        public JsonResult AddLifeStyle(PatientLifeStyle_Custom _objPLS)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oLifeStyleRepository.AddPatientLifeStyle(_objPLS);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        public JsonResult UpdateLifeStyle(PatientLifeStyleModel _objPLS)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oLifeStyleRepository.UpdatePatientLifeStyle(_objPLS);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        public JsonResult AddFamilyHX(PatientFamilyHX_Custom _objFHX)
        {
            try
            {
               ApiResultModel apiresult = new ApiResultModel();
                PatientFamilyHX_Custom objHX = new PatientFamilyHX_Custom();
                objHX.name = _objFHX.name;
                objHX.patientID = _objFHX.patientID;
                if(_objFHX.relationship!=null)
                {
                    objHX.relationship = Regex.Replace(_objFHX.relationship, @"^\s*$\n", string.Empty, RegexOptions.Multiline).Trim();
                }
                else
                {
                    objHX.relationship = null;
                }
              
                apiresult = oMyHealthRepository.AddFamilyHX(objHX);
               return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        public JsonResult UpdateFamilyHX(UpdateFamilyHX _objFHX)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                UpdateFamilyHX objHX = new UpdateFamilyHX();
                objHX.patientfamilyHXID = _objFHX.patientfamilyHXID;
                objHX.patientID = _objFHX.patientID;
                if (_objFHX.relationship != null)
                {
                    objHX.relationship = Regex.Replace(_objFHX.relationship, @"^\s*$\n", string.Empty, RegexOptions.Multiline).Trim();
                }
                else
                {
                    objHX.relationship = null;
                }

                apiresult = oMyHealthRepository.UpdateFamilyHX(objHX);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        [HttpPost]
        public JsonResult deleteFamilyHX(long fhxID)
        {
            var oRet = oMyHealthRepository.DeleteFamilyHX(fhxID);
            return Json(oRet);
        }
       
        
    }
}
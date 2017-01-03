using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.PatientRepositories;

namespace WebApp.Controllers
{
    public class AlertController : Controller
    {
        AlertRespository oAlertRepository;
        public AlertController()
        {

            oAlertRepository = new AlertRespository();
        }
        // GET: Alert
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
                ViewBag.errorMessage = "";
                ViewBag.successMessage = "";
                return View();
            }
        }
        public PartialViewResult PartialAlertView()
        {
            try
            {
                var oData = oAlertRepository.LoadPatientAlerts(SessionHandler.UserInfo.Id);

                return PartialView("PartialAlertView", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
                return PartialView("PartialAlertView");
            }
            
        }
        public PartialViewResult AlertView()
        {
            try
            {
                var oData = oAlertRepository.LoadPatientAlerts(SessionHandler.UserInfo.Id);

                return PartialView("AlertView", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
                return PartialView("AlertView");
            }

        }
        public JsonResult DeleteAlert(DeleteAlertModel _objRAPP)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oAlertRepository.DeleteAlert(_objRAPP);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase.ToString() });
            }

        }
        public JsonResult ReadAllAlerts(long id)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oAlertRepository.ReadAllAlerts(id);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase.ToString() });
            }

        }

    }
}
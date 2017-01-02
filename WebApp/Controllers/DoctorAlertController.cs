using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;

namespace WebApp.Controllers
{
    public class DoctorAlertController : Controller
    {
        // GET: DoctorAlert
        DoctorAlertRepository oDoctorAlertRepository;
        public DoctorAlertController()
        {

            oDoctorAlertRepository = new DoctorAlertRepository();
        }
        // GET: Alert
        public ActionResult Index()
        {
            if (SessionHandler.IsExpired)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("DoctorLogin", "Account"),
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
        public PartialViewResult PartialDoctorAlertView()
        {
            try
            {
                var oData = oDoctorAlertRepository.LoadDoctorAlerts(SessionHandler.UserInfo.Id);

                return PartialView("PartialDoctorAlertView", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
                return PartialView("PartialDoctorAlertView");
            }

        }
        public PartialViewResult DoctorAlertView()
        {
            try
            {
                var oData = oDoctorAlertRepository.LoadDoctorAlerts(SessionHandler.UserInfo.Id);

                return PartialView("DoctorAlertView", oData);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
                return PartialView("DoctorAlertView");
            }

        }
        public JsonResult DeleteAlert(DeleteAlertModel _objRAPP)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = oDoctorAlertRepository.DeleteAlert(_objRAPP);
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
                apiresult = oDoctorAlertRepository.ReadAllAlerts(id);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase.ToString() });
            }

        }
    }
}
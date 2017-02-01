using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;
using WebApp.Helper;
using System;
using System.Globalization;
using WebApp.Repositories.PatientRepositories;

namespace WebApp.Controllers
{
    [DoctorSessionExpire]
    [Authorize(Roles = "Doctor")]
    [Authorize]
    public class DoctorController : Controller
    {
        DoctorTimingsRepository objTimingRepo = new DoctorTimingsRepository();
        // GET: DoctorJson
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DoctorTimings()
        {
            return View();
        }
        [HttpPost]
        // GET: Doctor Timings
        public JsonResult GetDoctorTimings()
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
                var Model = new DoctorTimingsViewModel();
                var objRepo = new DoctorRepository();
                Model.DoctorId = SessionHandler.UserInfo.Id;
                Model.DoctorTimingsList = objTimingRepo.GetListByDoctorId(Model.DoctorId).ToList();

                Model.DayWiseTimings = GetList(Model.DoctorTimingsList);



                Model.DoctorTiming = new DoctorTimingsModel();
                Model.DoctorTiming.doctorID = Model.DoctorId;
                Model.DoctorTiming.doctorTimingsID = 0;
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
        }

        private List<DoctorTimingsListModel> GetList(List<DoctorTimingsModel> list)
        {

            var daywiseList = new List<DoctorTimingsListModel>();
            var obj = new DoctorTimingsListModel();
            obj.Day = "Monday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);

            obj = new DoctorTimingsListModel();
            obj.Day = "Tuesday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);


            obj = new DoctorTimingsListModel();
            obj.Day = "Wednesday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);


            obj = new DoctorTimingsListModel();
            obj.Day = "Thursday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);

            obj = new DoctorTimingsListModel();
            obj.Day = "Friday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);

            obj = new DoctorTimingsListModel();
            obj.Day = "Saturday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);

            obj = new DoctorTimingsListModel();
            obj.Day = "Sunday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower()) > 0 ? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList() : new List<DoctorTimingsModel>();
            daywiseList.Add(obj);

            return daywiseList;


        }



        [HttpPost]
        public JsonResult CreateEditTimings(DoctorTimingsModel model)
        {
            if (model.from.Length < 8)
                model.from = "0" + model.from;

            if (model.to.Length < 8)
                model.to = "0" + model.to;

            if (SessionHandler.IsExpired)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("DoctorLogin", "Account"),
                    isRedirect = true
                });
            }
            if (model.from.Contains("PM"))
            {
                if (model.to.Contains("AM"))
                {
                    return Json("single day", JsonRequestBehavior.AllowGet);
                }
            }
            if (model.from == model.to)
            {
                return Json("same time", JsonRequestBehavior.AllowGet);

            }
            if (DateTime.ParseExact(model.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <
                DateTime.ParseExact(model.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay)
            {
                return Json("from time greater than to time", JsonRequestBehavior.AllowGet);
            }
            TimeSpan diff = DateTime.ParseExact(model.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay -
                DateTime.ParseExact(model.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            if (diff.TotalMinutes < 15)
            {
                return Json("time range", JsonRequestBehavior.AllowGet);
            }
            var timingsList = objTimingRepo.GetListByDoctorId(model.doctorID);
            var alreadItems = timingsList
                .Where(o => o.day == model.day &&
                (o.from == model.from || o.to == model.to
                ||
                (
                DateTime.ParseExact(model.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(model.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay

                )
                ||
                (
                DateTime.ParseExact(model.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(model.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                )

                ||
                (
                DateTime.ParseExact(model.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay <=
                DateTime.ParseExact(o.from, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                &&
                DateTime.ParseExact(model.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay >=
                DateTime.ParseExact(o.to, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay
                )

                )).ToList();
            if (alreadItems.Count <= 0)
            {
                var userName = SessionHandler.UserName;

                if (model.doctorTimingsID <= 0)
                {
                    model.username = userName;
                    objTimingRepo.Add(model);
                }
                else
                {
                    model.username = userName;
                    objTimingRepo.Put(model.doctorTimingsID, model);
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("overlapped", JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult DeleteDoctorTiming(long id)
        {
            if (SessionHandler.IsExpired)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("DoctorLogin", "Account"),
                    isRedirect = true
                });
            }
            objTimingRepo.Delete(id);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateConsult(CreateConsultModel model)
        {
            try
            {
                ConsultationRepository objConsultationRepo = new ConsultationRepository();
               
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = objConsultationRepo.CreateConsult(model);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }
    }

}
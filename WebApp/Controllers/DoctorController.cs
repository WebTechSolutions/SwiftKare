﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;
using WebApp.Helper;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Doctor")]
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

        private List<DoctorTimingsListModel> GetList(List<DoctorTimingsModel> list)
        {
            var daywiseList = new List<DoctorTimingsListModel>();
            var obj = new DoctorTimingsListModel();
            obj.Day = "Monday";
            obj.Timings = list.Count(o => o.day.ToLower() == obj.Day.ToLower())>0? list.Where(o => o.day.ToLower() == obj.Day.ToLower()).ToList(): new List<DoctorTimingsModel>();
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
            //jam
            var timingsList = objTimingRepo.GetListByDoctorId(model.doctorID);
            var alreadItems = timingsList.Where(o => o.day == model.day && o.from == model.from && o.to == model.to).ToList();
            if(alreadItems.Count<=0)
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
            }
            
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDoctorTiming(long id)
        {
            objTimingRepo.Delete(id);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

    }

}
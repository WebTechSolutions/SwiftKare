using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using System.Globalization;
using DataAccess;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {

        DoctorTimingsRepository objTimingRepo = new DoctorTimingsRepository();

        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }

        // GET: Doctor Timings
        public ActionResult DoctorTimings()
        {
            var Model = new DoctorTimingsViewModel();
            var objRepo = new DoctorRepository();
            var userId = HttpContext.User.Identity.GetUserId();
            var doctor = objRepo.GetByUserId(userId);
            Model.DoctorTimingsList = doctor.DoctorTimings.Where(o=>o.active== true);
            Model.DoctorTiming = new DoctorTiming();
            Model.DoctorTiming.doctorID = doctor.doctorID;
            Model.DoctorTiming.doctorTimingsID = 0;
            Model.Days = GetSelectListItems(
                new List<string>
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday",
            });
            return View(Model);
        }

        public ActionResult CreateDoctorTiming()
        {
            return View("CreateEdit", new DoctorTimingsViewModel());
        }

        public ActionResult EditDoctorTiming(long id)
        {
            var objRepo = new DoctorRepository();
           // var objTimingRepo = new DoctorTimingsRepository();
            var Model = new DoctorTimingsViewModel();

            Model.DoctorTiming = objTimingRepo.GetById(id);
            DateTime timeFrom = DateTime.Today.Add((TimeSpan)Model.DoctorTiming.from);
            DateTime timeTo = DateTime.Today.Add((TimeSpan)Model.DoctorTiming.to);
            Model.Timing.From = timeFrom.ToString("hh:mm tt");
            Model.Timing.To = timeTo.ToString("hh:mm tt");
            Model.DoctorTimingsList = Model.DoctorTiming.Doctor.DoctorTimings.Where(o=>o.active==true);
            Model.Days = GetSelectListItems(
                new List<string>
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday",
            });
            return View("DoctorTimings", Model);
        }

        public ActionResult DeleteDoctorTiming(long id)
        {
            //var objRepo = new DoctorTimingsRepository();
            objTimingRepo.Delete(id);
            return RedirectToAction("DoctorTimings");//////
        }


        [HttpPost]
        public ActionResult CreateEditTimings(DoctorTimingsViewModel model)
        {

            DateTime dateTimeFrom = DateTime.ParseExact(model.Timing.From,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
            DateTime dateTimeTo = DateTime.ParseExact(model.Timing.To,
                                "hh:mm tt", CultureInfo.InvariantCulture);


            TimeSpan From = dateTimeFrom.TimeOfDay;
            TimeSpan To = dateTimeTo.TimeOfDay;
            model.DoctorTiming.from = From;
            model.DoctorTiming.to = To;

            if (model.DoctorTiming.doctorTimingsID <= 0)
            {
                //var objRepo = new DoctorTimingsRepository();
                objTimingRepo.Add(model.DoctorTiming);
            }
            else
            {
                //var objRepo = new DoctorTimingsRepository();
                objTimingRepo.Put(model.DoctorTiming.doctorTimingsID, model.DoctorTiming);
            }
            return RedirectToAction("DoctorTimings");


            //return View(model);
        }


        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            //     <option value="State Name">State Name</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }

            return selectList;
        }




    }
}
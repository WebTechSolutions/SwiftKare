using DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;

namespace WebApp.Controllers
{
    public class SeeDoctorController : Controller
    {
       // GET: SeeDoctor
        public ActionResult SeeDoctor()
        {
            SeeDoctorViewModel SeeDoctorViewModel = new SeeDoctorViewModel();
            SeeDoctorViewModel=assignDefault();
            return View(SeeDoctorViewModel);
        }

        //private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        //{
        //    // Create an empty list to hold result of the operation
        //    var selectList = new List<SelectListItem>();

        //    // For each string in the 'elements' variable, create a new SelectListItem object
        //    // that has both its Value and Text properties set to a particular value.
        //    // This will result in MVC rendering each item as:
        //    //     <option value="State Name">State Name</option>
        //    foreach (var element in elements)
        //    {
        //        selectList.Add(new SelectListItem
        //        {
        //            Value = element,
        //            Text = element
        //        });
        //    }

        //    return selectList;
        //}
        [HttpPost]
        public ActionResult SearchDoctor(SeeDoctorViewModel model)
        {
            SeeDoctorViewModel seedoctorviewModel = new SeeDoctorViewModel();
            seedoctorviewModel = assignDefault();
            SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
           //IEnumerable<SeeDoctorDTO> docList= objDoctorRepo.SeeDoctor(model.Doctor.firstName, model.Gender, model.Language, model.Speciallity, model.AppDate.DayOfWeek.ToString(), model.Timing.seacrhTime);
            try
            {
                if (model.SearchModel.Gender == "ALL") { model.SearchModel.Gender = ""; }
                if (model.SearchModel.DoctorName == null) { model.SearchModel.DoctorName = ""; }
                IEnumerable docList = objSeeDoctorRepo.SeeDoctor(model.SearchModel);
                if (docList != null)
                {
                    ViewBag.DoctorList = docList;
                }
                if (docList == null)
                {
                    ViewBag.infoMessage = "No record found";
                    ViewBag.errorMessage = "";
                    ViewBag.successMessage = "";

                }
                return View("SeeDoctor", seedoctorviewModel);

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Error occurred while processing your request. Please try again. " + ex.Message;
                ViewBag.successMessage = "";
                ViewBag.infoMessage = "";
                return View("SeeDoctor");
            }


        }
        [HttpPost]
        public ActionResult SearchAppointments(SeeDoctorViewModel model)
        {
            try
            {
                //SeeDoctorViewModel seedoctorviewModel = new SeeDoctorViewModel();
                //seedoctorviewModel = assignDefault();
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                IEnumerable<BookAppointment> appList = objSeeDoctorRepo.FetchDoctorTimes(model.BookAppointment);
                if (appList != null)
                {
                    //calculate time slots
                    model.BookAppointment.mylist = displayTimeSlots(appList);
                    ViewBag.DoctorList = appList;
                }
                if (appList == null)
                {
                    ViewBag.infoMessage = "No record found";
                    ViewBag.errorMessage = "";
                    ViewBag.successMessage = "";

                }
                return View("SeeDoctor", model);

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Error occurred while processing your request. Please try again. " + ex.Message;
                ViewBag.successMessage = "";
                ViewBag.infoMessage = "";
                return View("SeeDoctor", model);
            }

        }
        private List<string> displayTimeSlots(IEnumerable<BookAppointment> appList)
        {
            List<string> timeSlots = new List<string> { };
           
            foreach (var item in appList)
            {
              
                TimeSpan startTime = item.fromTime;
                TimeSpan endTime = item.toTime;
                if (!(timeSlots.Contains(startTime.ToString())))
                {
                    timeSlots.Add(startTime.ToString());
                    TimeSpan tempp = TimeSpan.FromMinutes(15);
                    startTime=startTime.Add(tempp);
                    
                }

                bool flag = true;
                while (flag)
                {
                    if (!(timeSlots.Contains(startTime.ToString())))
                    {
                        //if (!(TimeSpan.Equals(slot, item.appTime)))
                        //{
                            timeSlots.Add(startTime.ToString());
                            TimeSpan tempp = TimeSpan.FromMinutes(15);
                            startTime = startTime.Add(tempp);

                        //}
                    }
                    else
                    {
                        TimeSpan tempp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(tempp);

                    }
                        
                    if (TimeSpan.Equals(startTime, endTime))
                    {
                        if (!(timeSlots.Contains(startTime.ToString())))
                        {
                                 timeSlots.Add(startTime.ToString());
                                TimeSpan tempp = TimeSpan.FromMinutes(15);
                            startTime = startTime.Add(tempp);

                        }
                        flag = false;
                    }
                } //while end 
            }//for loop for database records.


           
                foreach(var app in appList)
                {
                    if(timeSlots.Contains(app.appTime.ToString()))
                    {
                    timeSlots.Remove(app.appTime.ToString());
                    }
                }
                return timeSlots;
        }

        private SeeDoctorViewModel assignDefault()
        {
            SeeDoctorViewModel SeeDoctorViewModel = new SeeDoctorViewModel();
            SearchModel SearchModel = new SearchModel();
            BookAppointment BookAppointment = new BookAppointment();

            var langRepo = new LanguageRepository();
            var specRepo = new SpecialityRepository();

            IEnumerable<Language> language = langRepo.Get();
            SelectList langlist = new SelectList(language, "languageName", "languageName");

            SearchModel.LanguageList = langlist;
            IEnumerable<Speciallity> speciality = specRepo.Get();
            SelectList speclist = new SelectList(speciality, "specialityName", "specialityName");
            SearchModel.SpeciallityList = speclist;
            SeeDoctorViewModel.SearchModel = SearchModel;
            SeeDoctorViewModel.BookAppointment = BookAppointment;
            return SeeDoctorViewModel;
        }
            
    }
}
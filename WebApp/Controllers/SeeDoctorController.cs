using DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;
using DataAccess.CommonModels;
using WebApp.Repositories.PatientRepositories;
using DataAccess.CustomModels;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;
using WebApp.Helper;
using System.Globalization;
using Newtonsoft.Json;
using WebApp.Repositories.ProfileRepositories;

namespace WebApp.Controllers
{
    [PatientSessionExpire]
    [Authorize(Roles = "Patient")]
    public class SeeDoctorController : Controller
    {
        // GET: SeeDoctor
        public ActionResult SeeDoctor()
        {
           
                ViewBag.PatienID = SessionHandler.UserInfo.Id;

                ViewBag.PublisherKey = ConfigurationManager.AppSettings["StripePayPublisherKey"].ToString();
                ViewBag.Amount = 2000;

                return View();
           
        }
        public PartialViewResult MyCareTeam()
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                var favdoc = objDoctorRepo.MyCareTeam(SessionHandler.UserInfo.Id);
                foreach (var item in favdoc)
                {
                    if (item.picture != null && item.picture.Count() > 0)
                    {
                        item.ProfilePhotoBase64 = System.Text.Encoding.ASCII.GetString(item.picture);
                    }
                    else
                    {
                        item.ProfilePhotoBase64 = "data:image/png;base64,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";
                    }
                }
                return PartialView("MyCareTeamView", favdoc);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("MyCareTeamView");
        }

        public PartialViewResult PartialViewSurgery()
        {
            try
            {
                SurgeriesRepository oSurgeriesRepository = new SurgeriesRepository();
                var oAllSurgeries = oSurgeriesRepository.GetSurgeries();
                var oAllPAllSurgeries = oSurgeriesRepository.LoadPatientSurgeries(SessionHandler.UserInfo.Id);

                List<PSurgeries> oSurgeires = new List<PSurgeries> { };
                foreach (var item in oAllPAllSurgeries)
                {
                    oSurgeires.Add(new PSurgeries { surgeryID = item.surgeryID, patientID = item.patientID, bodyPart = item.bodyPart });
                }
                foreach (var item in oAllSurgeries)
                {
                    var flag = oSurgeires.Where(os => os.bodyPart == item.surgeryName).FirstOrDefault();
                    if (flag == null)
                    {
                        oSurgeires.Add(new PSurgeries { surgeryID = 0, patientID = 0, bodyPart = item.surgeryName });
                    }

                }
                return PartialView("PartialViewSurgery", oSurgeires);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
                return PartialView("PartialViewSurgery");
            }

        }

        [HttpPost]
        // Languages
        public JsonResult GetAllLanguages()
        {
            try
            {
                List<Languages> languages = new List<Languages>();
                var objLanguageRepo = new LanguageRepository();
                languages = objLanguageRepo.Get().ToList();
                return Json(new { Success = true, Object = languages });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }

        //Specialities
        [HttpPost]
        
        public JsonResult GetAllSpecialities()
        {
            try
            {
                List<Specialities> specialities = new List<Specialities>();
                var objSpecialityRepo = new SpecialityRepository();
                specialities = objSpecialityRepo.Get().ToList();
                return Json(new { Success = true, Object = specialities });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
       
        //[HttpPost]
        //public JsonResult SearchDoctor(SearchDoctorModel model)
        //{
        //    SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
        //    //IEnumerable<SeeDoctorDTO> docList= objDoctorRepo.SeeDoctor(model.Doctor.firstName, model.Gender, model.Language, model.Speciallity, model.AppDate.DayOfWeek.ToString(), model.Timing.seacrhTime);
        //    try
        //    {
        //        if (model.gender == "ALL") { model.gender = null; }
        //        if (model.name == "") { model.name = null; }
        //        if (model.language == "ALL") { model.language = null; }
        //        if (model.speciality == "ALL") { model.speciality = null; }
        //        if (model.appTime.ToString() == "") { model.appTime = null; }
        //        model.patientID = SessionHandler.UserInfo.Id;
        //        List<DoctorDataset> doctorList = objSeeDoctorRepo.SeeDoctor(model);
        //        return Json(new { Success = true, DoctorModel = doctorList });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = ex.Message });
        //    }


        //}
        [HttpPost]
        public JsonResult SearchDoctor(SearchDoctorModel model)
        {
            SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
            //IEnumerable<SeeDoctorDTO> docList= objDoctorRepo.SeeDoctor(model.Doctor.firstName, model.Gender, model.Language, model.Speciallity, model.AppDate.DayOfWeek.ToString(), model.Timing.seacrhTime);
            try
            {
                if (model.gender == "ALL") { model.gender = null; }
                if (model.name == "") { model.name = null; }
                if (model.language == "ALL") { model.language = null; }
                if (model.speciality == "ALL") { model.speciality = null; }
                if (model.appTime == "ALL") { model.appTime = null; }
                model.patientID = SessionHandler.UserInfo.Id;
                SearchDoctorResult doctorList = objSeeDoctorRepo.SeeDoctorWithShift(model);
                foreach(var item in doctorList.doctor)
                {
                    if (item.picture != null && item.picture.Count() > 0)
                    {
                        item.ProfilePhotoBase64 = System.Text.Encoding.ASCII.GetString(item.picture);
                    }
                    else
                    {
                        item.ProfilePhotoBase64 = "data:image/png;base64,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";
                    }
                }
                return Json(new { Success = true, DoctorModel = doctorList });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }


        }
        [HttpPost]
        public JsonResult FetchDoctorTimings(FetchTimingsModel model)
        {
            try
            {
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                List<FetchDoctorTimingModel> appList = new List<FetchDoctorTimingModel>();
                appList = objSeeDoctorRepo.FetchDoctorTimes(model);
                List<string> timings = new List<string>();
                if (appList != null)
                {
                    //calculate time slots
                    timings = displayTimeSlots(appList);
                }

                return Json(new { Success = true, Object = timings });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }

        [HttpPost]
        public JsonResult SaveAppointment(string inputModel)
        {
            try
            {
                AppointmentModel _objAppointment =  JsonConvert.DeserializeObject<AppointmentModel>(inputModel);

                ApiResultModel apiresult = new ApiResultModel();
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                apiresult = objSeeDoctorRepo.AddAppointment(_objAppointment);
                return Json(new { Success = true, ApiResultModel = apiresult });
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }
        }

        private List<string> displayTimeSlots(IEnumerable<FetchDoctorTimingModel> appList)
        {
            List<string> timeSlots = new List<string> { };

            foreach (var item in appList)
            {
                TimeSpan startTime = (TimeSpan)item.from;
                if (startTime.Minutes % 15 != 0)
                {
                    TimeSpan tempp = TimeSpan.FromMinutes(15- (startTime.Minutes % 15));
                    startTime = startTime.Add(tempp);
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
                        TimeSpan temppp = TimeSpan.FromMinutes(15);
                        startTime = startTime.Add(temppp);

                    }
                }
                TimeSpan endTime = (TimeSpan)item.to;
                if (endTime.Minutes % 15 != 0)
                {
                    TimeSpan tempp = TimeSpan.FromMinutes(15 - (endTime.Minutes % 15));
                    endTime = endTime.Add(tempp);
                    
                }

                TimeSpan itemstartTime = startTime;//(TimeSpan)item.from;
                
                if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                {
                    timeSlots.Add(startTime.ToString(@"hh\:mm"));
                    TimeSpan tempp = TimeSpan.FromMinutes(15);
                    startTime = startTime.Add(tempp);

                }
                bool flag = true;
                while (flag)
                {
                    if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                    {
                        //if (!(TimeSpan.Equals(slot, item.appTime)))
                        //{
                        timeSlots.Add(startTime.ToString(@"hh\:mm"));
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

                        if (!(timeSlots.Contains(startTime.ToString(@"hh\:mm"))))
                        {
                            timeSlots.Add(startTime.ToString(@"hh\:mm"));
                            TimeSpan tempp = TimeSpan.FromMinutes(15);
                            startTime = startTime.Add(tempp);

                        }
                        flag = false;
                    }
                    if(startTime.Hours == endTime.Hours)
                    {
                        //if (endTime.Minutes % 15 == 0)
                        //{
                        //    if (!(timeSlots.Contains(endTime.ToString(@"hh\:mm"))))
                        //    {
                        //        timeSlots.Add(endTime.ToString(@"hh\:mm"));
                        //    }
                        //        flag = false;
                        //}
                        //else
                        //{
                        //    flag = false;
                        //}
                        if (startTime.Minutes > endTime.Minutes)
                        {

                            //if (!(timeSlots.Contains(endTime.ToString(@"hh\:mm"))))
                            //{
                            //    timeSlots.Add(endTime.ToString(@"hh\:mm"));
                            //}
                            flag = false;
                        }
                    }
                    
                    //if ((timeSlots.Contains(itemstartTime.ToString(@"hh\:mm"))) && (timeSlots.Contains(endTime.ToString(@"hh\:mm"))))
                    //{
                    //    flag = false;
                    //}
                } //while end 
            }//for loop for database records.



            foreach (var app in appList)
            {
                if(app.appTime.HasValue)
                {
                    TimeSpan apptime = TimeSpan.Parse(app.appTime.Value.ToString());
                    if (timeSlots.Contains(apptime.ToString(@"hh\:mm")))
                    {
                        timeSlots.Remove(apptime.ToString(@"hh\:mm"));
                    }
                }
               
               
            }
           for(var i=0;i<timeSlots.Count;i++)
            {
                TimeSpan doctimings = TimeSpan.Parse(timeSlots[i]);
                var dateTime = new DateTime(doctimings.Ticks); // Date part is 01-01-0001
                var formattedTime = dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                timeSlots.RemoveAt(i);
                timeSlots.Insert(i, formattedTime);
                
            }
           if(timeSlots.Count>0)
            {
                timeSlots.RemoveAt(timeSlots.Count - 1);
            }
           
            return timeSlots;
        }

        [HttpPost]
        public JsonResult GetPatientROV(long patientid)
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                PatientROV rov = objDoctorRepo.LoadROV(patientid);
                return Json(new { Success = true, Object = rov });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult GetPatientChiefComplaints(long patientid)
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                PatientROV chiefComplaints = objDoctorRepo.GetPatientChiefComplaints(patientid);
                return Json(new { Success = true, Object = chiefComplaints });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult GetFavDoctors(long patientID)
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                var favdoc = objDoctorRepo.LoadFavDoctors(patientID);
                return Json(new { Success = true, Object = favdoc });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult GetROVList()
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                List<ROV_Custom> rov = objDoctorRepo.LoadROVList();
                return Json(new { Success = true, Object = rov });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult AddFavourite(FavouriteDoctorModel model)
        {
            try
            {

                SeeDoctorRepository objRepo = new SeeDoctorRepository();
               
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult = objRepo.AddFavourite(model);
                    return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult UpdateFavourite(FavouriteDoctorModel model)
        {
            try
            {

                SeeDoctorRepository objRepo = new SeeDoctorRepository();

                ApiResultModel apiresult = new ApiResultModel();
                apiresult = objRepo.UpdateFavourite(model);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult GetHealthConditions(long patientid)
        {
            try
            {
                ConditionRepository objRepo = new ConditionRepository();
                List<GetPatientConditions> model = objRepo.LoadHealthConditions(patientid);

                return Json(new { Success = true, Conditions = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
               
            }
        }

        [HttpPost]
        public JsonResult AddUpdateCondition(long conditionID, PatientConditions_Custom condition)
        {
            try
            {
                if (condition.conditionName == null || condition.conditionName == "" || !Regex.IsMatch(condition.conditionName, "^[0-9a-zA-Z ]+$"))
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Invalid condition name.Only letters and numbers are allowed.";
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                ConditionRepository objRepo = new ConditionRepository();
                if (conditionID == 0)
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult=objRepo.AddCondition(condition);
                    return Json(new { Success = true, ApiResultModel = apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditCondition(conditionID, condition);
                    return Json(new { Success = true, ApiResultModel =apiresult });
                }


            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }
        [HttpPost]
        public JsonResult DeleteCondition(long conditionID)
        {
            try
            {
                ConditionRepository objRepo = new ConditionRepository();
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = objRepo.DeleteCondition(conditionID);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }

        //Patient Medication
        [HttpPost]
        public JsonResult GetMedicines(string prefix)
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                List<MedicineModel> model = objRepo.GetMedicines(prefix);

                return Json(new { Success = true, Medicines = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult GetFrequency()
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                List<Frequency> model = objRepo.GetFrequency();

                return Json(new { Success = true, Frequency = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult FetchDoctoInfo(long doctorID)
        {
            try
            {
                SeeDoctorRepository objRepo = new SeeDoctorRepository();
                GetDoctorINFOVM model = objRepo.GetDoctorInfo(doctorID);

                return Json(new { Success = true, Object = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult GetMedications(long patientid)
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                List<GetMedication> model = objRepo.LoadMedications(patientid);

                return Json(new { Success = true, Medications = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult AddUpdateMedications(long mid, PatientMedication_Custom medication)
        {
            try
            {
                if (medication.medicineName == null || medication.medicineName == "")
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Invalid medicine name.";
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                MedicationRepository objRepo = new MedicationRepository();
                if (mid == 0)
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    medication.patientId = SessionHandler.UserInfo.Id;
                    apiresult = objRepo.AddMedication(medication);
                    return Json(new { Success = true, ApiResultModel = apiresult });

                }
                else
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    medication.patientId = SessionHandler.UserInfo.Id;
                    apiresult = objRepo.EditMedication(mid,medication);
                    return Json(new { Success = true, ApiResultModel= apiresult });
                }


            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }
        [HttpPost]
        public JsonResult DeleteMedications(long medicationID)
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                ApiResultModel apiresult = new ApiResultModel();
                apiresult = objRepo.DeleteMedication(medicationID);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }


        //Allergies
        [HttpPost]
        public JsonResult GetAllergies(string prefix)
        {
            try
            {
                AllergiesRepository objRepo = new AllergiesRepository();
                var allergies = objRepo.GetAllergies(prefix);

                return Json(allergies, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        //GetSensitivities
        [HttpPost]
        public JsonResult GetSensitivities()
        {
            try
            {
                AllergiesRepository objRepo = new AllergiesRepository();
                List<SensitivityModel> model = objRepo.GetSensitivities();

                return Json(new { Success = true, Object = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }

        //GetReactions
        [HttpPost]
        public JsonResult GetReactions()
        {
            try
            {
                AllergiesRepository objRepo = new AllergiesRepository();
                List<ReactionModel> model = objRepo.GetReactions();

                return Json(new { Success = true, Object = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult LoadPatientAllergies(long patientid)
        {
            try
            {

                var objRepo = new AllergiesRepository();
                List<GetPatientAllergies> pallergies = objRepo.LoadPatientAllergies(patientid);
                return Json(new { Success = true, Allergies = pallergies });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult AddUpdateAllergies(long allergiesID, PatientAllergies_Custom allergy)
        {
            try
            {
                if (allergy.allergyName == null || allergy.allergyName == "" || !Regex.IsMatch(allergy.allergyName, "^[0-9a-zA-Z ]+$"))
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Invalid allergy name.Only letters and numbers are allowed.";

                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                AllergiesRepository objRepo = new AllergiesRepository();
                if (allergiesID == 0)
                {
                    allergy.patientID = SessionHandler.UserInfo.Id;
                    ApiResultModel apiresult = objRepo.AddPatientAllergy(allergy);
                    return Json(new { Success = true, ApiResultModel = apiresult });

                }
                else
                {
                    allergy.patientID = SessionHandler.UserInfo.Id;
                    ApiResultModel apiresult = objRepo.EditPatientAllergy(allergiesID, allergy);
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }


            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }
        [HttpPost]
        public JsonResult DeleteAllergy(long allergyID)
        {
            try
            {
                AllergiesRepository objRepo = new AllergiesRepository();
                ApiResultModel apiresult = objRepo.DeletePatientAllergy(allergyID);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }


        //Patient Surgeries
        [HttpPost]
        public JsonResult GetSurgeries()
        {
            try
            {
                SurgeriesRepository objRepo = new SurgeriesRepository();
                var surgeries = objRepo.GetSurgeries();

                return Json(surgeries, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult AutocompleteSurgery(string prefix)
        {
            try
            {
                SurgeriesRepository objRepo = new SurgeriesRepository();
                var surgeries = objRepo.AutocompleteSurgery(prefix);

                return Json(surgeries, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }

        //[HttpPost]
        //public JsonResult LoadPatientSurgeries(long patientid)
        //{
        //    try
        //    {
        //        List<GetPatientSurgeries> psurgeries = new List<GetPatientSurgeries>();
        //        var objRepo = new SurgeriesRepository();
        //        psurgeries = objRepo.LoadPatientSurgeries(patientid);
        //        return Json(new { Success = true, Surgeries = psurgeries });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [PatientSessionExpire]
        [HttpPost]
        public JsonResult AddUpdateSurgeries(long surgeryID, PatientSurgery_Custom surgery)
        {
            try
            {
                if (surgery.bodyPart == null || surgery.bodyPart == "")//!Regex.IsMatch(surgery.bodyPart, @"^[a-zA-Z\s]+$"))
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Provide surgery name.";
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                SurgeriesRepository objRepo = new SurgeriesRepository();
                if (surgeryID == 0)
                {
                    ApiResultModel apiresult = objRepo.AddPatientSurgery(surgery);
                    return Json(new { Success = true, ApiResultModel = apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditPatientSurgery(surgeryID, surgery);
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }


            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }

        [HttpPost]
        public JsonResult GetPatientPharmacy(long patientID)
        {
            try
            {
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
               var pharmacy = objSeeDoctorRepo.GetPatientPharmacy(patientID);
               return Json(new { Success = true, Object = pharmacy });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }
        [HttpPost]
        public JsonResult SavePatientPharmacy(PatientPharmacy_Custom model)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                 apiresult = objSeeDoctorRepo.SavePatientPharmacy(model);
               
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }

        [HttpPost]
        public JsonResult DeleteSurgery(long surgeryID)
        {
            try
            {
                SurgeriesRepository objRepo = new SurgeriesRepository();
                ApiResultModel apiresult = objRepo.DeletePatientSurgery(surgeryID);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response.ReasonPhrase });
            }

        }

        [HttpPost]
        public JsonResult AutoCompleteMedicine(string prefix)
        {

            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                var medicine = objRepo.GetMedicines(prefix);
                return Json(medicine, JsonRequestBehavior.AllowGet);

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }
            //return Json(customers);
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
        public PartialViewResult DoctorProfile(long doctorID)
        {
            try
            {
                ProfileRepository oProfileRepository = new ProfileRepository();
                var oModel = oProfileRepository.GetDoctorProfileWithAllValues(doctorID);
                if (oModel != null)
                {
                    oModel.ConvertByteArrayToBase64();

                }

                return PartialView("DoctorProfileView", oModel);

            }

            catch (System.Web.Http.HttpResponseException ex)
            {
                ViewBag.Error = ex.Response.ReasonPhrase.ToString();
                ViewBag.Success = "";
            }
            return PartialView("DoctorProfileView");
        }

        #region Stripe Pay

        [HttpPost]
        public string ProceedWithPay(string tokenId)
        {
            var isSuceed = Helper.StripePayHelper.PerformStripeCharge(tokenId, 2000);

            if (isSuceed)
            {
                //Send Simple Email

                var sampleEmailBody = @"
                <h3>Thankyou for payment.</h3>
                <p>Vivamus et pellentesque velit. Morbi nec nisl at tellus placerat finibus. Pellentesque cursus id dui a dictum. Maecenas at augue sollicitudin, condimentum metus eu, sagittis arcu. Proin quis elit ac neque tincidunt egestas a eget enim. Aliquam a augue faucibus, gravida dui eget, semper ipsum. Mauris et luctus nunc. Cras pretium lorem et erat egestas sagittis.</p>
                <p>Cras placerat a enim et malesuada. Suspendisse eu sapien ultricies, commodo nulla quis, pharetra metus. Proin tempor eros id dui malesuada malesuada. Vivamus at tempus elit. Aliquam erat volutpat. Donec ultricies tortor tortor, ac aliquam diam pretium dignissim. Sed lobortis libero sed neque luctus, quis pellentesque nulla aliquet. Aliquam a nisi lobortis orci pretium tincidunt. Donec ac erat eget massa volutpat ornare ut id nunc.</p>
                <p>&nbsp;</p>
                <p><strong>-Best Regards,<br/>Sender Name</strong></p>
                ";

                var oSimpleEmail = new Helper.EmailHelper("syed_jamshed_ali@yahoo.com", "Payment successful.", sampleEmailBody);
                oSimpleEmail.SendMessage();
            }

            return isSuceed ? "succeed" : "failed";
        }

        #endregion


        #region Pharmacy Search

        [HttpPost]
        public PartialViewResult SearchPharmacy(DoseSpotPharmacySearch oModel)
        {
            var oDoseSpotRepo = new DoseSpotRepository();
            var oRes = oDoseSpotRepo.GetPharmacySearchResult(oModel);

            return PartialView("SearchPharmacyView", oRes);
        }


        #endregion

    }
}
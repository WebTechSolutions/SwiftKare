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

namespace WebApp.Controllers
{
    [Authorize(Roles = "Patient")]
    public class SeeDoctorController : Controller
    {
       // GET: SeeDoctor
        public ActionResult SeeDoctor()
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
                ViewBag.PatienID = SessionHandler.UserInfo.Id;

                ViewBag.PublisherKey = ConfigurationManager.AppSettings["StripePayPublisherKey"].ToString();
                ViewBag.Amount = 2000;

                return View();
            }
        }
        public PartialViewResult MyCareTeam()
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                var favdoc = objDoctorRepo.MyCareTeam(SessionHandler.UserInfo.Id);

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
                SurgeriesRepository oSurgeriesRepository=new SurgeriesRepository();
                var oAllSurgeries = oSurgeriesRepository.GetSurgeries();
                var oAllPAllSurgeries = oSurgeriesRepository.LoadPatientSurgeries(SessionHandler.UserInfo.Id);
               
                List<PSurgeries> oSurgeires = new List<PSurgeries> { };
                foreach (var item in oAllPAllSurgeries)
                {
                    oSurgeires.Add(new PSurgeries {surgeryID=item.surgeryID, patientID = item.patientID,bodyPart=item.bodyPart});
                }
                foreach (var item in oAllSurgeries)
                {
                    var flag = oSurgeires.Where(os => os.bodyPart == item.surgeryName).FirstOrDefault();
                    if (flag == null)
                    {
                        oSurgeires.Add(new PSurgeries { surgeryID =0,patientID = 0, bodyPart = item.surgeryName});
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
                if (model.appTime.ToString() == "") { model.appTime = null ; }
                List<DoctorDataset> doctorList = objSeeDoctorRepo.SeeDoctor(model);
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
        public JsonResult SaveAppointment(AppointmentModel _objAppointment)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                apiresult = objSeeDoctorRepo.AddAppointment(_objAppointment);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
               
                TimeSpan itemstartTime = (TimeSpan)item.from;
                TimeSpan endTime = (TimeSpan)item.to;
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
                // if (doctimings.Hours<12)
                // {
                //     timeSlots.RemoveAt(i);
                //     timeSlots.Insert(i, doctimings.ToString(@"hh\:mm") + " AM");
                // }
                //else if(doctimings.Hours >= 12)
                // {
                //     timeSlots.RemoveAt(i);
                //     timeSlots.Insert(i, doctimings.ToString(@"hh\:mm") + " PM");
                // }
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
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
                return Json(new { Success = true, ApiResultModel= apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
        public JsonResult AddUpdateMedications(long mid,PatientMedication_Custom medication)
        {
            try
            {
                if (medication.medicineName == null || medication.medicineName == "" || !Regex.IsMatch(medication.medicineName, "^[0-9a-zA-Z ]+$"))
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Invalid medicine name.Only letters and numbers are allowed.";
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                MedicationRepository objRepo = new MedicationRepository();
                if (mid == 0)
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult = objRepo.AddMedication(medication);
                    return Json(new { Success = true, ApiResultModel =apiresult });

                }
                else
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult = objRepo.EditMedication(mid,medication);
                    return Json(new { Success = true, ApiResultModel= apiresult });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
                return Json(new { Success = true, ApiResultModel= apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
        public JsonResult AddUpdateAllergies(long allergiesID,PatientAllergies_Custom allergy)
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
                    ApiResultModel apiresult = objRepo.AddPatientAllergy(allergy);
                    return Json(new { Success = true, ApiResultModel= apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditPatientAllergy(allergiesID,allergy);
                    return Json(new { Success = true, ApiResultModel =apiresult });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteAllergy(long allergyID)
        {
            try
            {
                AllergiesRepository objRepo = new AllergiesRepository();
                ApiResultModel apiresult = objRepo.DeletePatientAllergy(allergyID);
                return Json(new { Success = true, ApiResultModel= apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }


        //Patient Surgeries
        [HttpPost]
        public JsonResult GetSurgeries()
        {
            try
            {
                SurgeriesRepository objRepo = new SurgeriesRepository();
               var surgeries= objRepo.GetSurgeries();

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
                    return Json(new { Success = true, ApiResultModel =apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditPatientSurgery(surgeryID, surgery);
                    return Json(new { Success = true, ApiResultModel =apiresult });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult AddUpdatePharmacy(PatientPharmacy_Custom pharmacy)
        {
            ApiResultModel apiresult = new ApiResultModel();
            try
            {
                if (pharmacy.pharmacy == null || pharmacy.pharmacy == "" || !Regex.IsMatch(pharmacy.pharmacy, "^[0-9a-zA-Z ]+$"))
                {
                    
                    apiresult.message = "Invalid pharmacy name.Only letters and numbers are allowed.";
                    return Json(new { Success = true, ApiResultModel = apiresult });
                }
                else
                {
                    PatientRepository objRepo = new PatientRepository();
                    apiresult = objRepo.AddPharmacy(pharmacy);
                    return Json(new { Success = true, ApiResultModel = apiresult });

                }

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult DeleteSurgery(long surgeryID)
        {
            try
            {
                SurgeriesRepository objRepo = new SurgeriesRepository();
                ApiResultModel apiresult = objRepo.DeletePatientSurgery(surgeryID);
                return Json(new { Success = true, ApiResultModel= apiresult });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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
        
    }
}
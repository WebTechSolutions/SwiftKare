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

namespace WebApp.Controllers
{
    public class SeeDoctorController : Controller
    {
       // GET: SeeDoctor
        public ActionResult SeeDoctor()
        {
            ViewBag.PatienID = 10015;
            return View();
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
                if (model.appDate.ToString() == "") { model.appDate = null; }
                if (model.appTime.ToString() == "") { model.appTime = null ; }
                List<DoctorModel> doctorList = objSeeDoctorRepo.SeeDoctor(model);
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
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult SaveAppointment(AppointmentModel model)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                apiresult = objSeeDoctorRepo.AddAppointment(model);
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
                } //while end 
            }//for loop for database records.



            foreach (var app in appList)
            {
                if (timeSlots.Contains(app.appTime.ToString()))
                {
                    timeSlots.Remove(app.appTime.ToString());
                }
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
        public JsonResult GetMedicines()
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                List<MedicineModel> model = objRepo.GetMedicines();

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
                List<DoctorInfoCustom> model = objRepo.GetDoctorInfo(doctorID);

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


        //Patient Allergies
        [HttpPost]
        public JsonResult GetAllergies()
        {
            try
            {
               AllergiesRepository objRepo = new AllergiesRepository();
               List<AllergiesModel> model = objRepo.GetAllergies();

                return Json(new { Success = true, Allergies = model });

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
                List<SurgeriesModel> model = objRepo.GetSurgeries();

                return Json(new { Success = true, Surgeries = model });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult LoadPatientSurgeries(long patientid)
        {
            try
            {
                List<GetPatientSurgeries> psurgeries = new List<GetPatientSurgeries>();
                var objRepo = new SurgeriesRepository();
                psurgeries = objRepo.LoadPatientSurgeries(patientid);
                return Json(new { Success = true, Surgeries = psurgeries });
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddUpdateSurgeries(long surgeryID, PatientSurgery_Custom surgery)
        {
            try
            {
                if (surgery.bodyPart == null || surgery.bodyPart == "" || !Regex.IsMatch(surgery.bodyPart, "^[0-9a-zA-Z ]+$"))//!Regex.IsMatch(surgery.bodyPart, @"^[a-zA-Z\s]+$"))
                {
                    ApiResultModel apiresult = new ApiResultModel();
                    apiresult.message = "Invalid body part name.Only letters and numbers are allowed.";
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
     

    }
}
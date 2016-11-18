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
                List<DoctorModel> doctorList = objSeeDoctorRepo.SeeDoctor(model);
                return Json(new { Success = true, DoctorList = doctorList });

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
                SeeDoctorRepository objSeeDoctorRepo = new SeeDoctorRepository();
                long appID = objSeeDoctorRepo.AddAppointment(model);
                return Json(new { Success = true, appID = appID });

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
                if (!(timeSlots.Contains(startTime.ToString())))
                {
                    timeSlots.Add(startTime.ToString());
                    TimeSpan tempp = TimeSpan.FromMinutes(15);
                    startTime = startTime.Add(tempp);

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
        public JsonResult GetROV(long patientid)
        {
            try
            {
                SeeDoctorRepository objDoctorRepo = new SeeDoctorRepository();
                AppointmentModel rov = objDoctorRepo.LoadROV(patientid);
                return Json(new { Success = true, Object = rov });
                             
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

        
        public JsonResult AddUpdateCondition(long conditionID, PatientConditions_Custom condition)
        {
            try
            {
                if (condition.conditionName == null || condition.conditionName == "" || !Regex.IsMatch(condition.conditionName, @"^[a-zA-Z\s]+$"))
                {
                    return Json(new { Success = true, ConditionID = -1 });
                }
                ConditionRepository objRepo = new ConditionRepository();
                if (conditionID == 0)
                {
                    ApiResultModel apiresult = objRepo.AddCondition(condition);
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

        public JsonResult DeleteCondition(long conditionID)
        {
            try
            {
                ConditionRepository objRepo = new ConditionRepository();
                ApiResultModel apiresult = objRepo.DeleteCondition(conditionID);
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

        public JsonResult AddUpdateMedications(long mid,PatientMedication_Custom medication)
        {
            try
            {
                if (medication.medicineName == null || medication.medicineName == "" || !Regex.IsMatch(medication.medicineName, @"^[a-zA-Z\s]+$"))
                {
                    return Json(new { Success = true, ConditionID = -1 });
                }
                MedicationRepository objRepo = new MedicationRepository();
                if (mid == 0)
                {
                    ApiResultModel apiresult = objRepo.AddMedication(medication);
                    return Json(new { Success = true, ApiResultModel =apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditMedication(mid,medication);
                    return Json(new { Success = true, ApiResultModel= apiresult });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }

        public JsonResult DeleteMedications(long medicationID)
        {
            try
            {
                MedicationRepository objRepo = new MedicationRepository();
                ApiResultModel apiresult = objRepo.DeleteMedication(medicationID);
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

        public JsonResult AddUpdateAllergies(long allergiesID,PatientAllergies_Custom allergy)
        {
            try
            {
                if (allergy.allergyName == null || allergy.allergyName == "" || !Regex.IsMatch(allergy.allergyName, @"^[a-zA-Z\s]+$"))
                {
                    return Json(new { Success = true, AllergyID = -1 });
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

        public JsonResult AddUpdateSurgeries(long id,PatientSurgery_Custom surgery)
        {
            try
            {
                if (surgery.bodyPart == null || surgery.bodyPart == "" || !Regex.IsMatch(surgery.bodyPart, @"^[a-zA-Z\s]+$"))
                {
                    return Json(new { Success = true, SurgeryID = -1 });
                }
                SurgeriesRepository objRepo = new SurgeriesRepository();
                if (id == 0)
                {
                    ApiResultModel apiresult = objRepo.AddPatientSurgery(surgery);
                    return Json(new { Success = true, ApiResultModel =apiresult });

                }
                else
                {
                    ApiResultModel apiresult = objRepo.EditPatientSurgery(id,surgery);
                    return Json(new { Success = true, ApiResultModel =apiresult });
                }


            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }

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
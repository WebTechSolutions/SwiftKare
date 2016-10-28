using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;

namespace WebApp.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }

        // GET: SeeDoctor
        public ActionResult SeeDoctor()
        {
            var Model = new SeeDoctorViewModel();
            var langRepo = new LanguageRepository();
            var specRepo = new SpecialityRepository();

            IEnumerable<Language> language =langRepo.Get();
            SelectList langlist = new SelectList(language, "languageId", "languageName");
            Model.LanguageList = langlist;
            IEnumerable<Speciallity> speciality = specRepo.Get();
            SelectList speclist = new SelectList(speciality, "speciallityID", "specialityName");
            Model.SpeciallityList = speclist;
                     
            return View(Model);
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

        [HttpPost]
        public ActionResult Search(SeeDoctorViewModel model)
        {
            var selecteItem = model.Gender.Where(item => item.Selected).FirstOrDefault();
            return View();
        }
    }
}
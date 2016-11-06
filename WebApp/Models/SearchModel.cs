using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class SearchModel
    {
        public IEnumerable<SelectListItem> LanguageList { get; set; }
        public string Language { get; set; }
        public IEnumerable<SelectListItem> SpeciallityList { get; set; }
        public string Speciallity { get; set; }
        public TimeSpan docTime { get; set; }
        public string Gender { get; set; }
        public DateTime AppDate { get; set; }
        public string DoctorName { get; set; }
        public IEnumerable<SelectListItem> GenderList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem> { new SelectListItem() { Text = "ALL", Value = "ALL" }, new SelectListItem() { Text = "Male", Value = "Male" }, new SelectListItem() { Text = "Female", Value = "Female" } };
                return list.Select(l => new SelectListItem { Selected = (l.Value == Gender), Text = l.Text, Value = l.Value });
            }
        }
    }
}
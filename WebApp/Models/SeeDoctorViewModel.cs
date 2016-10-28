using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class SeeDoctorViewModel
    {
        public IEnumerable<SelectListItem> LanguageList { get; set; }
        public Language Language { get; set; }
        public IEnumerable<SelectListItem> SpeciallityList { get; set; }
        public Speciallity Speciallity { get; set; }
        public IEnumerable<Doctor> DoctorList { get; set; }
        public DoctorTiming DoctorTiming { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime AppDate { get; set; }
        public Time Timing { get; set; } = new Time();
        public string SelectedValue { get; set; }
        public IEnumerable<SelectListItem> Gender
        {
            get
            {
                return new[]
                {
                new SelectListItem { Value = "1", Text = "All" },
                new SelectListItem { Value = "2", Text = "Male" },
                new SelectListItem { Value = "3", Text = "Female" },
            };
            }
        }

    }
    public class Time
    {
        public string seacrhTime { get; set; } 
        
    }
}
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
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }

        
    }
}
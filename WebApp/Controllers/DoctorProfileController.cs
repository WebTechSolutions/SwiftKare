using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;
using WebApp.Helper;
using System;
using System.Globalization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorProfileController : Controller
    {
        //DoctorTimingsRepository objTimingRepo = new DoctorTimingsRepository();

        public ActionResult Index()
        {
            return View();
        }


    }

}
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        DoctorTimingsRepository objTimingRepo = new DoctorTimingsRepository();
        // GET: DoctorJson
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DoctorTimings()
        {
            return View();
        }
        [HttpPost]
        // GET: Doctor Timings
        public JsonResult GetDoctorTimings()
        {
            var Model = new DoctorTimingsViewModel();
            var objRepo = new DoctorRepository();
            var userId = ApplicationGlobalVariables.Instance.UserId;
            var doctor = objRepo.GetByUserId(userId);
            Model.DoctorId = doctor.doctorID;
            Model.DoctorTimingsList = objTimingRepo.GetListByDoctorId(doctor.doctorID).ToList();
            Model.DoctorTiming = new DoctorTimingsModel();
            Model.DoctorTiming.doctorID = doctor.doctorID;
            Model.DoctorTiming.doctorTimingsID = 0;
            return Json(Model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateEditTimings(DoctorTimingsModel model)
        {
            var userName = ApplicationGlobalVariables.Instance.UserName;

            if (model.doctorTimingsID <= 0)
            {
                model.cb = userName;
                objTimingRepo.Add(model);
            }
            else
            {
                model.mb = userName;
                objTimingRepo.Put(model.doctorTimingsID, model);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDoctorTiming(long id)
        {
            objTimingRepo.Delete(id);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

    }

}
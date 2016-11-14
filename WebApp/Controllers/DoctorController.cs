using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Models;
using DataAccess.CustomModels;
using WebApp.Helper;

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
            Model.DoctorId = SessionHandler.UserInfo.Id;
            Model.DoctorTimingsList = objTimingRepo.GetListByDoctorId(Model.DoctorId).ToList();
            Model.DoctorTiming = new DoctorTimingsModel();
            Model.DoctorTiming.doctorID = Model.DoctorId;
            Model.DoctorTiming.doctorTimingsID = 0;
            return Json(Model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateEditTimings(DoctorTimingsModel model)
        {
            //jam
            var timingsList = objTimingRepo.GetListByDoctorId(model.doctorID);
            var alreadItems = timingsList.Where(o => o.day == model.day && o.from == model.from && o.to == model.to).ToList();
            if(alreadItems.Count<=0)
            {
                var userName = SessionHandler.UserName;

                if (model.doctorTimingsID <= 0)
                {
                    model.username = userName;
                    objTimingRepo.Add(model);
                }
                else
                {
                    model.username = userName;
                    objTimingRepo.Put(model.doctorTimingsID, model);
                }
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
using DataAccess;
using DataAccess.CustomModels;
using Identity.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.ProfileRepositories;

namespace WebApp.Controllers
{
    [PatientSessionExpire]
    [Authorize]
    public class NewsController : Controller
    {
        #region Declarations

        NewsRepository oNewsRepository;
        public NewsController()
        {
            oNewsRepository = new NewsRepository();
        }

        #endregion

        #region Action Methods

        public ActionResult Index()
        {
            var oAllData = oNewsRepository.GetNewsList();

            var userMgr = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userMgr.GetRoles(SessionHandler.UserId);
            if (roles.Contains("Doctor"))
            {
                return View(model:oAllData, viewName: "Index", masterName: "~/Views/Shared/_DoctorLayout.cshtml");
            }
            else {
                return View(model: oAllData, viewName: "Index", masterName: "~/Views/Shared/_PatientLayout.cshtml");
            }

        }

        public ActionResult NewsDetails(int id)
        {
            var oModel = oNewsRepository.GetNewsDetail(id);

            if (oModel != null) {

                var userMgr = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userMgr.GetRoles(SessionHandler.UserId);
                if (roles.Contains("Doctor"))
                {
                    return View(model: oModel, viewName: "NewsDetails", masterName: "~/Views/Shared/_DoctorLayout.cshtml");
                }
                else
                {
                    return View(model: oModel, viewName: "NewsDetails", masterName: "~/Views/Shared/_PatientLayout.cshtml");
                }
            }

            return RedirectToAction("Index");
        }

        #endregion


    }
}
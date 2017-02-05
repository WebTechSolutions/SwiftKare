using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.MessageRepositories;

namespace WebApp.Controllers
{
    [DoctorSessionExpire]
    [Authorize(Roles = "Doctor")]
   
    public class DoctorHelpTicketController : Controller
    {
        // GET: HelpTicket
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SendHelpTicket(HelpTicket _objemail)
        {
            try
            {
                ApiResultModel apiresult = new ApiResultModel();
                MessageRepository oMessageRepository = new MessageRepository();
                apiresult = oMessageRepository.SendHelpTicket(_objemail);
                return Json(new { Success = true, ApiResultModel = apiresult });

            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                return Json(new { Message = ex.Response });
            }

        }
    }
}
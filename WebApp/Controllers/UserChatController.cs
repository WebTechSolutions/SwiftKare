using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

namespace WebApp.Controllers
{
    public class UserChatController : Controller
    {
        // GET: UserChat
        public ActionResult Index()
        {
            if(HttpContext.Application["OpenTokSession"] == null)
            {
                HttpContext.Application["OpenTokSession"] = UserChatHelper.GenerateOpenTokSession();
                HttpContext.Application["OpenTokToken"] = UserChatHelper.GenerateOpenTokToken(Convert.ToString(HttpContext.Application["OpenTokSession"]));
            }

            ViewBag.OpenTokApiKey = UserChatHelper.TokBoxApiKey;
            ViewBag.OpenTokSession = Convert.ToString(HttpContext.Application["OpenTokSession"]);
            ViewBag.OpenTokToken = Convert.ToString(HttpContext.Application["OpenTokToken"]);

            return View();
        }
    }
}
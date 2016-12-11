using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

namespace WebApp.Controllers
{
    [Authorize]
    public class UserChatController : Controller
    {
        [HttpPost]
        public bool ReadyForCall(string senderId, string receiverId, string userType, string recipientName)
        {
            try
            {
                var openTokSession = UserChatHelper.GetOpenTokSessionInformation(senderId, receiverId, userType, recipientName);
                openTokSession.RecipientName = recipientName;
                if (openTokSession == null || string.IsNullOrEmpty(openTokSession.SessionId) || string.IsNullOrEmpty(openTokSession.TokenId))
                {
                    return false;
                }
                
                openTokSession.UserType = userType;

                openTokSession.UserType = userType;

                HttpContext.Session["MyOpenTokSession"] = openTokSession;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // GET: UserChat
        public ActionResult Index()
        {
            var openTokSession = (OpenTokSession)(HttpContext.Session["MyOpenTokSession"]);

            if (openTokSession == null || string.IsNullOrEmpty(openTokSession.SessionId) || string.IsNullOrEmpty(openTokSession.TokenId))
            {
                //Redirect user to appropriate page
            }

            ViewBag.UserType = openTokSession.UserType;
            ViewBag.RecipientName = openTokSession.RecipientName;
            ViewBag.OpenTokApiKey = UserChatHelper.TokBoxApiKey;
            ViewBag.OpenTokSession = openTokSession.SessionId;
            ViewBag.OpenTokToken = openTokSession.TokenId;

            return View();
        }
    }


}
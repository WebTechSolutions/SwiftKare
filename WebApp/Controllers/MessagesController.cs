using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Repositories.DoctorRepositories;

namespace WebApp.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        #region Declarations

        MessageRepository oMessageRepository;

        public MessagesController()
        {
            oMessageRepository = new MessageRepository();
        }

        #endregion


        #region Action Methods

        // GET: Index
        public ActionResult Index()
        {
            var inboxData = oMessageRepository.GetInboxMessage();

            return View(inboxData);
        }

        [HttpPost]
        public PartialViewResult GetMessageDetails(string msgId)
        {
            //TODO: get data by calling actual repo
            var oMsg = oMessageRepository.GetInboxMessage().FirstOrDefault();

            return PartialView("PartialMessageDetail", oMsg);
        }

        #endregion

    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;

namespace WebApp.Controllers
{
    public class SendGridDemoController : Controller
    {
        // GET: SendGridDemo
        public ActionResult Index()
        {
            //Send Simple Email

//            var sampleEmailBody = @"
//<h3>Mail From SendGrid</h3>
//<p>Vivamus et pellentesque velit. Morbi nec nisl at tellus placerat finibus. Pellentesque cursus id dui a dictum. Maecenas at augue sollicitudin, condimentum metus eu, sagittis arcu. Proin quis elit ac neque tincidunt egestas a eget enim. Aliquam a augue faucibus, gravida dui eget, semper ipsum. Mauris et luctus nunc. Cras pretium lorem et erat egestas sagittis.</p>
//<p>Cras placerat a enim et malesuada. Suspendisse eu sapien ultricies, commodo nulla quis, pharetra metus. Proin tempor eros id dui malesuada malesuada. Vivamus at tempus elit. Aliquam erat volutpat. Donec ultricies tortor tortor, ac aliquam diam pretium dignissim. Sed lobortis libero sed neque luctus, quis pellentesque nulla aliquet. Aliquam a nisi lobortis orci pretium tincidunt. Donec ac erat eget massa volutpat ornare ut id nunc.</p>
//<p>&nbsp;</p>
//<p><strong>-Best Regards,<br/>Sender Name</strong></p>
//";

            //var oSimpleEmail = new EmailHelper("hirenpatel2236@gmail.com", "Demo Mail From SendGrid", sampleEmailBody);
            //oSimpleEmail.SendMessage();

            //var oEmailWithAttachment = new EmailHelper("hirenpatel2236@gmail.com", "Demo Mail With Attachment From SendGrid", sampleEmailBody, Server.MapPath("~/Content/images/123.jpg"));
            //oEmailWithAttachment.SendMessage();

            var oCurrentUtcDate = DateTime.UtcNow;
            return View(oCurrentUtcDate);
        }
    }
}
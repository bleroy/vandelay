using System;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Themes;

namespace Vandelay.Industries.Controllers {
    [Themed]
    [OrchardFeature("Vandelay.Fail")]
    public class FailController : Controller {
        public ActionResult Index() {
            throw new InvalidOperationException("Something went purposefully horribly wrong. Don't worry, it's going to be ok.");
        }

        public ActionResult NotFound() {
            HttpContext.Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult Error() {
            HttpContext.Response.StatusCode = 500;
            return View("ErrorPage");
        }
    }
}
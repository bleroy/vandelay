using System;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Themes;

namespace Vandelay.Industries.Controllers {
    [Themed]
    [OrchardFeature("Vandelay.Fail")]
    public class FailController : Controller {
        public ActionResult NotFound() {
            return HttpNotFound();
        }

        public ActionResult Error() {
            throw new InvalidOperationException("Something went purposefully horribly wrong. Don't worry, it's going to be ok.");
        }
    }
}
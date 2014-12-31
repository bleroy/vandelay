using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Orchard;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Controllers {
    [SessionState(SessionStateBehavior.Required)]
    [OrchardFeature("Vandelay.ThemePicker")]
    public class SessionThemeController : Controller {
        private readonly WorkContext _workContext;

        public SessionThemeController(WorkContext workContext) {
            _workContext = workContext;
        }

        public ActionResult UseDefault() {
            SetUseDefault(true);
            return RedirectToHome();
        }

        public ActionResult ClearDefault() {
            SetUseDefault(false);
            return RedirectToHome();
        }

        private void SetUseDefault(bool value) {
            var session = _workContext.HttpContext.Session;
            if (session != null) {
                session[_workContext.CurrentSite.SiteName + "Vandelay.ThemePicker.UseDefault"]
                    = value;
            }
        }

        private static ActionResult RedirectToHome() {
            return new RedirectToRouteResult("",
                                             new RouteValueDictionary {
                                                 {"area", "HomePage"},
                                                 {"controller", "Home"},
                                                 {"action", "Index"}
                                             });
        }

    }
}
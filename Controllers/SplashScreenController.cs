using System;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using Vandelay.Industries.Models;
using Vandelay.Industries.ViewModels;

namespace Vandelay.Industries.Controllers {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenController : Controller {
        private readonly IWorkContextAccessor _wca;

        public SplashScreenController(IWorkContextAccessor wca) {
            _wca = wca;
        }

        [Themed]
        public ActionResult Index(string returnUrl = null) {
            var settings = _wca.GetContext().CurrentSite.As<SplashScreenSettingsPart>();
            var markdown = new MarkdownSharp.Markdown();

            return View(new SplashScreenViewModel {
                AcceptButtonText = settings.AcceptButtonText,
                RedirectUrl = settings.RedirectUrl,
                RejectButtonText = settings.RejectButtonText,
                ReturnUrl = returnUrl,
                SplashScreenContents = markdown.Transform(settings.SplashScreenContents)
            });
        }

        [HttpPost]
        public ActionResult Accept(string returnUrl = null) {
            if (Response != null) {
                var cookies = Response.Cookies;
                if (cookies != null) {
                    cookies.Add(new HttpCookie(SplashScreenSettingsPart.CookieName) {
                        Expires = DateTime.Now.AddYears(5),
                        HttpOnly = false,
                        Secure = false,
                        Value = "A"
                    });
                }
            }
            return Redirect(returnUrl ?? "~/");
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Reset(string returnUrl = null) {
            if (Response != null) {
                var cookies = Response.Cookies;
                if (cookies != null) {
                    cookies.Add(new HttpCookie(SplashScreenSettingsPart.CookieName) {
                        Expires = DateTime.Now.AddYears(-1),
                        HttpOnly = false,
                        Secure = false
                    });
                }
            }
            return Redirect(returnUrl ?? "~/");
        }
    }
}
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.UI.Admin;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Filters {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenFilter : FilterProvider, IResultFilter {
        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Splash screen itself is not subject to filtering
            var routeValues = filterContext.RouteData.Values;
            if (routeValues["area"] as string == "Vandelay.Industries" &&
                routeValues["controller"] as string == "SplashScreen") {
                return;
            }

            // Neither are requests to the account controller
            if (routeValues["area"] as string == "Orchard.Users" &&
                routeValues["controller"] as string == "Account") {
                return;
            }

            var httpContext = filterContext.HttpContext;
            if (httpContext == null || httpContext.Request == null) return;

            // don't try to put a splash screen on POST requests
            if (httpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) return;

            // Don't put a splash screen on admin, media, or theme resources
            var requestedPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
            if (requestedPath.StartsWith("~/media/", StringComparison.OrdinalIgnoreCase)) return;
            if (requestedPath.StartsWith("~/themes/", StringComparison.OrdinalIgnoreCase)) return;
            if (AdminFilter.IsApplied(new RequestContext(httpContext, new RouteData()))) return;

            // ignore child actions, e.g. HomeController is using RenderAction()
            if (filterContext.IsChildAction) return;

            var splashScreenCookie = httpContext.Request
                .Cookies[SplashScreenSettingsPart.CookieName];
            // If cookie exists, the user has been here and has accepted the terms of use.
            if (splashScreenCookie != null && !String.IsNullOrWhiteSpace(splashScreenCookie.Value)) return;
            var config = filterContext.GetWorkContext().CurrentSite.As<SplashScreenSettingsPart>();
            if (config == null || !config.Enabled) return;

            // If request is for an ignored URL, don't display a splash screen
            var ignoredUrls = config.IgnoredUrls;
            if (IsIgnoredUrl(httpContext.Request.AppRelativeCurrentExecutionFilePath, ignoredUrls)) return;

            // This is a new user, and the splash screen is enabled. Redirect to splash screen.
            filterContext.Cancel = true;
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                {"area", "Vandelay.Industries"},
                {"controller", "SplashScreen"},
                {"action", "Index"},
                {"returnUrl", httpContext.Request.Path}
            });
            filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {}

        /// <summary>
        /// Returns true if the given url should be ignored, as defined in the settings
        /// <remarks>Copied from OutputCacheFilter</remarks>
        /// </summary>
        private static bool IsIgnoredUrl(string url, string ignoredUrls) {
            if (String.IsNullOrEmpty(ignoredUrls)) {
                return false;
            }

            // remove ~ if present
            if (url.StartsWith("~")) {
                url = url.Substring(1);
            }

            using (var urlReader = new StringReader(ignoredUrls)) {
                string relativePath;
                while (null != (relativePath = urlReader.ReadLine())) {
                    // remove ~ if present
                    if (relativePath.StartsWith("~")) {
                        relativePath = relativePath.Substring(1);
                    }

                    if (String.IsNullOrWhiteSpace(relativePath)) {
                        continue;
                    }

                    relativePath = relativePath.Trim();

                    // ignore comments
                    if (relativePath.StartsWith("#")) {
                        continue;
                    }

                    if (String.Equals(relativePath, url, StringComparison.OrdinalIgnoreCase)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
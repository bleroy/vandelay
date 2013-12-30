using System;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Filters
{
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenFilter : FilterProvider, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // Splash screen itself is not subject to filtering
            var routeValues = filterContext.RouteData.Values;
            if (routeValues["area"] as string == "Vandelay.Industries" &&
                routeValues["controller"] as string == "SplashScreen") {
                return;
            }

            var httpContext = filterContext.HttpContext;
            if (httpContext == null) return;

            // Don't filter on admin, media, or theme resources
            var requestedPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
            if (requestedPath.StartsWith("~/media/", StringComparison.OrdinalIgnoreCase)) return;
            if (requestedPath.StartsWith("~/themes/", StringComparison.OrdinalIgnoreCase)) return;
            if (requestedPath.StartsWith("~/admin", StringComparison.OrdinalIgnoreCase)) return;

            var splashScreenCookie = httpContext.Request
                .Cookies[SplashScreenSettingsPart.CookieName];
            // If cookie exists, the user has been here and has accepted the terms of use.
            if (splashScreenCookie != null && !String.IsNullOrWhiteSpace(splashScreenCookie.Value)) return;
            var config = filterContext.GetWorkContext().CurrentSite.As<SplashScreenSettingsPart>();
            if (config == null || !config.Enabled) return;
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

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Vandelay.Industries.Routes {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "splashscreen",
                        new RouteValueDictionary {
                            {"area", "Vandelay.Industries"},
                            {"controller", "SplashScreen"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Vandelay.Industries"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
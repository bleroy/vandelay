using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Vandelay.Industries.Routes {
    [OrchardFeature("Vandelay.Fail")]
    public class FailRouteProvider : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {   Priority = 10,
                                                     Route = new Route(
                                                         "fail",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Vandelay.Industries"},
                                                                                      {"controller", "Fail"},
                                                                                      {"action", "Index"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Vandelay.Industries"}
                                                         },
                                                         new MvcRouteHandler())
                             },
                             new RouteDescriptor {   Priority = 10,
                                                     Route = new Route(
                                                         "notfound",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Vandelay.Industries"},
                                                                                      {"controller", "Fail"},
                                                                                      {"action", "NotFound"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Vandelay.Industries"}
                                                         },
                                                         new MvcRouteHandler())
                             },
                             new RouteDescriptor {   Priority = 10,
                                                     Route = new Route(
                                                         "error",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Vandelay.Industries"},
                                                                                      {"controller", "Fail"},
                                                                                      {"action", "Error"}
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
using System.Web.Routing;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Feedburner")]
    public class Feed {
        public string Title { get; set;}
        public string Format { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalUrl { get; set; }
    }
}
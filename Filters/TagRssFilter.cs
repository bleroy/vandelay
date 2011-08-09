using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.Core.Feeds;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Vandelay.Industries.Feeds;

namespace Vandelay.Industries.Filters {
    [OrchardFeature("Vandelay.TagRss")]
    public class TagRssFilter : FilterProvider, IResultFilter {
        private readonly IWorkContextAccessor _wca;
        private readonly IFeedManager _feedManager;

        public TagRssFilter(IWorkContextAccessor wca, IFeedManager feedManager) {
            _wca = wca;
            _feedManager = feedManager;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            var routeValues = filterContext.RouteData.Values;
            if (routeValues["area"] as string == "Orchard.Tags" &&
                routeValues["controller"] as string == "Home" &&
                routeValues["action"] as string == "Search") {

                var tag = routeValues["tagName"] as string;
                if (! string.IsNullOrWhiteSpace(tag)) {
                    var workContext = _wca.GetContext();
                    workContext.Layout.ListingItemsForTag = tag;
                    _feedManager.Register(
                        TagFeedQuery.GetTitle(tag, workContext.CurrentSite),
                        "rss",
                        new RouteValueDictionary { { "tag", tag } }
                        );
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }
    }
}
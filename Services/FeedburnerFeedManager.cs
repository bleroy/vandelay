using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Caching;
using Orchard.Core.Feeds;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    [OrchardSuppressDependency("Orchard.Core.Feeds.Services.FeedManager")]
    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerFeedManager : IFeedManager {
        private readonly List<Feed> _feeds = new List<Feed>();
        private readonly IEnumerable<IFeedDataProvider> _feedDataProviders;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public FeedburnerFeedManager(
            IEnumerable<IFeedDataProvider> feedDataProviders,
            ICacheManager cacheManager,
            ISignals signals) {

            _feedDataProviders = feedDataProviders;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public void Register(string title, string format, RouteValueDictionary values) {
            var key = _feedDataProviders
                .Select(p => p.GetKey(title, format, values))
                .FirstOrDefault(s => s != null);
            var feed = (key == null) ?
                BuildFeed(title, format, values) :
                _cacheManager.Get(
                    key,
                    s => {
                        s.Monitor(_signals.When(key));
                        var providedFeed = _feedDataProviders
                            .Select(p => p.GetFeed(title, format, values))
                            .Where(f => f != null)
                            .FirstOrDefault();
                        return providedFeed ?? BuildFeed(title, format, values);
                    });
            _feeds.Add(feed);
        }

        private static Feed BuildFeed(string title, string format, RouteValueDictionary values) {
            var link = new RouteValueDictionary(values) {{"format", format}};
            if (!link.ContainsKey("area")) {
                link["area"] = "Feeds";
            }
            if (!link.ContainsKey("controller")) {
                link["controller"] = "Feed";
            }
            if (!link.ContainsKey("action")) {
                link["action"] = "Index";
            }
            return new Feed {
                Title = title,
                Format = format,
                RouteValues = link
            };
        }

        public MvcHtmlString GetRegisteredLinks(HtmlHelper html) {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);

            var sb = new StringBuilder();
            foreach (var feed in _feeds) {
                var linkUrl = feed.IsExternal ?
                    feed.ExternalUrl :
                    urlHelper.RouteUrl(feed.RouteValues);
                sb.Append("\r\n<link rel=\"alternate\" type=\"application/" +
                    feed.Format.ToLowerInvariant() + "+xml\"");
                if (!string.IsNullOrEmpty(feed.Title)) {
                    sb.Append(" title=\"" + html.AttributeEncode(feed.Title) + "\"");
                }
                sb.Append(" href=\"" + html.AttributeEncode(linkUrl) + "\"/>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
using System;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    [OrchardFeature("Vandelay.Feedburner")]
    public class DefaultFeedDataProvider : IFeedDataProvider {
        private readonly IContentManager _contentManager;

        public DefaultFeedDataProvider(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public Feed GetFeed(string title, string format, RouteValueDictionary values) {
            if (!values.ContainsKey("containerid")) return null;
            var container = _contentManager.Get((int)values["containerid"]);
            if (container != null) {
                var feedburnerPart = container.As<FeedburnerPart>();
                if (feedburnerPart == null) return null;
                var link = new RouteValueDictionary(values) {
                    {"format", format},
                    {"area", "Feeds"},
                    {"controller", "Feed"},
                    {"action", "Index"}
                };
                return new Feed {
                    Title = title,
                    Format = format,
                    RouteValues = link,
                    ExternalUrl = feedburnerPart.FeedburnerUrl,
                    IsExternal = !String.IsNullOrWhiteSpace(feedburnerPart.FeedburnerUrl)
                };
            }
            return null;
        }

        public string GetKey(ContentItem item, string format) {
            return GetKey(item.Id, format);
        }

        public string GetKey(string title, string format, RouteValueDictionary values) {
            return !values.ContainsKey("containerid") ? null : GetKey((int)values["containerid"], format);
        }

        private static string GetKey(int id, string format) {
            return "Vandelay.Feedburner:containerid=" + id + ';' + format;
        }
    }
}
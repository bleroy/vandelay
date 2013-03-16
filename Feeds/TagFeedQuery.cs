using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Orchard;
using Orchard.Core.Feeds;
using Orchard.Core.Feeds.Models;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Orchard.Tags.Services;
using Orchard.Utility.Extensions;

namespace Vandelay.Industries.Feeds {
    [OrchardFeature("Vandelay.TagRss")]
    public class TagFeedQuery : IFeedQueryProvider, IFeedQuery {
        private readonly ITagService _tagService;
        private readonly IOrchardServices _services;

        public TagFeedQuery(ITagService tagService, IOrchardServices services) {
            _tagService = tagService;
            _services = services;
        }

        public FeedQueryMatch Match(FeedContext context) {
            var tagName = context.ValueProvider.GetValue("tag");
            if (tagName == null)
                return null;

            return new FeedQueryMatch { FeedQuery = this, Priority = -5 };
        }

        public void Execute(FeedContext context) {
            var tagValue = context.ValueProvider.GetValue("tag");
            if (tagValue == null) return;

            var tagName = (string)tagValue.ConvertTo(typeof (string));
            var tag = _tagService.GetTagByName(tagName);
            if (tag == null) return;

            var limitValue = context.ValueProvider.GetValue("limit");
            var limit = 20;
            if (limitValue != null) {
                limit = (int) limitValue.ConvertTo(typeof (int));
            }

            var site = _services.WorkContext.CurrentSite;

            if (context.Format == "rss") {
                var link = new XElement("link");
                context.Response.Element.SetElementValue("title", GetTitle(tagName, site));
                context.Response.Element.Add(link);
                context.Response.Element.SetElementValue("description", GetDescription(tagName, site));

                context.Response.Contextualize(requestContext => link.Add(GetTagUrl(tagName, requestContext)));
            }
            else {
                context.Builder.AddProperty(context, null, "title", GetTitle(tagName, site));
                context.Builder.AddProperty(context, null, "description", GetDescription(tagName, site));
                context.Response.Contextualize(requestContext => context.Builder.AddProperty(context, null, "link", GetTagUrl(tagName, requestContext)));
            }

            var items = _tagService.GetTaggedContentItems(tag.Id, 0, limit);

            foreach (var item in items) {
                context.Builder.AddItem(context, item.ContentItem);
            }
        }

        public static string GetTitle(string tagName, ISite site) {
            return site.SiteName + " - " + tagName;
        }

        public static string GetDescription(string tagName, ISite site) {
            return site.SiteName + " - " + tagName;
        }

        private static string GetTagUrl(string tag, RequestContext requestContext) {
            var urlHelper = new UrlHelper(requestContext);
            var uriBuilder = new UriBuilder(urlHelper.RequestContext.HttpContext.Request.ToRootUrlString()) {
                Path = urlHelper.RouteUrl(new {
                    area = "Orchard.Tags",
                    controller = "Home",
                    action = "Search",
                    tagName = tag
                })
            };
            return uriBuilder.Uri.OriginalString;

        }
    }
}
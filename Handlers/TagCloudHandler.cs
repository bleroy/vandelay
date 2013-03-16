using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Tags.Models;
using Vandelay.Industries.Models;
using Vandelay.Industries.Services;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudHandler : ContentHandler {
        private readonly ITagCloudService _tagCloudService;
        private readonly ISignals _signals;

        public TagCloudHandler(
            IRepository<TagCloudRecord> repository,
            ITagCloudService tagCloudService,
            ISignals signals) {

            Filters.Add(StorageFilter.For(repository));
            _tagCloudService = tagCloudService;
            _signals = signals;
        }

        protected override void Loading(LoadContentContext context) {
            if (context.ContentType == "TagCloud") {
                SetupTagCloudLoader(context.ContentItem);
            }
            base.Loading(context);
        }

        protected override void Versioning(VersionContentContext context) {
            if (context.ContentType == "TagCloud") {
                SetupTagCloudLoader(context.BuildingContentItem);
            }
            base.Versioning(context);
        }

        protected override void Published(PublishContentContext context) {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null) {
                _signals.Trigger(TagCloudService.VandelayTagcloudTagschanged);
            }
            base.Published(context);
        }

        protected override void Unpublished(PublishContentContext context) {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null) {
                _signals.Trigger(TagCloudService.VandelayTagcloudTagschanged);
            }
            base.Unpublished(context);
        }

        protected override void Removed(RemoveContentContext context) {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null) {
                _signals.Trigger(TagCloudService.VandelayTagcloudTagschanged);
            }
            base.Removed(context);
        }

        private void SetupTagCloudLoader(ContentItem item) {
            var cloudPart = (TagCloudPart) item.Get(typeof (TagCloudPart));
            cloudPart._tagCounts.Loader(tags =>
            _tagCloudService.GetPopularTags(cloudPart.Buckets, cloudPart.Slug).ToList());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;
using Vandelay.Industries.Services;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerPartDriver : ContentPartDriver<FeedburnerPart> {
        private readonly IEnumerable<IFeedDataProvider> _feedDataProviders;
        private readonly ISignals _signals;

        public FeedburnerPartDriver(IEnumerable<IFeedDataProvider> feedDataProviders, ISignals signals) {
            _feedDataProviders = feedDataProviders;
            _signals = signals;
        }

        protected override string Prefix {
            get {
                return "feedburner";
            }
        }

        protected override DriverResult Display(FeedburnerPart part, string displayType, dynamic shapeHelper) {
            if (!string.IsNullOrWhiteSpace(part.FeedburnerUrl)) {
                return ContentShape("Parts_Feedburner", () => shapeHelper.Parts_Feedburner(
                    FeedburnerUrl: part.FeedburnerUrl));
            }
            return new DriverResult();
        }

        //GET
        protected override DriverResult Editor(
            FeedburnerPart part, dynamic shapeHelper) {

                return ContentShape("Parts_Feedburner_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Feedburner",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            FeedburnerPart part, IUpdateModel updater, dynamic shapeHelper) {

            updater.TryUpdateModel(part, Prefix, null, null);
            foreach (var key in _feedDataProviders
                .Select(feedDataProvider => feedDataProvider.GetKey(part.ContentItem, "rss"))
                .Where(key => key != null)) {

                _signals.Trigger(key);
            }
            return Editor(part, shapeHelper);
        }
    }
}
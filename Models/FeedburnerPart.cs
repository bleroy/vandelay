using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerPartRecord : ContentPartRecord {
        public virtual string FeedburnerUrl { get; set; }
    }

    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerPart : ContentPart<FeedburnerPartRecord> {
        public string FeedburnerUrl {
            get { return Record.FeedburnerUrl; }
            set { Record.FeedburnerUrl = value; }
        }
    }
}
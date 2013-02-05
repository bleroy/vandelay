using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.RemoteRss")]
    public class RemoteRssPartRecord : ContentPartRecord {
        public RemoteRssPartRecord() {
            ItemsToDisplay = 5;
            CacheDuration = 20;
        }

        public virtual string RemoteRssUrl { get; set; }
        public virtual int ItemsToDisplay { get; set; }
        public virtual int CacheDuration { get; set; }
    }

    [OrchardFeature("Vandelay.RemoteRss")]
    public class RemoteRssPart : ContentPart<RemoteRssPartRecord> {
        public string RemoteRssUrl {
            get { return Record.RemoteRssUrl; }
            set { Record.RemoteRssUrl = value; }
        }

        public int ItemsToDisplay {
            get { return Record.ItemsToDisplay; }
            set { Record.ItemsToDisplay = value; }
        }

        public int CacheDuration {
            get { return Record.CacheDuration; }
            set { Record.CacheDuration = value; }
        }
    }
}
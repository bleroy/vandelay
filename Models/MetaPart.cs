using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Industries")]
    public class MetaPart : ContentPart<MetaRecord> {
        public string Keywords {
            get { return Retrieve(r => r.Keywords); }
            set { Store(r => r.Keywords, value); }
        }

        public string Description {
            get { return Retrieve(r => r.Description); }
            set { Store(r => r.Description, value); }
        }
    }

    [OrchardFeature("Vandelay.Industries")]
    public class MetaRecord : ContentPartRecord {
        public virtual string Keywords { get; set; }
        public virtual string Description { get; set; }
    }
}
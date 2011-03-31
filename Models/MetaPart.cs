using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Vandelay.Meta.Models {
    public class MetaPart : ContentPart<MetaRecord> {
        public string Keywords {
            get { return Record.Keywords; }
            set { Record.Keywords = value; }
        }

        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }
    }

    public class MetaRecord: ContentPartRecord {
        public virtual string Keywords { get; set; }
        public virtual string Description { get; set; }
    }
}
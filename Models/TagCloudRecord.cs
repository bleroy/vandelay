using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudRecord : ContentPartRecord {
        public TagCloudRecord() {
            Buckets = 5;
        }
        public virtual int Buckets { get; set; }
        public virtual string Slug { get; set; }
    }
}
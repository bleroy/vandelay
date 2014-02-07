using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudPart : ContentPart<TagCloudRecord> {
        internal readonly LazyField<IList<TagCount>> TagCountField = new LazyField<IList<TagCount>>();

        public IList<TagCount> TagCounts { get { return TagCountField.Value; } }

        public int Buckets { 
            get { return Retrieve(r => r.Buckets); }
            set { Store(r => r.Buckets, value); }
        }

        public string Slug {
            get { return Retrieve(r => r.Slug); }
            set { Store(r => r.Slug, value); }
        }
    }
}
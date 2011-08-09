using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudPart : ContentPart<TagCloudRecord> {
        internal readonly LazyField<IList<TagCount>> _tagCounts = new LazyField<IList<TagCount>>();

        public IList<TagCount> TagCounts { get { return _tagCounts.Value; } }

        public int Buckets { 
            get { return Record.Buckets; }
            set { Record.Buckets = value; }
        }

        public string Slug {
            get { return Record.Slug; }
            set { Record.Slug = value; }
        }
    }
}
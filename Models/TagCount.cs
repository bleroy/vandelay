using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCount {
        public string TagName { get; set; }
        public int Count { get; set; }
        public int Bucket { get; set; }
    }
}
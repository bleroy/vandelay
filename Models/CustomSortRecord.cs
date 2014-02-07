using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortRecord {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
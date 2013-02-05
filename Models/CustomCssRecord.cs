using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCssRecord : ContentPartRecord {
        public virtual string CustomId { get; set; }
        public virtual string CssClass { get; set; }
        public virtual string Scripts { get; set; }
    }
}
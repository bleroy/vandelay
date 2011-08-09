using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconSettingsPartRecord : ContentPartRecord {
        public virtual string FaviconUrl { get; set; }
    }
}
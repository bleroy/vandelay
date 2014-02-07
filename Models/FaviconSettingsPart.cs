using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconSettingsPart : ContentPart<FaviconSettingsPartRecord> {
        public string FaviconUrl {
            get { return Retrieve(r => r.FaviconUrl); }
            set { Store(r => r.FaviconUrl, value); }
        }
    }
}

using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconSettingsPart : ContentPart<FaviconSettingsPartRecord> {
        public string FaviconUrl {
            get { return Record.FaviconUrl; }
            set { Record.FaviconUrl = value; }
        }
    }
}

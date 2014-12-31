using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCss : ContentPart<CustomCssRecord> {
        public string CustomId {
            get { return Retrieve(r =>r.CustomId); }
            set { Store(r => r.CustomId, value); }
        }

        public string CssClass {
            get { return Retrieve(r => r.CssClass); }
            set { Store(r => r.CssClass, value); }
        }

        public string Scripts {
            get { return Retrieve(r => r.Scripts); }
            set { Store(r => r.Scripts, value); }
        }
    }
}

using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCss : ContentPart<CustomCssRecord> {
        public string CustomId {
            get { return Record.CustomId; }
            set { Record.CustomId = value; }
        }

        public string CssClass {
            get { return Record.CssClass; }
            set { Record.CssClass = value; }
        }

        public string Scripts {
            get { return Record.Scripts; }
            set { Record.Scripts = value; }
        }
    }
}

using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCssDriver : ContentPartDriver<CustomCss> {
        private readonly INotifier _notifier;
        private const string TemplateName = "Parts/CustomCss";

        protected override string Prefix {
            get { return "vandelay_classy"; }
        }

        public Localizer T { get; set; }

        public CustomCssDriver(INotifier notifier) {
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(CustomCss part, string displayType, dynamic shapeHelper) {
            return new DriverResult();
        }

        protected override DriverResult Editor(CustomCss part, dynamic shapeHelper) {
            return ContentShape("Parts_CustomCss_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CustomCss part, IUpdateModel updater, dynamic shapeHelper) {
            if (!updater.TryUpdateModel(part, Prefix, null, null)) {
                _notifier.Error(T("Error during CustomCss update."));
            }
            return Editor(part, shapeHelper);
        }

    }
}
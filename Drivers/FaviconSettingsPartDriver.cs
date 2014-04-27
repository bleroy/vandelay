using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;
using Vandelay.Industries.Services;
using Vandelay.Industries.ViewModels;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconSettingsPartDriver : ContentPartDriver<FaviconSettingsPart> {
        private readonly ISignals _signals;
        private readonly IFaviconService _faviconService;

        public FaviconSettingsPartDriver(ISignals signals, IFaviconService faviconService) {
            _signals = signals;
            _faviconService = faviconService;
        }

        protected override string Prefix { get { return "FaviconSettings"; } }

        protected override DriverResult Editor(FaviconSettingsPart part, dynamic shapeHelper) {
            var faviconSuggestions = _faviconService.GetFaviconSuggestions();

            return ContentShape("Parts_Favicon_FaviconSettings",
                               () => shapeHelper.EditorTemplate(
                                   TemplateName: "Parts/Favicon.FaviconSettings",
                                   Model: new FaviconSettingsViewModel {
                                       FaviconUrl = part.FaviconUrl,
                                       FaviconUrlSuggestions = faviconSuggestions
                                   },
                                   Prefix: Prefix)).OnGroup("Favicon");
        }

        protected override DriverResult Editor(FaviconSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            _signals.Trigger("Vandelay.Favicon.Changed");
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(FaviconSettingsPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FaviconUrl", part.Record.FaviconUrl);
        }

        protected override void Importing(FaviconSettingsPart part, ImportContentContext context) {
            part.Record.FaviconUrl = context.Attribute(part.PartDefinition.Name, "FaviconUrl");
        }
    }
}
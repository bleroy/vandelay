using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenSettingsPartDriver : ContentPartDriver<SplashScreenSettingsPart> {
        public SplashScreenSettingsPartDriver() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix { get { return "SplashScreenSettings"; } }

        protected override DriverResult Editor(SplashScreenSettingsPart part, dynamic shapeHelper) {
            if (String.IsNullOrWhiteSpace(part.AcceptButtonText)) {
                part.AcceptButtonText = T("Accept").Text;
            }
            return ContentShape("Parts_SplashScreenSettings",
                               () => shapeHelper.EditorTemplate(
                                   TemplateName: "Parts/SplashScreenSettings",
                                   Model: part,
                                   Prefix: Prefix)).OnGroup("Splash Screen");
        }

        protected override DriverResult Editor(SplashScreenSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(SplashScreenSettingsPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).With(part)
                .ToAttr(p => p.Enabled)
                .ToAttr(p => p.AcceptButtonText)
                .ToAttr(p => p.RejectButtonText)
                .ToAttr(p => p.RedirectUrl)
                .ToAttr(p => p.SplashScreenContents);
        }

        protected override void Importing(SplashScreenSettingsPart part, ImportContentContext context) {
            context.Data.Element(part.PartDefinition.Name).With(part)
                .FromAttr(p => p.Enabled)
                .FromAttr(p => p.AcceptButtonText)
                .FromAttr(p => p.RejectButtonText)
                .FromAttr(p => p.RedirectUrl)
                .FromAttr(p => p.SplashScreenContents);
        }
    }
}
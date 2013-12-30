using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenSettingsPartHandler : ContentHandler {
        public SplashScreenSettingsPartHandler() {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<SplashScreenSettingsPart>("Site"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Splash Screen")));
        }
    }
}
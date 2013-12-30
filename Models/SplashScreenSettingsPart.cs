using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.SplashScreen")]
    public class SplashScreenSettingsPart : ContentPart {
        public const string CookieName = "SplashScreen";

        public bool Enabled {
            get { return this.Retrieve(p => p.Enabled); }
            set { this.Store(p => p.Enabled, value); }
        }

        public string SplashScreenContents {
            get { return this.Retrieve(p => p.SplashScreenContents); }
            set { this.Store(p => p.SplashScreenContents, value);}
        }

        public string AcceptButtonText {
            get { return this.Retrieve(p => p.AcceptButtonText); }
            set { this.Store(p => p.AcceptButtonText, value); }
        }

        public string RejectButtonText {
            get { return this.Retrieve(p => p.RejectButtonText); }
            set { this.Store(p => p.RejectButtonText, value); }
        }

        public string RedirectUrl {
            get { return this.Retrieve(p => p.RedirectUrl); }
            set { this.Store(p => p.RedirectUrl, value);}
        }
    }
}
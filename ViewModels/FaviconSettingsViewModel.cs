using System.Collections.Generic;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.ViewModels {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconSettingsViewModel {
        public string FaviconUrl { get; set; }
        public IEnumerable<string> FaviconUrlSuggestions { get; set; }
    }
}
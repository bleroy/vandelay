using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.ViewModels {
    [OrchardFeature("Vandelay.ThemePicker")]
    public class ThemePickerIndexViewModel {
        public IEnumerable<ThemePickerSettingsRecord> ThemeSelectionSettings { get; set; }
        public IEnumerable<string> ThemeSelectionRules { get; set; }
        public IEnumerable<string> Themes { get; set; }
    }
}
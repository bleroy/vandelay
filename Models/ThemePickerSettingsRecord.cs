using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.ThemePicker")]
    public class ThemePickerSettingsRecord {
        public virtual int Id { get; set;}
        public virtual string RuleType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Criterion { get; set; }
        public virtual string Theme { get; set; }
        public virtual int Priority { get; set; }
        public virtual string Zone { get; set; }
        public virtual string Position { get; set; }
    }
}
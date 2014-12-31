using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Settings {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCssSettings {
        public string CustomId { get; set; }
        public string CssClass { get; set; }
        public string Scripts { get; set; }
    }

    [OrchardFeature("Vandelay.Classy")]
    public class CustomCssSettingsHooks : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "CustomCss")
                yield break;

            var model = definition.Settings.GetModel<CustomCssSettings>();
            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "CustomCss")
                yield break;

            var model = new CustomCssSettings();
            updateModel.TryUpdateModel(model, "CustomCssSettings", null, null);
            builder
                .WithSetting("CustomCssSettings.CustomId", !string.IsNullOrWhiteSpace(model.CustomId) ? model.CustomId : null)
                .WithSetting("CustomCssSettings.CssClass", !string.IsNullOrWhiteSpace(model.CssClass) ? model.CssClass : null)
                .WithSetting("CustomCssSettings.Scripts", !string.IsNullOrWhiteSpace(model.Scripts) ? model.Scripts : null);
            yield return DefinitionTemplate(model);
        }
    }
}

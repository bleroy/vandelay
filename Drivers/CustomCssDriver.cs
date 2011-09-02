using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
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

        protected override void Exporting(CustomCss part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("CssClass", part.CssClass);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CustomId", part.CustomId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Scripts", part.Scripts);
        }

        protected override void Importing(CustomCss part, ImportContentContext context) {
            string partName = part.PartDefinition.Name;

            part.CssClass = GetAttribute<string>(context, partName, "CssClass");
            part.CustomId = GetAttribute<string>(context, partName, "CustomId");
            part.Scripts = GetAttribute<string>(context, partName, "Scripts");
        }

        //Using TV for generic parameter here simply to avoid confusion with T Localizer property
        private TV GetAttribute<TV>(ImportContentContext context, string partName, string elementName) {
            string value = context.Attribute(partName, elementName);
            if (value != null)
            {
                return (TV)Convert.ChangeType(value, typeof(TV));
            }
            return default(TV);
        }
    }
}
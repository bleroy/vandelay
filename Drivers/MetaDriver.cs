﻿using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.UI.Resources;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.Industries")]
    public class MetaDriver : ContentPartDriver<MetaPart> {
        private readonly IWorkContextAccessor _wca;

        public MetaDriver(IWorkContextAccessor workContextAccessor) {
            _wca = workContextAccessor;
        }

        protected override string Prefix {
            get {
                return "meta";
            }
        }

        protected override DriverResult Display(MetaPart part, string displayType, dynamic shapeHelper) {
            if (displayType != "Detail") return null;
            var resourceManager = _wca.GetContext().Resolve<IResourceManager>();
            if (!String.IsNullOrWhiteSpace(part.Description)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "description",
                    Content = part.Description
                });
            }
            if (!String.IsNullOrWhiteSpace(part.Keywords)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "keywords",
                    Content = part.Keywords
                });
            }
            return null;
        }

        //GET
        protected override DriverResult Editor(MetaPart part, dynamic shapeHelper) {

            return ContentShape("Parts_Meta_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Meta",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(MetaPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(MetaPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Keywords", part.Record.Keywords);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Record.Description);
        }

        protected override void Importing(MetaPart part, ImportContentContext context) {
            part.Record.Keywords = context.Attribute(part.PartDefinition.Name, "Keywords");
            part.Record.Description = context.Attribute(part.PartDefinition.Name, "Description");
        }
    }
}
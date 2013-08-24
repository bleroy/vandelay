using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudDriver : ContentPartDriver<TagCloudPart> {

        protected override string Prefix {
            get {
                return "tagcloud";
            }
        }
        protected override DriverResult Display(TagCloudPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_TagCloud",
                () => shapeHelper.Parts_TagCloud(
                    TagCounts: part.TagCounts,
                    ContentPart: part,
                    ContentItem: part.ContentItem));
        }

        //GET
        protected override DriverResult Editor(TagCloudPart part, dynamic shapeHelper) {

            return ContentShape("Parts_TagCloud_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TagCloud",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(TagCloudPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(TagCloudPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Slug", part.Record.Slug);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Buckets", part.Record.Buckets);
        }

        protected override void Importing(TagCloudPart part, ImportContentContext context) {
            part.Record.Slug = context.Attribute(part.PartDefinition.Name, "Slug");
            part.Record.Buckets = Convert.ToInt32(context.Attribute(part.PartDefinition.Name, "Buckets"));
        }
    }
}
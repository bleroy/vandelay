using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudDriver : ContentPartDriver<TagCloudPart> {
        protected override DriverResult Display(TagCloudPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_TagCloud",
                () => shapeHelper.Parts_TagCloud(TagCounts: part.TagCounts));
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
    }
}
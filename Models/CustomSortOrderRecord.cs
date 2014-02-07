using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortOrderRecord : ContentPartRecord {
        public virtual CustomSortRecord CustomSortRecord { get; set; }
        public virtual int SortOrder { get; set; }
    }
}
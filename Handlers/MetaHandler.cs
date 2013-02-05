using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.Industries")]
    public class MetaHandler : ContentHandler {
        public MetaHandler(IRepository<MetaRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
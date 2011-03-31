using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Vandelay.Meta.Models;

namespace Vandelay.Meta.Handlers {
    public class MetaHandler : ContentHandler {
        public MetaHandler(IRepository<MetaRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
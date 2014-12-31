using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.RemoteRss")]
    public class RemoteRssPartHandler : ContentHandler {
        public RemoteRssPartHandler(IRepository<RemoteRssPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
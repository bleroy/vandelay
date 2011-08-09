using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerPartHandler : ContentHandler {
        public FeedburnerPartHandler(IRepository<FeedburnerPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
using System;
using System.Xml.Linq;
using Orchard;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using Orchard.Services;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface IRemoteRssService : IDependency {
        XElement GetFeed(RemoteRssPart remoteRss);
    }

    [OrchardFeature("Vandelay.RemoteRss")]
    public class RemoteRssService : IRemoteRssService {
        private readonly IClock _clock;
        private readonly ICacheManager _cacheManager;
        private const string RemoteRssCacheKey = "Vandelay.RemoteRss.";

        public RemoteRssService(IClock clock, ICacheManager cacheManager) {
            _clock = clock;
            _cacheManager = cacheManager;
        }

        public XElement GetFeed(RemoteRssPart remoteRss) {
            var feed = _cacheManager.Get(RemoteRssCacheKey + remoteRss.RemoteRssUrl,
                                         s => {
                                             s.Monitor(_clock.When(TimeSpan.FromMinutes(remoteRss.CacheDuration)));
                                             var f = XElement.Load(remoteRss.RemoteRssUrl);
                                             return f;
                                         }
                );
            return feed;
        }
    }
}
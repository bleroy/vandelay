using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Autoroute.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Tags.Models;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface ITagCloudService : IDependency {
        IEnumerable<TagCount> GetPopularTags(int buckets, string slug);
    }

    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudService : ITagCloudService {
        private readonly IRepository<ContentTagRecord> _contentTagRepository;
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        internal static readonly string VandelayTagcloudTagschanged = "Vandelay.TagCloud.TagsChanged";

        public TagCloudService(
            IRepository<ContentTagRecord> contentTagRepository,
            IContentManager contentManager,
            ICacheManager cacheManager,
            ISignals signals) {

            _contentTagRepository = contentTagRepository;
            _contentManager = contentManager;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public IEnumerable<TagCount> GetPopularTags(int buckets, string slug) {
            var cacheKey = "Vandelay.TagCloud." + (slug ?? "") + '.' + buckets;
            return _cacheManager.Get(cacheKey,
                              ctx => {
                                  ctx.Monitor(_signals.When(VandelayTagcloudTagschanged));
                                  IEnumerable<TagCount> tagCounts;
                                  if (string.IsNullOrWhiteSpace(slug)) {
                                      tagCounts = (from tc in _contentTagRepository.Table
                                                   where tc.TagsPartRecord.ContentItemRecord.Versions.Where(v => v.Published).Any()
                                                   group tc by tc.TagRecord.TagName
                                                       into g
                                                       select new TagCount {
                                                           TagName = g.Key,
                                                           Count = g.Count()
                                                       }).ToList();
                                  }
                                  else {
                                      var container = _contentManager
                                          .Query<AutoroutePart, AutoroutePartRecord>()
                                          .ForVersion(VersionOptions.Published)
                                          .Where(c => c.DisplayAlias == slug)
                                          .List()
                                          .FirstOrDefault();
                                      if (container == default(AutoroutePart)) return new List<TagCount>();
                                      tagCounts = _contentManager
                                          .Query<TagsPart, TagsPartRecord>(VersionOptions.Published)
                                          .Join<CommonPartRecord>()
                                          .Where(t => t.Container.Id == container.Id)
                                          .List()
                                          .SelectMany(t => t.CurrentTags)
                                          .GroupBy(t => t.TagName)
                                          .Select(g => new TagCount {
                                              TagName = g.Key,
                                              Count = g.Count()
                                          })
                                          .ToList();
                                  }
                                  var maxCount = tagCounts.Max(tc => tc.Count);
                                  var minCount = tagCounts.Min(tc => tc.Count);
                                  var delta = maxCount - minCount;
                                  if (delta != 0) {
                                      // Linear fitting algorithm to associate tags to buckets
                                      // There are smarter ones, left as an exercise to the reader.
                                      // (see for example http://www.codeproject.com/KB/recipes/K-Mean_Clustering.aspx)
                                      foreach (var tagCount in tagCounts) {
                                          tagCount.Bucket = (tagCount.Count - minCount) * (buckets - 1) / delta + 1;
                                      }
                                  }
                                  return tagCounts;
                              });
        }
    }
}
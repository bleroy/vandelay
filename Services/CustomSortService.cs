using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface ICustomSortService : IDependency {
        IQueryable<CustomSortRecord> Query();
        CustomSortRecord Create(string name);
        CustomSortRecord Get(int id);
        void Delete(int id);
        IEnumerable<int> GetOrderedIds(int sortId);
        void SaveOrderedIds(int sortId, IEnumerable<int> orderedIds);
    }

    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortService : ICustomSortService {
        private readonly IRepository<CustomSortRecord> _sortRepository;
        private readonly IRepository<CustomSortOrderRecord> _sortOrderRepository; 

        public CustomSortService(
            IRepository<CustomSortRecord> sortRepository,
            IRepository<CustomSortOrderRecord> sortOrderRepository) {

            _sortRepository = sortRepository;
            _sortOrderRepository = sortOrderRepository;
        }

        public IQueryable<CustomSortRecord> Query() {
            return _sortRepository.Table;
        }

        public CustomSortRecord Create(string name) {
            var newRecord = new CustomSortRecord {
                Name = name
            };
            _sortRepository.Create(newRecord);
            return newRecord;
        }

        public CustomSortRecord Get(int id) {
            return _sortRepository.Get(id);
        }

        public void Delete(int id) {
            var sort = _sortRepository.Get(id);
            if (sort != null) {
                _sortRepository.Delete(sort);
            }
        }

        public IEnumerable<int> GetOrderedIds(int sortId) {
            return _sortOrderRepository
                .Table
                .Where(o => o.CustomSortRecord.Id == sortId)
                .OrderByDescending(o => o.SortOrder)
                .Select(o => o.ContentItemRecord.Id)
                .ToList();
        }

        public void SaveOrderedIds(int sortId, IEnumerable<int> orderedIds) {
            var sort = _sortRepository.Get(sortId);
            if (sort == null) return;
            var currentIds = _sortOrderRepository
                .Fetch(o => o.CustomSortRecord.Id == sortId)
                .ToList();
            var currentIdLookup = new HashSet<int>(currentIds.Select(o => o.Id));
            var currentRecords = currentIds
                .ToDictionary(o => o.Id, o => o);
            var orderedIdList = orderedIds.ToList();
            // Sort order is stored as negative numbers to easily get nulls last when querying just by using descending order.
            var orderedIdToIndex = orderedIdList
                .Select((id, index) => new {id, index})
                .ToDictionary(o => o.id, o => -o.index);
            var newIdLookup = new HashSet<int>(orderedIdList);
            // Delete records no longer here, update others
            foreach (var id in currentIdLookup) {
                if (newIdLookup.Contains(id)) {
                    var record = currentRecords[id];
                    record.SortOrder = orderedIdToIndex[id];
                    _sortOrderRepository.Update(record);
                }
                else {
                    _sortOrderRepository.Delete(currentRecords[id]);
                }
            }
            // Create the new records
            foreach (var id in orderedIdList.Where(id => !currentIdLookup.Contains(id))) {
                _sortOrderRepository.Create(new CustomSortOrderRecord {
                    CustomSortRecord = sort,
                    ContentItemRecord = new ContentItemRecord {Id = id},
                    SortOrder = orderedIdToIndex[id]
                });
            }
        }
    }
}

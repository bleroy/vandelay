using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Services;
using Vandelay.Industries.Models;
using Vandelay.Industries.Services;

namespace Vandelay.Industries.Providers.SortCriteria {
    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortCriteria : ISortCriterionProvider {
        private readonly ICustomSortService _customSortService;
        private readonly UrlHelper _url;

        public CustomSortCriteria(ICustomSortService customSortService, UrlHelper urlHelper) {
            _customSortService = customSortService;
            _url = urlHelper;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeSortCriterionContext describe) {
            foreach (var customSort in _customSortService.Query()) {
                var descriptor = describe.For("CustomSorts",
                    T("Custom Sorts"), T("Manually-defined sort orders."));
                var sortId = customSort.Id;
                var sortName = customSort.Name;
                var sortEditUrl = _url.Action("Edit", "CustomSortAdmin", new {id = sortId, area="Vandelay.Industries"});
                descriptor.Element(
                    type: customSort.Name,
                    name: new LocalizedString(customSort.Name),
                    description: T("Custom sort order <a href=\"{1}\">{0}</a>",
                        customSort.Name,
                        sortEditUrl),
                    sort: context => ApplySortCriterion(context, sortId, sortName),
                    display: context => DisplaySortCriterion(context, sortName));
            }
        }

        public void ApplySortCriterion(SortCriterionContext context, int sortId, string sortName) {
            Action<IHqlExpressionFactory> predicate = y => y.Eq("CustomSortRecord.Id", sortId);

            Action<IAliasFactory> relationship = x =>
                x.ContentPartRecord<CustomSortOrderRecord>("left outer join", predicate);

            context.Query = context.Query.Join(relationship);

            context.Query = context.Query.OrderBy(relationship, x => x.Desc("SortOrder"));
        }

        public LocalizedString DisplaySortCriterion(SortCriterionContext context, string sortName) {
            return T("Ordered by {0}", sortName);

        }
    }
}
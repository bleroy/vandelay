using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Vandelay.Industries.Services;

namespace Vandelay.Industries.Controllers {
    [OrchardFeature("Vandelay.CustomSort")]
    [Admin]
    public class CustomSortAdminController : Controller {
        private dynamic Shape { get; set; }
        private readonly ISiteService _siteService;
        private readonly ICustomSortService _sortService;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;

        public CustomSortAdminController(
            ISiteService siteService,
            ICustomSortService sortService,
            IProjectionManager projectionManager,
            IContentManager contentManager,
            IOrchardServices services,
            IShapeFactory shapeFactory) {

            _siteService = siteService;
            _sortService = sortService;
            _projectionManager = projectionManager;
            _contentManager = contentManager;
            _services = services;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var sorts = _sortService.Query();
            var paginatedSorts = sorts.OrderBy(s => s.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(sorts.Count());
            var vm = Shape.CustomSortAdmin_Index(
                CustomSorts: paginatedSorts,
                Pager: pagerShape
            );
            return new ShapeResult(this, vm);
        }

        public ActionResult Create() {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            return new ShapeResult(this, Shape.CustomSortAdmin_Create());
        }

        [HttpPost]
        public ActionResult Create(string name) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            var newSort = _sortService.Create(name);
            return RedirectToAction("Edit", new {id = newSort.Id});
        }

        public ActionResult Edit(int id) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            var sort = _sortService.Get(id);
            var queries = _contentManager
                .Query<TitlePart>(VersionOptions.Latest)
                .Join<QueryPartRecord>()
                .OrderBy<TitlePartRecord>(r => r.Title)
                .List();

            var sortedIds = _sortService.GetOrderedIds(id).ToArray();
            var contentItems = _contentManager.GetMany<IContent>(
                sortedIds, VersionOptions.Latest, new QueryHints());
            var contentShapes = contentItems.Select(item =>
                _contentManager.BuildDisplay(item, "SummaryAdmin"));
            var list = Shape.List(Id: "pinned-items");
            list.AddRange(contentShapes);

            var vm = Shape.CustomSortAdmin_Edit(
                SortOrder: sort,
                Queries: queries,
                PinnedItems: list,
                PinnedItemIds: sortedIds);

            return new ShapeResult(this, vm);
        }

        [HttpPost]
        public ActionResult Edit(int id, string name, string pinnedItems) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            var sort = _sortService.Get(id);
            sort.Name = name;

            _sortService.SaveOrderedIds(id,
                pinnedItems
                .Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToInt32(s)));

            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            _sortService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Themed(false)]
        public ActionResult QueryResults(int queryId, int skip = 0, int count = 12) {
            if (!_services.Authorizer.Authorize(Orchard.Projections.Permissions.ManageQueries, T("Not allowed to manage queries")))
                return new HttpUnauthorizedResult();

            var query = _contentManager.Get(queryId).As<QueryPart>();
            if (query == null) {
                return HttpNotFound(T("Query not found.").Text);
            }
            var contentItems = _projectionManager.GetContentItems(queryId, skip, count);
            var totalCount = _projectionManager.GetCount(queryId);
            var contentShapes = contentItems.Select(item =>
                _contentManager.BuildDisplay(item, "SummaryAdmin"));

            var list = Shape.List();
            list.AddRange(contentShapes);
            list.Attributes.Add("data-total-count", totalCount.ToString(CultureInfo.InvariantCulture));

            return new ShapePartialResult(this, list);
        }
    }
}
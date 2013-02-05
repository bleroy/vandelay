using System;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Contents.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Permissions = Orchard.Themes.Permissions;

namespace Vandelay.Industries.Menus {
    [OrchardFeature("Vandelay.ContentAdminMenu")]
    public class ContentAdminMenu : INavigationProvider {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public ContentAdminMenu(IContentDefinitionManager contentDefinitionManager, IContentManager contentManager) {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }

        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            var contentTypeDefinitions = _contentDefinitionManager
                .ListTypeDefinitions().OrderBy(d => d.Name);
            var contentTypes = contentTypeDefinitions
                .Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable)
                .OrderBy(ctd => ctd.DisplayName);
            if (contentTypes.Any()) {
                builder
                    .AddImageSet("content")
                    .Add(T("Content"), "1.4", menu => {
                        menu.LinkToFirstChild(false);
                        foreach (var contentTypeDefinition in contentTypes) {
                            if (string.Compare(
                                contentTypeDefinition.Settings["ContentTypeSettings.Creatable"],
                                "true", StringComparison.OrdinalIgnoreCase) == 0) {

                                menu.Add(T(contentTypeDefinition.DisplayName), "5", item =>
                                    item.Action("List", "Admin",
                                    new RouteValueDictionary {
                                        {"area", "Contents"},
                                        {"model.Id", contentTypeDefinition.Name}
                                    })
                                    .Permission(DynamicPermissions.CreateDynamicPermission(
                                        DynamicPermissions.PermissionTemplates["PublishOwnContent"],
                                        contentTypeDefinition)));
                            }
                        }
                    });
            }
        }
    }
}
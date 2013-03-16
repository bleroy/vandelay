using System;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents;
using Orchard.Core.Contents.Settings;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Vandelay.Industries.Menus {
    [OrchardFeature("Vandelay.ContentAdminMenu")]
    public class ContentAdminMenu : INavigationProvider {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ContentAdminMenu(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
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
                                ContentTypeDefinition definition = contentTypeDefinition;
                                menu.Add(T(contentTypeDefinition.DisplayName), "5", item =>
                                    item.Action("List", "Admin",
                                    new RouteValueDictionary {
                                        {"area", "Contents"},
                                        {"model.Id", definition.Name}
                                    })
                                    .Permission(DynamicPermissions.CreateDynamicPermission(
                                        DynamicPermissions.PermissionTemplates["PublishOwnContent"],
                                        definition)));
                            }
                        }
                    });
            }
        }
    }
}
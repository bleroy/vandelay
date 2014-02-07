using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Vandelay.Industries.Menus {
    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortAdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("projector").Add(T("Queries"), "3",
                menu => menu
                    .Add(T("Custom sort orders"), "3.0",
                        qi => qi.Action("Index", "CustomSortAdmin", new { area = "Vandelay.Industries" })
                            .Permission(Orchard.Projections.Permissions.ManageQueries).LocalNav())
            );
        }
    }
}

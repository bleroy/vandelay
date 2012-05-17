using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Vandelay.Industries.Permissions;

namespace Vandelay.Industries.Menus {
    [OrchardFeature("Vandelay.UserStorage")]
    public class UserStorageAdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(
                    T("Users"), "20",
                menu => menu.Add(T("Storage"), "5.0", item => item.Action("Index", "UserStorage", new {area = "Vandelay.Industries"})
                    .LocalNav().Permission(UserStoragePermissions.ManageUserStorage)));
        }
    }
}
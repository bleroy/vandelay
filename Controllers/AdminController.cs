using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Orchard;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Localization;
using Orchard.UI.Admin;
using Vandelay.Industries.Services;
using Vandelay.Industries.ViewModels;
using Permissions = Orchard.Themes.Permissions;

namespace Vandelay.Industries.Controllers {
    [Admin]
    [OrchardFeature("Vandelay.ThemePicker")]
    [ValidateInput(false)]
    public class AdminController : Controller {
        private readonly ISettingsService _settingsService;
        private readonly IExtensionManager _extensionManager;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IEnumerable<IThemeSelectionRule> _rules;

        public AdminController(
            IOrchardServices orchardServices,
            ISettingsService settingsService,
            IExtensionManager extensionManager,
            ShellDescriptor shellDescriptor,
            IEnumerable<IThemeSelectionRule> rules) {

            Services = orchardServices;
            _settingsService = settingsService;
            _extensionManager = extensionManager;
            _shellDescriptor = shellDescriptor;
            _rules = rules;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index() {
            if (!Services.Authorizer.Authorize(Permissions.ApplyTheme, T("Cannot manage themes")))
                return new HttpUnauthorizedResult();

            var viewModel = new ThemePickerIndexViewModel {
                ThemeSelectionSettings = _settingsService.Get(),
                ThemeSelectionRules = _rules.Select(r => r.GetType().Name),
                Themes = _extensionManager.AvailableExtensions()
                    .Where(d => DefaultExtensionTypes.IsTheme(d.ExtensionType) &&
                                _shellDescriptor.Features.Any(sf => sf.Name == d.Id))
                    .Select(d => d.Id)
                    .OrderBy(n => n)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(
            string name,
            string ruleType,
            string criterion,
            string theme,
            int priority,
            string zone,
            string position) {

            if (!Services.Authorizer.Authorize(Permissions.ApplyTheme, T("Cannot manage themes")))
                return new HttpUnauthorizedResult();

            _settingsService.Add(name, ruleType, criterion, theme, priority, zone, position);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Remove(FormCollection form) {
            if (!Services.Authorizer.Authorize(Permissions.ApplyTheme, T("Cannot manage themes")))
                return new HttpUnauthorizedResult();

            var id = int.Parse(form.GetKey(0).Substring(1));
            _settingsService.Remove(id);

            return RedirectToAction("Index");
        }
    }
}
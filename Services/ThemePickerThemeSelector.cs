using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard;
using Orchard.Caching;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using Orchard.UI.Admin;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    [OrchardFeature("Vandelay.ThemePicker")]
    public class ThemePickerThemeSelector : IThemeSelector {
        private readonly ISettingsService _settingsService;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IShapeFactory _shapeFactory;
        private readonly IEnumerable<IThemeSelectionRule> _rules;

        private ThemeSelectorResult _result;

        public ThemePickerThemeSelector(
            ISettingsService settingsService,
            ISignals signals,
            ICacheManager cacheManager,
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IEnumerable<IThemeSelectionRule> rules) {

            _settingsService = settingsService;
            _signals = signals;
            _cacheManager = cacheManager;
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
            _rules = rules;
        }

        public ThemeSelectorResult GetTheme(RequestContext context) {
            if (AdminFilter.IsApplied(context)) return null;
            if (_result != null) return _result;
            // If the user reverted to the default, short-circuit this
            var workContext = _workContextAccessor.GetContext();
            var session = workContext.HttpContext.Session;
            if (session != null &&
                (bool)(session[workContext.CurrentSite.SiteName + "Vandelay.ThemePicker.UseDefault"] ?? false)) {
                
                return null;
            }
            var settings = _cacheManager.Get(
                "Vandelay.ThemePicker",
                ctx => {
                    ctx.Monitor(_signals.When("Vandelay.ThemePicker.SettingsChanged"));
                    return _settingsService.Get();
                });
            var selectedThemeRule =
                (from settingsRecord in settings
                 let rule = _rules.Where(r =>
                                         r.GetType().Name.Equals(settingsRecord.RuleType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()
                 where rule != null && rule.Matches(settingsRecord.Name, settingsRecord.Criterion)
                 select settingsRecord).FirstOrDefault();
            if (selectedThemeRule == default(ThemePickerSettingsRecord)) return null;
            if (!String.IsNullOrWhiteSpace(selectedThemeRule.Zone)) {
                dynamic linkShape = _shapeFactory.Create("Vandelay_ThemePicker_LinkToDefault");
                var zone = workContext.Layout.Zones[selectedThemeRule.Zone];
                zone.Add(linkShape, selectedThemeRule.Position);
            }
            _result = new ThemeSelectorResult {
                Priority = selectedThemeRule.Priority,
                ThemeName = selectedThemeRule.Theme
            };
            return _result;
        }
    }

}

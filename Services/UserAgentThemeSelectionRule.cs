using System.Text.RegularExpressions;
using Orchard;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Services {
    [OrchardFeature("Vandelay.ThemePicker")]
    public class UserAgentThemeSelectionRule : IThemeSelectionRule {
        private readonly WorkContext _workContext;

        public UserAgentThemeSelectionRule(WorkContext workContext) {
            _workContext = workContext;
        }

        public bool Matches(string name, string criterion) {
            var agent = _workContext.HttpContext.Request.UserAgent;
            if (agent == null) return false;
            return new Regex(criterion, RegexOptions.IgnoreCase)
                .IsMatch(agent);
        }
    }
}
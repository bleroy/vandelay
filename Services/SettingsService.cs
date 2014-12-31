using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface ISettingsService : IDependency {
        IEnumerable<ThemePickerSettingsRecord> Get();
        void Remove(int id);
        void Add(string name, string ruleType, string criterion, string theme, int priority, string zone, string position);
    }

    [OrchardFeature("Vandelay.ThemePicker")]
    public class SettingsService : ISettingsService {
        private readonly IRepository<ThemePickerSettingsRecord> _repository;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;

        public SettingsService(
            IRepository<ThemePickerSettingsRecord> repository,
            ISignals signals,
            ICacheManager cacheManager) {
            _repository = repository;
            _signals = signals;
            _cacheManager = cacheManager;
        }

        public IEnumerable<ThemePickerSettingsRecord> Get() {
            return _cacheManager.Get("Vandelay.ThemePicker.Settings",
                                     ctx => {
                                         ctx.Monitor(_signals.When("Vandelay.ThemePicker.SettingsChanged"));
                                         return _repository.Table.ToList();
                                     });
        }

        public void Remove(int id) {
            _repository.Delete(_repository.Get(r => r.Id == id));
            _signals.Trigger("Vandelay.ThemePicker.SettingsChanged");
        }

        public void Add(string name, string ruleType, string criterion, string theme, int priority, string zone, string position) {
            _repository.Create(new ThemePickerSettingsRecord {
                Name = name,
                RuleType = ruleType,
                Criterion = criterion,
                Theme = theme,
                Priority = priority,
                Zone = zone,
                Position = position
            });
            _signals.Trigger("Vandelay.ThemePicker.SettingsChanged");
        }
    }
}
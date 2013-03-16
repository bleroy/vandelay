using Orchard;

namespace Vandelay.Industries.Services {
    public interface IThemeSelectionRule : IDependency {
        bool Matches(string name, string criterion);
    }
}
using Betauer.DI;
using Betauer.Managers;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenManager : Betauer.Managers.ScreenManager{
        protected override UserSettingsFile CreateUserSettingsFile() {
            return new UserSettingsFile(new ApplicationConfig.DefaultsUserScreenSettings());
        }
    }
}
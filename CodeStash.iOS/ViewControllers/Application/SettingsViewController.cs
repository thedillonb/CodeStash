using CodeStash.Core.ViewModels.Application;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class SettingsViewController : ViewModelDialogViewController<SettingsViewModel>
    {
        public SettingsViewController()
        {
            Title = "Settings";
        }
    }
}


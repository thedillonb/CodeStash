using CodeStash.Core.ViewModels.Application;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using ReactiveUI;
using CodeFramework.iOS.Views;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class SettingsViewController : ViewModelDialogView<SettingsViewModel>
    {
        public SettingsViewController()
            : base(UITableViewStyle.Grouped)
        {
            Title = "Settings";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            var sectionAccount = new Section("Account");
//            sectionAccount.Add(new TrueFalseElement("Save Credentials", ViewModel.SaveCredentials, e => ViewModel.SaveCredentials = e.Value));
//
            var sectionApplication = new Section("Application");
            var deleteCacheElement = new StyledStringElement("Delete Cache", "0 MB", UITableViewCellStyle.Value1)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            deleteCacheElement.Tapped += () => ViewModel.DeleteCacheCommand.ExecuteIfCan();
            sectionApplication.Add(deleteCacheElement);

            var sectionAbout = new Section("About")
            {
                new StyledStringElement("Follow On Twitter", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/thedillonb"))),
                new StyledStringElement("Rate This App", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codehub-github-for-ios/id707173885?mt=8"))),
                new StyledStringElement("Source Code", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://github.com/thedillonb/CodeStash"))),
                new StyledStringElement("App Version", ViewModel.Version)
            };

            Root = new RootElement(Title) { sectionApplication, sectionAbout };
        }
    }
}


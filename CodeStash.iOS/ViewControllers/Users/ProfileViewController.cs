using System;
using System.Linq;
using System.Reactive.Linq;
using AtlassianStashSharp.Models;
using CodeStash.Core.ViewModels.Users;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Users
{
    public class ProfileViewController : ViewModelPrettyDialogViewController<ProfileViewModel>
    {
        public override void ViewDidLoad()
        {
            Title = ViewModel.UserSlug;

            base.ViewDidLoad();

            HeaderView.Image = CodeFramework.iOS.Images.LoginUserUnknown;

            var repositorySection = new Section();
            Root.Reset(repositorySection);

            ViewModel.WhenAnyValue(x => x.User).Where(x => x != null).Subscribe(x =>
            {
                HeaderView.Text = x.DisplayName;

                var selfLink = x.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                HeaderView.ImageUri = selfLink.Href + "/avatar.png";
            });

            ViewModel.Repositories.Changed.Subscribe(_ => repositorySection.Reset(ViewModel.Repositories.Select(x =>
            {
                var el = new StyledStringElement(x.Name);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            })));

        }
    }
}


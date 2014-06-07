using System;
using System.Linq;
using System.Reactive.Linq;
using AtlassianStashSharp.Models;
using CodeFramework.iOS.Views;
using CodeStash.Core.ViewModels.Users;
using CodeStash.iOS.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Users
{
    public class ProfileViewController : ViewModelDialogView<ProfileViewModel>
    {
        public override void ViewDidLoad()
        {
            Style = UITableViewStyle.Grouped;
            Title = ViewModel.UserSlug;

            base.ViewDidLoad();

            var header = new ImageAndTitleHeaderView { BackgroundColor = UIColor.GroupTableViewBackgroundColor };
            header.Image = CodeFramework.iOS.Images.LoginUserUnknown;
            header.Text = ViewModel.UserSlug;

            var repositorySection = new Section();
            Root = new RootElement(Title) { repositorySection };

            TableView.TableHeaderView = header;

            ViewModel.WhenAnyValue(x => x.User).Where(x => x != null).Subscribe(x =>
            {
                header.Text = x.DisplayName;

                var selfLink = x.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                header.ImageUri = selfLink.Href + "/avatar.png";
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


using System;
using CodeStash.Core.ViewModels.Users;
using CodeStash.iOS.Views;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using MonoTouch.Dialog;
using CodeStash.iOS.ViewControllers.Repositories;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Users
{
    public class ProfileViewController : ViewModelDialogViewController<ProfileViewModel>
    {
        private readonly ImageAndTitleHeaderView _header;

        public ProfileViewController(string userSlug)
            : base(UITableViewStyle.Grouped)
        {
            Title = userSlug;
            ViewModel.UserSlug = userSlug;

            _header = new ImageAndTitleHeaderView() { BackgroundColor = UIColor.Clear };
            _header.Image = Images.LoginUserUnknown;
            _header.Text = userSlug;

            var repositorySection = new Section();
            var root = new RootElement(Title);
            root.Add(repositorySection);
            Root = root;


            ViewModel.WhenAnyValue(x => x.User).Where(x => x != null).Subscribe(x =>
            {
                _header.Text = x.DisplayName;

                var selfLink = x.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                _header.ImageUri = selfLink.Href + "/avatar.png";
            });

            ViewModel.Repositories.Changed.Subscribe(_ => 
            {
                repositorySection.Clear();
                repositorySection.AddAll(ViewModel.Repositories.Select(x => {
                    var el = new StyledStringElement(x.Name);
                    el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                    return el;
                }));
            });

            ViewModel.GoToRepositoryCommand.OfType<Repository>().Subscribe(x =>
            {
                var ctrl = new RepositoryViewController(x.Project.Key, x.Slug);
                NavigationController.PushViewController(ctrl, true);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.TableHeaderView = _header;
        }
    }
}


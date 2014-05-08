using System;
using CodeStash.Core.ViewModels.PullRequests;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.UIKit;
using ReactiveUI;
using CodeStash.Core.Services;
using AtlassianStashSharp.Helpers;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestsViewController : ViewModelDialogViewController<PullRequestsViewModel>
    {
        protected readonly IApplicationService ApplicationService = IoC.Resolve<IApplicationService>();

        public PullRequestsViewController(string projectKey, string repositorySlug)
        {
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
        }

        public override void ViewDidLoad()
        {
            Title = "Pull Requests";

            base.ViewDidLoad();

            ViewModel.PullRequests.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.PullRequests.Select(x => {
                    var element = new NameTimeStringElement
                    {
                        Name = x.Author.User.DisplayName,
                        String = x.Description,
                        Time = UnixDateTimeHelper.FromUnixTime(x.UpdatedDate).ToDaysAgo(),
                        ImageUri = new Uri(ApplicationService.Account.Domain.Replace("/rest/api/1.0", "") + "/users/" + x.Author.User.Slug + "/avatar.png"),
                        Lines = 4
                    };
                    element.Tapped += () => ViewModel.GoToPullRequestCommand.Execute(x);
                    return element;
                }));
                var root = new RootElement(Title) { UnevenRows = true };
                root.Add(sec);
                Root = root;
            });

            var selector = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            ViewModel.WhenAnyValue(x => x.SelectedView).Subscribe(x => selector.SelectedSegment = x);
            selector.ValueChanged += (sender, e) => ViewModel.SelectedView = selector.SelectedSegment;
            NavigationItem.TitleView = selector;
        }
    }
}


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
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var applicationService = IoC.Resolve<IApplicationService>();

            var sec = new Section();
            var root = new RootElement("Pull Requests") { UnevenRows = true };
            root.Add(sec);
            Root = root;

            ViewModel.PullRequests.Changed.Subscribe(_ => sec.Reset(ViewModel.PullRequests.Select(x =>
            {
                var element = new NameTimeStringElement
                {
                    Name = x.Author.User.DisplayName,
                    String = x.Description,
                    Time = UnixDateTimeHelper.FromUnixTime(x.UpdatedDate).ToDaysAgo(),
                    ImageUri = new Uri(applicationService.Account.Domain.Replace("/rest/api/1.0", "") + "/users/" + x.Author.User.Slug + "/avatar.png"),
                    Lines = 4
                };
                element.Tapped += () => ViewModel.GoToPullRequestCommand.Execute(x);
                return element;
            })));

            var selector = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            ViewModel.WhenAnyValue(x => x.SelectedView).Subscribe(x => selector.SelectedSegment = x);
            selector.ValueChanged += (sender, e) => ViewModel.SelectedView = selector.SelectedSegment;
            NavigationItem.TitleView = selector;
        }
    }
}


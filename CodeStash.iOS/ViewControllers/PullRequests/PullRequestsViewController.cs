using System;
using CodeStash.Core.ViewModels.PullRequests;
using System.Linq;
using MonoTouch.UIKit;
using ReactiveUI;
using AtlassianStashSharp.Helpers;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestsViewController : ViewModelDialogViewController<PullRequestsViewModel>
    {
        public PullRequestsViewController()
            : base(unevenRows: true, style: UITableViewStyle.Plain)
        {
            Title = "Pull Requests";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root.Reset(sec);

            ViewModel.PullRequests.Changed.Subscribe(_ => sec.Reset(ViewModel.PullRequests.Select(x =>
            {
                var element = new NameTimeStringElement
                {
                    Name = x.Author.User.DisplayName,
                    String = x.Description,
                    Time = UnixDateTimeHelper.FromUnixTime(x.UpdatedDate).ToDaysAgo(),
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


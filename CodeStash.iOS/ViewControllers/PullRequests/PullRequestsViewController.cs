using System;
using CodeStash.Core.ViewModels.PullRequests;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.UIKit;
using ReactiveUI;
using AtlassianStashSharp.Helpers;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestsViewController : ViewModelDialogViewController<PullRequestsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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


using System;
using CodeStash.Core.ViewModels.PullRequests;
using CodeFramework.iOS.Views;
using ReactiveUI;
using MonoTouch.Dialog;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestParticipantsViewController : ViewModelCollectionView<PullRequestParticipantsViewModel>
    {
        public PullRequestParticipantsViewController()
            : base("Participants")
        {
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Bind(ViewModel.WhenAnyValue(x => x.Participants), x =>
            {
                var element = new StyledStringElement(x.User.DisplayName, x.Approved ? "Approved" : "Has Not Approved");
                element.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;
                element.Tapped += () => ViewModel.GoToUserCommand.ExecuteIfCan(x);
                return element;
            });
        }
    }
}


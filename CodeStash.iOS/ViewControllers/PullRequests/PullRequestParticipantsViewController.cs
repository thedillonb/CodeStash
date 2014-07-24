using System;
using CodeStash.Core.ViewModels.PullRequests;
using CodeFramework.iOS.Views;
using ReactiveUI;
using MonoTouch.Dialog;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestParticipantsViewController : ViewModelCollectionViewController<PullRequestParticipantsViewModel>
    {
        public PullRequestParticipantsViewController()
            : base(searchbarEnabled: false)
        {
            Title = "Participants";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.BindList(ViewModel.Participants, x =>
            {
                var element = new StyledStringElement(x.User.DisplayName, x.Approved ? "Approved" : "Has Not Approved");
                element.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;
                element.Tapped += () => ViewModel.GoToUserCommand.ExecuteIfCan(x);
                return element;
            });
        }
    }
}


using System;
using CodeStash.Core.ViewModels.Commits;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsBranchViewController : ViewModelCollectionViewController<CommitsBranchViewModel>
    {
        public override void ViewDidLoad()
        {
            Title = "Commits";

            this.WhenActivated(d =>
            {
                d(SearchTextChanging.Subscribe(x => ViewModel.SearchKeyword = x));
            });

            var icon = Images.Branch.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            this.BindList(ViewModel.Branches, x => 
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToCommitsCommand.Execute(x));
                element.Image = icon;
                return element;
            });

            base.ViewDidLoad();
        }
    }
}


using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsBranchViewController : ViewModelCollectionView<CommitsBranchViewModel>
    {
        public override void ViewDidLoad()
        {
            var icon = Images.Branch.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            Title = "Commits";

            Bind(ViewModel.WhenAnyValue(x => x.Branches), x => 
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToCommitsCommand.Execute(x));
                element.Image = icon;
                return element;
            });

            base.ViewDidLoad();
        }
    }
}


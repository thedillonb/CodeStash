using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsBranchViewController : ViewModelDialogViewController<CommitsBranchViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement("Commits") { sec };
            var icon = Images.Branch.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);

            ViewModel.Branches.Changed.Subscribe(_ => sec.Reset(ViewModel.Branches.Select(x =>
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToCommitsCommand.Execute(x));
                element.Image = icon;
                return element;
            })));
        }
    }
}


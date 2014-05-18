using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using System.Reactive.Linq;
using MonoTouch.Dialog;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsBranchViewController : ViewModelDialogViewController<CommitsBranchViewModel>
    {
        public CommitsBranchViewController(string projectKey, string repositorySlug)
        {
            Title = "Commits";

            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;

            ViewModel.Branches.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Branches.Select(x => 
                {
                    var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToCommitsCommand.Execute(x));
                    element.Image = Images.Branch;
                    return element;
                }));
                Root = new RootElement(Title) { sec};
            });

            ViewModel.GoToCommitsCommand.OfType<Branch>().Subscribe(x => 
            {
                var ctrl = new CommitsViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, x.Id) { Title = x.DisplayId };
                NavigationController.PushViewController(ctrl, true);
            });
        }
    }
}


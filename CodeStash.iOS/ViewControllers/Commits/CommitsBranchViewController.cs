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
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;

            ViewModel.Branches.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Branches.Select(x => new StyledStringElement(x.DisplayId, () => ViewModel.GoToCommitsCommand.Execute(x))));
                Root = new RootElement(Title) { sec};
            });


            ViewModel.GoToCommitsCommand.OfType<Branch>().Subscribe(x => NavigationController.PushViewController(
                new CommitsViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, x.Id), true));
        }
    }
}


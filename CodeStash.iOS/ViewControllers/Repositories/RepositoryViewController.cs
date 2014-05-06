using System;
using CodeStash.Core.ViewModels.Repositories;
using MonoTouch.Dialog;
using CodeStash.iOS.ViewControllers.Commits;
using CodeStash.iOS.ViewControllers.Source;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoryViewController : ViewModelDialogViewController<RepositoryViewModel>
    {
        public RepositoryViewController()
            : base(MonoTouch.UIKit.UITableViewStyle.Grouped)
        {
            var commitSection = new Section();
            commitSection.Add(new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null)));

            var sourceSection = new Section();
            sourceSection.Add(new StyledStringElement("Source Code", () => ViewModel.GoToSourceCommand.Execute(null)));

            var pullRequestsSection = new Section();
            pullRequestsSection.Add(new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null)));

            var root = new RootElement(Title);
            root.Add(commitSection);
            root.Add(sourceSection);
            root.Add(pullRequestsSection);
            Root = root;

            ViewModel.GoToCommitsCommand.Subscribe(_ => 
                NavigationController.PushViewController(
                    new CommitsBranchViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug), true));

            ViewModel.GoToSourceCommand.Subscribe(_ =>
            {
                var ctrl = new SourceViewController();
                ctrl.ViewModel.ProjectKey = ViewModel.ProjectKey;
                ctrl.ViewModel.RepositorySlug = ViewModel.RepositorySlug;
                NavigationController.PushViewController(ctrl, true);
            });

        }
    }
}


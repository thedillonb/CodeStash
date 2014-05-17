using System;
using CodeStash.Core.ViewModels.Repositories;
using MonoTouch.Dialog;
using CodeStash.iOS.ViewControllers.Commits;
using CodeStash.iOS.ViewControllers.Source;
using CodeStash.iOS.ViewControllers.PullRequests;
using CodeStash.iOS.Views;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Elements;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoryViewController : ViewModelDialogViewController<RepositoryViewModel>
    {
        private readonly ImageAndTitleHeaderView _header;
        private const float _spacing = 10f;

        public RepositoryViewController(string projectKey, string repositorySlug)
        {
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            Title = repositorySlug;

            _header = new ImageAndTitleHeaderView() { BackgroundColor = UIColor.Clear };
            _header.Image = Images.LoginUserUnknown;
            _header.Text = repositorySlug;

            var commitSection = new Section();
            commitSection.Add(new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null)));

            var sourceSection = new Section();
            sourceSection.Add(new SpacingElement(_spacing));
            sourceSection.Add(new StyledStringElement("Source Code", () => ViewModel.GoToSourceCommand.Execute(null)));

            var pullRequestsSection = new Section();
            pullRequestsSection.Add(new SpacingElement(_spacing));
            pullRequestsSection.Add(new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null)));

            var root = new RootElement(Title);
            root.Add(commitSection);
            root.Add(sourceSection);
            root.Add(pullRequestsSection);
            Root = root;

            ViewModel.GoToCommitsCommand.Subscribe(_ =>
            {
                var ctrl = new CommitsBranchViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug);
                NavigationController.PushViewController(ctrl, true);
            });

            ViewModel.GoToSourceCommand.Subscribe(_ =>
            {
                var ctrl = new SourceViewController();
                ctrl.ViewModel.ProjectKey = ViewModel.ProjectKey;
                ctrl.ViewModel.RepositorySlug = ViewModel.RepositorySlug;
                NavigationController.PushViewController(ctrl, true);
            });

            ViewModel.GoToPullRequestsCommand.Subscribe(_ =>
            {
                var ctrl = new PullRequestsViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug);
                NavigationController.PushViewController(ctrl, true);
            });

            ViewModel.WhenAnyValue(x => x.Repository).Where(x => x != null).Subscribe(x =>
            {
                var selfLink = x.Project.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                _header.ImageUri = selfLink.Href + "/avatar.png";
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.TableHeaderView = _header;
            TableView.TableFooterView = new UIView();
            TableView.SeparatorInset = UIEdgeInsets.Zero;
            TableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
        }
    }
}


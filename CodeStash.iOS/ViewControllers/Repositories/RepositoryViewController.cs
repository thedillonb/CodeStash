using System;
using CodeStash.Core.ViewModels.Repositories;
using MonoTouch.Dialog;
using CodeStash.iOS.Views;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoryViewController : ViewModelDialogViewController<RepositoryViewModel>
    {
        private const float _spacing = 10f;

        public override void ViewDidLoad()
        {
            Style = UITableViewStyle.Grouped;
            Title = ViewModel.RepositorySlug;

            base.ViewDidLoad();

            var header = new ImageAndTitleHeaderView() 
            { 
                BackgroundColor = UIColor.Clear,
                EnableSeperator = true,
                SeperatorColor = TableView.SeparatorColor
            };
            header.Image = Images.LoginUserUnknown;
            header.Text = ViewModel.RepositorySlug;

            TableView.TableHeaderView = header;
            TableView.TableFooterView = new UIView();
            TableView.SeparatorInset = UIEdgeInsets.Zero;
            TableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

            var settingsSection = new Section();
            var splitElement = new SplitButtonElement();
            var forksButton = splitElement.AddButton("Forks", "0", () => ViewModel.GoToForksCommand.ExecuteIfCan());
            var releatedButton = splitElement.AddButton("Related", "0", () => ViewModel.GoToRelatedCommand.ExecuteIfCan());
            settingsSection.Add(splitElement);

            var commitSection = new Section();
            commitSection.Add(new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null)));

            var sourceSection = new Section();
            sourceSection.Add(new StyledStringElement("Source Code", () => ViewModel.GoToSourceCommand.Execute(null)));

            var pullRequestsSection = new Section();
            pullRequestsSection.Add(new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null)));

            var root = new RootElement(Title);
            root.Add(settingsSection);
            root.Add(commitSection);
            root.Add(sourceSection);
            root.Add(pullRequestsSection);
            Root = root;

            ViewModel.WhenAnyValue(x => x.Repository).Where(x => x != null).Subscribe(x =>
            {
                var selfLink = x.Project.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                header.ImageUri = selfLink.Href + "/avatar.png";
            });

            ViewModel.WhenAnyValue(x => x.ForkedRepositories).Subscribe(x => forksButton.Text = x.ToString());
            ViewModel.WhenAnyValue(x => x.RelatedRepositories).Subscribe(x => releatedButton.Text = x.ToString());
        }
    }
}


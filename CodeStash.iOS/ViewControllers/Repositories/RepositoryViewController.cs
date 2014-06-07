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

        public RepositoryViewController()
            : base(UITableViewStyle.Grouped)
        {
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.RepositorySlug;

            base.ViewDidLoad();

            var header = new ImageAndTitleHeaderView { BackgroundColor = UIColor.GroupTableViewBackgroundColor };
            header.Image = Images.LoginUserUnknown;
            header.Text = ViewModel.RepositorySlug;

            TableView.TableHeaderView = header;
            TableView.TableFooterView = new UIView();
            TableView.SeparatorInset = UIEdgeInsets.Zero;
            TableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            TableView.SectionHeaderHeight = 0.3f;

            var settingsSection = new Section();
            var splitElement = new SplitButtonElement();
            var forksButton = splitElement.AddButton("Forks", "0", () => ViewModel.GoToForksCommand.ExecuteIfCan());
            var releatedButton = splitElement.AddButton("Related", "0", () => ViewModel.GoToRelatedCommand.ExecuteIfCan());
            settingsSection.Add(splitElement);
//
//            var eventsElement = new StyledStringElement("Events", () => {}, Images.Megaphone.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
//            settingsSection.Add(eventsElement);
//
//            var readmeElement = new StyledStringElement("Readme", () => {}, Images.Readme.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
//            settingsSection.Add(readmeElement);
//
            var codeSection = new Section();
            codeSection.Add(new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null), Images.Commit.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)));
            codeSection.Add(new StyledStringElement("Source Code", () => ViewModel.GoToSourceCommand.Execute(null), Images.Build.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)));
            codeSection.Add(new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null), Images.Merge.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)));

            var root = new RootElement(Title);
            root.Add(settingsSection);
            root.Add(codeSection);
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


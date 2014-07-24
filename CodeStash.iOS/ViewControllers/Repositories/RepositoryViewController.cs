using System;
using CodeStash.Core.ViewModels.Repositories;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoryViewController : ViewModelPrettyDialogViewController<RepositoryViewModel>
    {
        private UIActionSheet _actionSheet;
        private const float _spacing = 10f;

        public override void ViewDidLoad()
        {
            Title = ViewModel.RepositorySlug;

            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowActionMenu());

            HeaderView.Image = Images.ProjectAvatar;
            HeaderView.Text = ViewModel.RepositorySlug;
            TableView.TableHeaderView = HeaderView;

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

            Root.Reset(settingsSection, codeSection);

            ViewModel.WhenAnyValue(x => x.Repository).Where(x => x != null).Subscribe(x =>
            {
                var selfLink = x.Project.Links["self"].FirstOrDefault();
                if (selfLink == null || string.IsNullOrEmpty(selfLink.Href))
                    return;

                HeaderView.ImageUri = selfLink.Href + "/avatar.png";
            });

            ViewModel.WhenAnyValue(x => x.ForkedRepositories).Subscribe(x => forksButton.Text = x.ToString());
            ViewModel.WhenAnyValue(x => x.RelatedRepositories).Subscribe(x => releatedButton.Text = x.ToString());
        }

        private void ShowActionMenu()
        {
            _actionSheet = new UIActionSheet();
            _actionSheet.Title = Title;
            var showinStash = _actionSheet.AddButton("Show in Stash");
            _actionSheet.CancelButtonIndex = _actionSheet.AddButton("Cancel");
            _actionSheet.Dismissed += (sender, e) =>
            {
                if (e.ButtonIndex == showinStash)
                    ViewModel.GoToStashCommand.ExecuteIfCan();
                _actionSheet = null;
            };

            _actionSheet.ShowInView(View);
        }
    }
}


using System;
using MonoTouch.Dialog;
using CodeStash.Core.ViewModels.PullRequests;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestCommitsViewController : ViewModelCollectionView<PullRequestCommitsViewModel>
    {
        public override void ViewDidLoad()
        {
            Title = ViewModel.Title;
            Root = new RootElement(ViewModel.Title) { UnevenRows = true };

            Bind(ViewModel.WhenAnyValue(x => x.Commits), x =>
            {
                var element = new NameTimeStringElement
                {
                    Name = x.Author.Name,
                    Time = AtlassianStashSharp.Helpers.UnixDateTimeHelper.FromUnixTime(x.AuthorTimestamp).ToDaysAgo(),
                    String = x.Message,
                    Lines = 4
                };
                element.Tapped += () => ViewModel.GoToCommitCommand.Execute(x);
                return element;
            });

            base.ViewDidLoad();
        }
    }
}


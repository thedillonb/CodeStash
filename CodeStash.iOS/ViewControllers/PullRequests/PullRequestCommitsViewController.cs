using System;
using MonoTouch.Dialog;
using CodeStash.Core.ViewModels.PullRequests;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestCommitsViewController : ViewModelDialogViewController<PullRequestCommitsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            var root = new RootElement(ViewModel.Title) { UnevenRows = true };
            root.Add(sec);
            Root = root;

            ViewModel.Commits.Changed.Subscribe(_ => sec.Reset(ViewModel.Commits.Select(x =>
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
            })));
        }
    }
}


using System;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsViewController : ViewModelDialogViewController<CommitsViewModel>
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


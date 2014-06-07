using System;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using System.Linq;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsViewController : ViewModelCollectionView<CommitsViewModel>
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


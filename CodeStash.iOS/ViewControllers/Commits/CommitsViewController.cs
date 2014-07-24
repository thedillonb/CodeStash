using System;
using CodeStash.Core.ViewModels.Commits;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsViewController : ViewModelCollectionViewController<CommitsViewModel>
    {
        public CommitsViewController()
            : base(unevenRows: true)
        {
            this.WhenActivated(d =>
            {
                d(SearchTextChanging.Subscribe(x => ViewModel.SearchKeyword = x));
            });
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.Title;

            this.BindList(ViewModel.Commits, x =>
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


using System;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using System.Linq;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitsViewController : ViewModelDialogViewController<CommitsViewModel>
    {
        public CommitsViewController(string projectKey, string repositorySlug, string branch)
        {
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            ViewModel.Branch = branch;

            ViewModel.Commits.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Commits.Select(x => {
                    var element = new NameTimeStringElement
                    { 
                        Name = x.Author.Name, 
                        Time = AtlassianStashSharp.Helpers.UnixDateTimeHelper.FromUnixTime(x.AuthorTimestamp).ToDaysAgo(), 
                        String = x.Message, 
                        Lines = 4 
                    };
                    element.Tapped += () => ViewModel.GoToCommitCommand.Execute(x);
                    return element;
                }));
                var root = new RootElement(Title) { UnevenRows = true };
                root.Add(sec);
                Root = root;
            });

            ViewModel.GoToCommitCommand.OfType<Commit>().Subscribe(x => 
            {
                var ctrl = new CommitViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, x.Id);
                NavigationController.PushViewController(ctrl, true);
            });
        }
    }
}


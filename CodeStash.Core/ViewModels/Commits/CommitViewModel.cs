using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Node { get; set; }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(Node) || Node.Length < 7)
                    return "Commit";
                else
                    return Node.Substring(0, 7);
            }
        }

        private Commit _commit;
        public Commit Commit
        {
            get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
        }

        public ReactiveList<Change> Changes { get; private set; }

        public CommitViewModel(IApplicationService applicationService)
        {
            Changes = new ReactiveList<Change>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                Commit = (await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].Get().ExecuteAsync()).Data;
                var data = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].GetAllChanges().ExecuteAsync();
                Changes.Reset(data.Data.Values);
            });
        }
    }
}


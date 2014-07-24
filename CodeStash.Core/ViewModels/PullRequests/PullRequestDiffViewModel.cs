using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestDiffViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
  
        private Diff _diff;
        public Diff Diff
        {
            get { return _diff; }
            private set { this.RaiseAndSetIfChanged(ref _diff, value); }
        }

        public IReactiveCommand LoadCommand { get; private set; }

        public PullRequestDiffViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Diff = (await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId].GetDiff(Path).ExecuteAsync()).Data;
            });
        }
    }
}


using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using System.Threading.Tasks;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitsBranchViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<Branch> Branches { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public CommitsBranchViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToCommitsCommand = new ReactiveCommand();
            Branches = new ReactiveList<Branch>();
        }

        protected override async Task Load()
        {
            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Branches.GetAll().ExecuteAsync();
            Branches.Reset(response.Data.Values);
        }
    }
}


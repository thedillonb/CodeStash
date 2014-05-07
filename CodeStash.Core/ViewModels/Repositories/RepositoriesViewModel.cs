using System.Threading.Tasks;
using AtlassianStashSharp.Models;
using ReactiveUI;
using CodeStash.Core.Services;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoriesViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public string ProjectKey { get; set; }

        public IReactiveCommand GoToRepositoryCommand { get; private set; }

        public ReactiveList<Repository> Repositories { get; private set; }

        public RepositoriesViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToRepositoryCommand = new ReactiveCommand();
            Repositories = new ReactiveList<Repository>();
        }

        public override async Task Load()
        {
            var d = await ApplicationService.StashClient.Projects[ProjectKey].Repositories.GetAll().ExecuteAsync();
            Repositories.Reset(d.Data.Values);
        }
    }
}


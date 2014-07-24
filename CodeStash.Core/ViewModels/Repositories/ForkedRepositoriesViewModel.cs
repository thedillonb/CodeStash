using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class ForkedRepositoriesViewModel : BaseRepositoriesViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public ForkedRepositoriesViewModel(IApplicationService applicationService)
        {
            ShowOwner = true;

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetForks().ExecuteAsync();
                InternalRepositories.Reset(d.Data.Values);
            });
        }
    }
}


using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RelatedRepositoriesViewModel : BaseRepositoriesViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public RelatedRepositoriesViewModel(IApplicationService applicationService)
        {
            ShowOwner = true;

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetRelated().ExecuteAsync();
                InternalRepositories.Reset(d.Data.Values);
            });
        }
    }
}


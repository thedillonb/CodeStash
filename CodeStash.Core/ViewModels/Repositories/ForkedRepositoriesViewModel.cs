using CodeStash.Core.Services;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class ForkedRepositoriesViewModel : BaseRepositoriesViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ForkedRepositoriesViewModel(IApplicationService applicationService)
        {
            ShowOwner = true;

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetForks().ExecuteAsync();
                Repositories.Reset(d.Data.Values);
            });
        }
    }
}


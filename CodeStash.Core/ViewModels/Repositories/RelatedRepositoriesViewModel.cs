using CodeStash.Core.Services;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RelatedRepositoriesViewModel : BaseRepositoriesViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public RelatedRepositoriesViewModel(IApplicationService applicationService)
        {
            ShowOwner = true;

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetRelated().ExecuteAsync();
                Repositories.Reset(d.Data.Values);
            });
        }
    }
}


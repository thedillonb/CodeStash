using AtlassianStashSharp.Models;
using ReactiveUI;
using CodeStash.Core.Services;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoriesViewModel : BaseRepositoriesViewModel
    {
        public string ProjectKey { get; set; }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            private set { this.RaiseAndSetIfChanged(ref _project, value); }
        }

        public RepositoriesViewModel(IApplicationService applicationService)
        {
            LoadCommand.RegisterAsyncTask(async _ =>
            {
                Project = (await applicationService.StashClient.Projects[ProjectKey].Get().ExecuteAsync()).Data;
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories.GetAll().ExecuteAsync();
                Repositories.Reset(d.Data.Values);
            });
        }
    }
}


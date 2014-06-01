using System;
using AtlassianStashSharp.Models;
using ReactiveUI;
using CodeStash.Core.Services;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoriesViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string Name { get; set; }

        public IReactiveCommand GoToRepositoryCommand { get; private set; }

        public ReactiveList<Repository> Repositories { get; private set; }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            private set { this.RaiseAndSetIfChanged(ref _project, value); }
        }

        public RepositoriesViewModel(IApplicationService applicationService)
        {
            GoToRepositoryCommand = new ReactiveCommand();
            Repositories = new ReactiveList<Repository>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                Project = (await applicationService.StashClient.Projects[ProjectKey].Get().ExecuteAsync()).Data;
                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories.GetAll().ExecuteAsync();
                Repositories.Reset(d.Data.Values);
            });

            GoToRepositoryCommand.OfType<Repository>().Subscribe(x =>
            {
                var vm = this.CreateViewModel<RepositoryViewModel>();
                vm.ProjectKey = x.Project.Key; 
                vm.RepositorySlug = x.Slug;
                ShowViewModel(vm);
            });
        }
    }
}


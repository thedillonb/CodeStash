using System;
using ReactiveUI;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Repositories;

namespace CodeStash.Core.ViewModels.Projects
{
    public class ProjectsViewModel  : LoadableViewModel
    {
        public ReactiveList<Project> Projects { get; private set; }

        public IReactiveCommand GoToProjectCommand { get; private set; }

        public ProjectsViewModel(IApplicationService applicationService)
        {
            GoToProjectCommand = new ReactiveCommand();
            Projects = new ReactiveList<Project>();

            LoadCommand.RegisterAsyncTask(async x =>
            {
                var getAllProjects = applicationService.StashClient.Projects.GetAll();

                Projects.Reset();
                for (var i = 0; i < 500; i++)
                {
                    var data = await getAllProjects.ExecuteAsync(i);
                    Projects.AddRange(data.Data.Values);

                    if (data.Data.NextPageStart.HasValue)
                        i += data.Data.NextPageStart.Value;
                    if (data.Data.IsLastPage)
                        break;
                }
            });

            GoToProjectCommand.OfType<Project>().Subscribe(x =>
            {
                var vm = this.CreateViewModel<RepositoriesViewModel>();
                vm.ProjectKey = x.Key;
                vm.Name = x.Name;
                ShowViewModel(vm);
            });
        }
    }
}


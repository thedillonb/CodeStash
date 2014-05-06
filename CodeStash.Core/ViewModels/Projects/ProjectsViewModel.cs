using System;
using ReactiveUI;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;

namespace CodeStash.Core.ViewModels.Projects
{
    public class ProjectsViewModel  : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public ReactiveList<Project> Projects { get; private set; }

        public IReactiveCommand GoToProjectCommand { get; private set; }

        public ProjectsViewModel(IApplicationService applicationService)
        {
            this.ApplicationService = applicationService;
            GoToProjectCommand = new ReactiveCommand();
            Projects = new ReactiveList<Project>();
        }

        public override async Task Load()
        {
            var d = await ApplicationService.StashClient.Projects.GetAll().ExecuteAsync();

            using (Projects.SuppressChangeNotifications())
            {
                Projects.Clear();
                Projects.AddRange(d.Data.Values);
            }
        }
    }
}


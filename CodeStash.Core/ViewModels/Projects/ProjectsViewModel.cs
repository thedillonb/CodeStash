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
            ApplicationService = applicationService;
            GoToProjectCommand = new ReactiveCommand();
            Projects = new ReactiveList<Project>();
        }

        public override async Task Load()
        {
            Projects.Reset((await ApplicationService.StashClient.Projects.GetAll().ExecuteAsync()).Data.Values);
        }
    }
}


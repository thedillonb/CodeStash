using System;
using ReactiveUI;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private Repository _repository;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public Repository Repository
        {
            get { return _repository; }
            private set { this.RaiseAndSetIfChanged(ref _repository, value); }
        }

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public IReactiveCommand GoToPullRequestsCommand { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public RepositoryViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToSourceCommand = new ReactiveCommand();
            GoToPullRequestsCommand = new ReactiveCommand();
            GoToCommitsCommand = new ReactiveCommand();
        }

        public override async Task Load()
        {
            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Get().ExecuteAsync();
            Repository = response.Data;
        }
    }
}


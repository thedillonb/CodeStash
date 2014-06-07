using System;
using ReactiveUI;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Commits;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public string Title { get; set; }

        public IReactiveCommand GoToCommitCommand { get; private set; }

        public ReactiveList<Commit> Commits { get; private set; }

        public PullRequestCommitsViewModel(IApplicationService applicationService)
        {
            Commits = new ReactiveList<Commit>();

            GoToCommitCommand = new ReactiveCommand();
            GoToCommitCommand.OfType<Commit>().Subscribe(x =>
            {
                var vm = CreateViewModel<CommitViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Node = x.Id;
                ShowViewModel(vm);
            });

            LoadCommand.RegisterAsyncTask(async x =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId].GetAllCommits().ExecuteAsync();
                Commits.Reset(response.Data.Values);
            });
        }
    }
}


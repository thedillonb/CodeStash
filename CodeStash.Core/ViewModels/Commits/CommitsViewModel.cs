using System;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitsViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Branch { get; set; }

        public string Title { get; set; }

        public IReactiveCommand GoToCommitCommand { get; private set; }

        public ReactiveCollection<Commit> Commits { get; private set; }

        public CommitsViewModel(IApplicationService applicationService)
        {
            Commits = new ReactiveCollection<Commit>();

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
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits.GetAll(until: Branch).ExecuteAsync();
                Commits.Reset(response.Data.Values);
            });
        }
    }
}


using System;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitsBranchViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<Branch> Branches { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public CommitsBranchViewModel(IApplicationService applicationService)
        {
            GoToCommitsCommand = new ReactiveCommand();
            Branches = new ReactiveList<Branch>();

            LoadCommand.RegisterAsyncTask(async x =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Branches.GetAll().ExecuteAsync();
                Branches.Reset(response.Data.Values);
            });

            GoToCommitsCommand.OfType<Branch>().Subscribe(x =>
            {
                var vm = CreateViewModel<CommitsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Branch = x.Id;
                vm.Title = x.DisplayId;
                ShowViewModel(vm);
            });
        }
    }
}


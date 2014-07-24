using System;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitsBranchViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public IReadOnlyReactiveList<Branch> Branches { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public IReactiveCommand<object> GoToCommitsCommand { get; private set; }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        public CommitsBranchViewModel(IApplicationService applicationService)
        {
            GoToCommitsCommand = ReactiveCommand.Create();

            var branches = new ReactiveCollection<Branch>();
            Branches = branches.CreateDerivedCollection(x => x, 
                x => x.DisplayId.IndexOf(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0,
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async x =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Branches.GetAll().ExecuteAsync();
                branches.Reset(response.Data.Values);
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


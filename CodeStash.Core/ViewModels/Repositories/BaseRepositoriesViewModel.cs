using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using AtlassianStashSharp.Models;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Repositories
{
    public abstract class BaseRepositoriesViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public bool ShowOwner { get; set; }

        public IReactiveCommand<object> GoToRepositoryCommand { get; private set; }

        public IReadOnlyReactiveList<Repository> Repositories { get; private set; }

        protected ReactiveList<Repository> InternalRepositories { get; private set; }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        protected BaseRepositoriesViewModel()
        {
            GoToRepositoryCommand = ReactiveCommand.Create();

            InternalRepositories = new ReactiveCollection<Repository>();
            Repositories = InternalRepositories.CreateDerivedCollection(x => x, 
                x => x.Name.IndexOf(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0,
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            ShowOwner = false;

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


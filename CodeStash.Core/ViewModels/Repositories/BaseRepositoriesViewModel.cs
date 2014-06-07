using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using AtlassianStashSharp.Models;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Repositories
{
    public abstract class BaseRepositoriesViewModel : LoadableViewModel
    {
        public string Name { get; set; }

        public bool ShowOwner { get; set; }

        public IReactiveCommand GoToRepositoryCommand { get; private set; }

        public ReactiveCollection<Repository> Repositories { get; private set; }

        protected BaseRepositoriesViewModel()
        {
            GoToRepositoryCommand = new ReactiveCommand();
            Repositories = new ReactiveCollection<Repository>();

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


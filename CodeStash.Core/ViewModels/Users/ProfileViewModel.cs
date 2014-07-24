using System;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using ReactiveUI;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Repositories;

namespace CodeStash.Core.ViewModels.Users
{
    public class ProfileViewModel : BaseViewModel, ILoadableViewModel
    {
        public string UserSlug { get; set; }

        public ReactiveList<Repository> Repositories { get; private set; }

        public IReactiveCommand<object> GoToRepositoryCommand { get; private set; }

        private User _user;
        public User User
        {
            get { return _user; }
            private set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        public IReactiveCommand LoadCommand { get; private set; }

        public ProfileViewModel(IApplicationService applicationService)
        {
            Repositories = new ReactiveList<Repository>();
            GoToRepositoryCommand = ReactiveCommand.Create();

            GoToRepositoryCommand.OfType<Repository>().Subscribe(x =>
            {
                var vm = CreateViewModel<RepositoryViewModel>();
                vm.ProjectKey = x.Project.Key;
                vm.RepositorySlug = x.Slug;
                ShowViewModel(vm);
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                User = (await applicationService.StashClient.Users[UserSlug].Get().ExecuteAsync()).Data;
                Repositories.Reset((await applicationService.StashClient.Users[UserSlug].Repositories.GetAll().ExecuteAsync()).Data.Values);
            });
        }
    }
}


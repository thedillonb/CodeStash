using System;
using Xamarin.Utilities.Core.ViewModels;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.Users
{
    public class ProfileViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public ReactiveList<Repository> Repositories { get; private set; }

        public IReactiveCommand GoToRepositoryCommand { get; private set; }

        public string UserSlug { get; set; }

        private User _user;
        public User User
        {
            get { return _user; }
            private set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        public ProfileViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            Repositories = new ReactiveList<Repository>();
            GoToRepositoryCommand = new ReactiveCommand();
        }

        protected override async Task Load()
        {
            User = (await ApplicationService.StashClient.Users[UserSlug].Get().ExecuteAsync()).Data;
            Repositories.Reset((await ApplicationService.StashClient.Users[UserSlug].Repositories.GetAll().ExecuteAsync()).Data.Values);
        }
    }
}


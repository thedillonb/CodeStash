using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using CodeStash.Core.ViewModels.Users;
using CodeStash.Core.Services;
using CodeFramework.Core.ViewModels.Application;
using CodeFramework.Core.Services;

namespace CodeStash.Core.ViewModels.Application
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        public IReactiveCommand<object> GoToSettingsCommand { get; private set; }

        public IReactiveCommand<object> GoToAccountsCommand { get; private set; }

        public IReactiveCommand<object> GoToProfileCommand { get; private set; }

        public MainViewModel(IAccountsService accountsService)
        {
            GoToSettingsCommand = ReactiveCommand.Create();
            GoToSettingsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<SettingsViewModel>()));

            GoToAccountsCommand = ReactiveCommand.Create();
            GoToAccountsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<AccountsViewModel>()));

            GoToProfileCommand = ReactiveCommand.Create();
            GoToProfileCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<ProfileViewModel>();
                vm.UserSlug = accountsService.ActiveAccount.Username;
                ShowViewModel(vm);
            });
        }
    }
}


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
        public IReactiveCommand GoToSettingsCommand { get; private set; }

        public IReactiveCommand GoToAccountsCommand { get; private set; }

        public IReactiveCommand GoToProfileCommand { get; private set; }

        public MainViewModel(IAccountsService accountsService)
        {
            GoToSettingsCommand = new ReactiveCommand();
            GoToSettingsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<SettingsViewModel>()));

            GoToAccountsCommand = new ReactiveCommand();
            GoToAccountsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<AccountsViewModel>()));

            GoToProfileCommand = new ReactiveCommand();
            GoToProfileCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<ProfileViewModel>();
                vm.UserSlug = accountsService.ActiveAccount.Username;
                ShowViewModel(vm);
            });
        }
    }
}


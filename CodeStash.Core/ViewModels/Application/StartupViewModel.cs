using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeStash.Core.Data;
using ReactiveUI;
using CodeStash.Core.Services;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Application
{
    public class StartupViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private Account _account;

        public IReactiveCommand GoToAccountsCommand { get; private set; }

        public IReactiveCommand GoToNewUserCommand { get; private set; }

        public IReactiveCommand GoToMainCommand { get; private set; }

        public IReactiveCommand BecomeActiveWindowCommand { get; private set; }

        public Account Account
        {
            get { return _account; }
            private set { this.RaiseAndSetIfChanged(ref _account, value); }
        }

        public StartupViewModel(IApplicationService application)
        {
            ApplicationService = application;
            GoToMainCommand = new ReactiveCommand();
            GoToAccountsCommand = new ReactiveCommand();
            GoToNewUserCommand = new ReactiveCommand();
            BecomeActiveWindowCommand = new ReactiveCommand();
            LoadCommand.RegisterAsyncTask(x => Load());

            GoToAccountsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<AccountsViewModel>()));
            GoToMainCommand.Subscribe(_ => ShowViewModel(CreateViewModel<MainViewModel>()));
            GoToNewUserCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<LoginViewModel>();
                vm.WhenAnyValue(x => x.LoggedInAcconut).Skip(1).Subscribe(x => application.Account = x);
                ShowViewModel(vm);
            });
        }

        private void GoToAccountsOrNewUser()
        {
            if (ApplicationService.Accounts.Any())
                GoToAccountsCommand.Execute(null);
            else
                GoToNewUserCommand.Execute(null);
        }

        private async Task Load()
        {
            Account = ApplicationService.DefaultAccount;

            // Account no longer exists
            if (Account == null)
            {
                GoToAccountsOrNewUser();
            }
            else
            {
                try
                {
                    // Attempt a login
                    var client = AtlassianStashSharp.StashClient.CrateBasic(new Uri(Account.Domain), Account.Username, Account.Password);
                    var info = await client.Users[Account.Username].Get().ExecuteAsync();

                    // Maybe attempt to get the avatar image?

                    if (string.IsNullOrEmpty(Account.AvatarUrl))
                    {
                        var selfLink = info.Data.Links["self"].FirstOrDefault();
                        if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                        {
                            Account.AvatarUrl = selfLink.Href + "/avatar.png";
                            ApplicationService.Accounts.Update(Account);
                        }
                    }

                    ApplicationService.StashClient = client;
                    ApplicationService.Account = Account;

                    GoToMainCommand.Execute(null);
                }
                catch
                {
                    GoToAccountsCommand.ExecuteIfCan();
                }
            }
        }
    }
}


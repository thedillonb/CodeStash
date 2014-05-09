using System;
using System.Linq;
using System.Threading.Tasks;
using CodeStash.Core.Data;
using ReactiveUI;
using CodeStash.Core.Services;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Application
{
    public class StartupViewModel : LoadableViewModel
    {
        protected readonly IApplicationService Application;
        private Account _account;

        public IReactiveCommand GoToAccountsCommand { get; private set; }

        public IReactiveCommand GoToNewUserCommand { get; private set; }

        public IReactiveCommand GoToMainCommand { get; private set; }

        public Account Account
        {
            get { return _account; }
            private set { this.RaiseAndSetIfChanged(ref _account, value); }
        }

        public StartupViewModel(IApplicationService application)
        {
            Application = application;
            GoToMainCommand = new ReactiveCommand();
            GoToAccountsCommand = new ReactiveCommand();
            GoToNewUserCommand = new ReactiveCommand();
        }

        private void GoToAccountsOrNewUser()
        {
            if (Application.Accounts.Any())
                GoToAccountsCommand.Execute(null);
            else
                GoToNewUserCommand.Execute(null);
        }

        public override async Task Load()
        {
            Account = Application.DefaultAccount;

            // Account no longer exists
            if (Account == null)
            {
                GoToAccountsOrNewUser();
            }
            else
            {
                // Attempt a login
                var client = AtlassianStashSharp.StashClient.CrateBasic(Account.Domain, Account.Username, Account.Password);
                var info = await client.ApplicationProperties.Get().ExecuteAsync();

                // Maybe attempt to get the avatar image?

                Console.WriteLine("Attempted login found: " + info.Data.DisplayName);

                Application.StashClient = client;
                Application.Account = Account;

                GoToMainCommand.Execute(null);
            }
        }
    }
}


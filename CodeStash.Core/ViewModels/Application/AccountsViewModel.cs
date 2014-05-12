using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeStash.Core.Data;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Application
{
    public class AccountsViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public ReactiveList<Account> Accounts { get; private set; } 

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set { this.RaiseAndSetIfChanged(ref _selectedAccount, value); }
        }

        public ReactiveCommand LoginCommand { get; private set; }

        public ReactiveCommand AddAccountCommand { get; private set; }

        public AccountsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            Accounts = new ReactiveList<Account>();
            LoginCommand = new ReactiveCommand();
            AddAccountCommand = new ReactiveCommand();

            this.WhenAnyValue(x => x.SelectedAccount).Where(x => x != null).Subscribe(x =>
            {
                ApplicationService.Account = x;
                DismissCommand.Execute(null);
            });

            LoginCommand.RegisterAsyncTask(async t =>
            {
                var account = t as Account;
                if (account == null)
                    return;

                var client = AtlassianStashSharp.StashClient.CrateBasic(account.Domain, account.Username, account.Password);
                await client.Projects.GetAll().ExecuteAsync();
                ApplicationService.Account = account;
                DismissCommand.Execute(null);
            });
        }

        protected override Task Load()
        {
            Accounts.Reset(ApplicationService.Accounts);
            return Task.FromResult(true);
        }
    }

}
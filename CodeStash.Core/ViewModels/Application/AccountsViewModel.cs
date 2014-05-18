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

        public IReactiveCommand LoginCommand { get; private set; }

        public IReactiveCommand AddAccountCommand { get; private set; }

        public IReactiveCommand DeleteAccountCommand { get; private set; }

        public AccountsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            Accounts = new ReactiveList<Account>();
            LoginCommand = new ReactiveCommand();
            AddAccountCommand = new ReactiveCommand();
            DeleteAccountCommand = new ReactiveCommand();

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

                var client = AtlassianStashSharp.StashClient.CrateBasic(new Uri(account.Domain), account.Username, account.Password);
                await client.Projects.GetAll().ExecuteAsync();
                ApplicationService.Account = account;
                DismissCommand.Execute(null);
            });

            DeleteAccountCommand.OfType<Account>().Subscribe(x =>
            {
                if (ApplicationService.Account != null && x.Id == ApplicationService.Account.Id)
                {
                    //Removing the current Account
                    ApplicationService.Account = null;
                    ApplicationService.StashClient = null;
                }

                //ApplicationService.Accounts.Remove(x);
                Accounts.Remove(x);
            });
        }

        protected override Task Load()
        {
            Accounts.Reset(ApplicationService.Accounts);
            return Task.FromResult(true);
        }
    }

}
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeStash.Core.Data;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Messages;

namespace CodeStash.Core.ViewModels.Application
{
    public class AccountsViewModel : LoadableViewModel
    {
        public ReactiveList<Account> Accounts { get; private set; } 

        public IReactiveCommand LoginCommand { get; private set; }

        public IReactiveCommand GoToAddAccountCommand { get; private set; }

        public IReactiveCommand DeleteAccountCommand { get; private set; }

        public AccountsViewModel(IApplicationService applicationService)
        {
            Accounts = new ReactiveList<Account>();
            LoginCommand = new ReactiveCommand();
            GoToAddAccountCommand = new ReactiveCommand();
            DeleteAccountCommand = new ReactiveCommand();

            LoginCommand.OfType<Account>().Subscribe(x =>
            {
                if (object.Equals(applicationService.Account, x))
                    DismissCommand.ExecuteIfCan();
                else
                {
                    applicationService.Account = x;
                    MessageBus.Current.SendMessage(new LogoutMessage());
                    DismissCommand.ExecuteIfCan();
                }
            });

            DeleteAccountCommand.OfType<Account>().Subscribe(x =>
            {
                if (applicationService.Account != null && x.Id == applicationService.Account.Id)
                {
                    //Removing the current Account
                    applicationService.Account = null;
                    applicationService.StashClient = null;
                }

                applicationService.Accounts.Remove(x);
                Accounts.Remove(x);
            });

            GoToAddAccountCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<LoginViewModel>();
                vm.WhenAnyValue(x => x.LoggedInAcconut).Skip(1).Subscribe(x => LoadCommand.ExecuteIfCan());
                ShowViewModel(vm);
            });

            LoadCommand.Subscribe(x => Accounts.Reset(applicationService.Accounts));
        }
    }

}
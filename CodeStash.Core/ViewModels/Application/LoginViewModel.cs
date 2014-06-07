using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using System;
using CodeFramework.Core.ViewModels.Application;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using System.Linq;
using CodeFramework.Core.Messages;

namespace CodeStash.Core.ViewModels.Application
{
    public class LoginViewModel : BaseViewModel, IAddAccountViewModel
    {
        private string _username;
        public string Username
        {
            get { return _username; }   
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _domain;
        public string Domain
        {
            get { return _domain; }
            set { this.RaiseAndSetIfChanged(ref _domain, value); }
        }

        private Account _loggedInAccount;
        public Account LoggedInAcconut
        {
            get { return _loggedInAccount; }
            private set { this.RaiseAndSetIfChanged(ref _loggedInAccount, value); }
        }

        public IReactiveCommand LoginCommand { get; private set; }

        public LoginViewModel(IAccountsService accountsService)
        {
            LoginCommand = new ReactiveCommand(this.WhenAny(x => x.Username, x => x.Password, x => x.Domain, (u, p, d) => 
                !string.IsNullOrEmpty(u.Value) && !string.IsNullOrEmpty(p.Value) && !string.IsNullOrEmpty(d.Value)));

            LoginCommand.RegisterAsyncTask(async x =>
            {
                var domain = Domain.TrimEnd('/');
                var client = AtlassianStashSharp.StashClient.CrateBasic(new Uri(domain), Username, Password);
                var info = await client.Users[Username].Get().ExecuteAsync();

                var account = new Account {Username = Username, Password = Password, Domain = domain};
                if (string.IsNullOrEmpty(account.AvatarUrl))
                {
                    var selfLink = info.Data.Links["self"].FirstOrDefault();
                    if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    {
                        account.AvatarUrl = selfLink.Href + "/avatar.png";
                    }
                }

                accountsService.Insert(account);
                accountsService.ActiveAccount = account;
                LoggedInAcconut = account;
                MessageBus.Current.SendMessage(new LogoutMessage());
                //DismissCommand.ExecuteIfCan();
            });
        }

    }
}
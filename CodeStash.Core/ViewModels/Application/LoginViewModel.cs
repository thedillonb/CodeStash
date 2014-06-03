using CodeStash.Core.Data;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using System;

namespace CodeStash.Core.ViewModels.Application
{
    public class LoginViewModel : BaseViewModel
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

        public LoginViewModel(IApplicationService applicationService)
        {
            LoginCommand = new ReactiveCommand(this.WhenAny(x => x.Username, x => x.Password, x => x.Domain, (u, p, d) => 
                !string.IsNullOrEmpty(u.Value) && !string.IsNullOrEmpty(p.Value) && !string.IsNullOrEmpty(d.Value)));

            LoginCommand.RegisterAsyncTask(async x =>
            {
                var domain = Domain.TrimEnd('/');
                var client = AtlassianStashSharp.StashClient.CrateBasic(new Uri(domain), Username, Password);
                await client.Projects.GetAll().ExecuteAsync();

                var account = new Account {Username = Username, Password = Password, Domain = domain};
                applicationService.Accounts.Insert(account);
                applicationService.Account = account;
                LoggedInAcconut = account;
                DismissCommand.ExecuteIfCan();
            });
        }

    }
}
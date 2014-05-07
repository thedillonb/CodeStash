using CodeStash.Core.Data;
using CodeStash.Core.Services;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.Application
{
    public class LoginViewModel : ReactiveObject
    {
        protected readonly IApplicationService ApplicationService;
        private string _username;
        private string _password;
        private string _domain;

        public string Username
        {
            get { return _username; }   
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public string Domain
        {
            get { return _domain; }
            set { this.RaiseAndSetIfChanged(ref _domain, value); }
        }

        public IReactiveCommand LoginCommand { get; private set; }

        public IReactiveCommand DismissCommand { get; private set; }

        public LoginViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            DismissCommand = new ReactiveCommand();

            LoginCommand = new ReactiveCommand(this.WhenAny(x => x.Username, x => x.Password, x => x.Domain, (u, p, d) => 
                !string.IsNullOrEmpty(u.Value) && !string.IsNullOrEmpty(p.Value) && !string.IsNullOrEmpty(d.Value)));

            LoginCommand.RegisterAsyncTask(async x =>
            {
                var client = AtlassianStashSharp.StashClient.CrateBasic(Domain, Username, Password);
                await client.Projects.GetAll().ExecuteAsync();

                var account = new Account {Username = Username, Password = Password, Domain = Domain};
                ApplicationService.Accounts.Insert(account);
                ApplicationService.Account = account;
                DismissCommand.Execute(null);
            });
        }

    }
}
using System;
using AtlassianStashSharp;
using Xamarin.Utilities.Core.Services;
using CodeFramework.Core.Services;
using CodeFramework.Core.Data;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeStash.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        protected readonly IDefaultValueService DefaultValueService;
        protected readonly IAccountsService AccountsService;

        public StashClient StashClient { get; private set; }

        public ApplicationService(IDefaultValueService defaultValueService, IAccountsService accountsService)
        {
            DefaultValueService = defaultValueService;
            AccountsService = accountsService;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            AccountsService.WhenAnyObservable(x => x.ActiveAccountChanged).StartWith(AccountsService.ActiveAccount).Subscribe(account =>
            {
                StashClient = account != null ? AtlassianStashSharp.StashClient.CrateBasic(new Uri(account.Domain), account.Username, account.Password) : null;
            });
        }
    }
}


using System;
using System.Linq;
using AtlassianStashSharp;
using CodeStash.Core.Data;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        protected readonly IDefaultValueService DefaultValueService;
        private Account _account;

        public ApplicationService(IDefaultValueService defaultValueService)
        {
            DefaultValueService = defaultValueService;
            Accounts = new Accounts(Database.Instance.SqlConnection);
        }

        public StashClient StashClient { get; set; }

        public Accounts Accounts { get; private set; }

        public Account DefaultAccount
        {
            get
            {
                int id;
                Account account = null;
                if (DefaultValueService.TryGet("DEFAULT_ACCOUNT_ID", out id))
                    account = Accounts.FirstOrDefault(x => x.Id == id);
                return account;
            }
        }

        public Account Account
        {
            get { return _account; }
            set
            {
                _account = value;
                DefaultValueService.Set("DEFAULT_ACCOUNT_ID", value != null ? (object)value.Id : null);
            }
        }
    }
}


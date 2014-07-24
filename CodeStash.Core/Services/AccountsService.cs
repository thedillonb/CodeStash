using System;
using CodeFramework.Core.Services;
using CodeStash.Core.Data;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.Core.Services
{
    public class AccountsService : BaseAccountsService<Account>
    {
        public AccountsService(IDefaultValueService defaultValueService)
            : base(defaultValueService)
        {
        }
    }
}


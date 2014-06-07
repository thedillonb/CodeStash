using System;
using CodeFramework.Core.Services;
using System.Threading.Tasks;
using System.Linq;

namespace CodeStash.Core.Services
{
    public class AccountValidatorService : IAccountValidatorService
    {
        private readonly IAccountsService _accounts;

        public AccountValidatorService(IAccountsService accounts)
        {
            _accounts = accounts;
        }

        public async Task Validate(CodeFramework.Core.Data.IAccount account)
        {
            var client = AtlassianStashSharp.StashClient.CrateBasic(new Uri(account.Domain), account.Username, account.Password);
            var info = await client.Users[account.Username].Get().ExecuteAsync();

            if (string.IsNullOrEmpty(account.AvatarUrl))
            {
                var selfLink = info.Data.Links["self"].FirstOrDefault();
                if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                {
                    account.AvatarUrl = selfLink.Href + "/avatar.png";
                    _accounts.Update(account);
                }
            }
        }
    }
}


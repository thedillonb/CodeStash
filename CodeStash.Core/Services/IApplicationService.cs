using System;
using AtlassianStashSharp;
using CodeStash.Core.Data;

namespace CodeStash.Core.Services
{
    public interface IApplicationService
    {
        StashClient StashClient { get; set; }

        Accounts Accounts { get; }

        Account DefaultAccount { get; }

        Account Account { get; set; }
    }
}


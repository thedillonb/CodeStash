using System;
using AtlassianStashSharp;

namespace CodeStash.Core.Services
{
    public interface IApplicationService
    {
        StashClient StashClient { get; }
    }
}


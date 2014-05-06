using System;
using AtlassianStashSharp;

namespace CodeStash.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        public ApplicationService()
        {
        }

        public StashClient StashClient { get; set; }
    }
}


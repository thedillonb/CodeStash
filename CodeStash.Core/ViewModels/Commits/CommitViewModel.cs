﻿using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private Commit _commit;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Node { get; set; }

        public Commit Commit
        {
            get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
        }

        public ReactiveList<Change> Changes { get; private set; }

        public CommitViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            Changes = new ReactiveList<Change>();
        }

        public override async Task Load()
        {
            Commit = (await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].Get().ExecuteAsync()).Data;

            var data = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].GetAllChanges().ExecuteAsync();
            using (Changes.SuppressChangeNotifications())
            {
                Changes.Clear();
                Changes.AddRange(data.Data.Values);
            }
        }
    }
}


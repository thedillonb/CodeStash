﻿using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitsViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Branch { get; set; }

        public IReactiveCommand GoToCommitCommand { get; private set; }

        public ReactiveList<Commit> Commits { get; private set; }

        public CommitsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToCommitCommand = new ReactiveCommand();
            Commits = new ReactiveList<Commit>();
        }

        public override async Task Load()
        {
            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits.GetAll(until: Branch).ExecuteAsync();
            using (Commits.SuppressChangeNotifications())
            {
                Commits.Clear();
                Commits.AddRange(response.Data.Values);
            }
        }
    }
}

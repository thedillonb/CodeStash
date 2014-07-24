﻿using System;
using ReactiveUI;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Commits;
using Xamarin.Utilities.Core.ViewModels;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public string Title { get; set; }

        public IReactiveCommand<object> GoToCommitCommand { get; private set; }

        public IReadOnlyReactiveList<Commit> Commits { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        public PullRequestCommitsViewModel(IApplicationService applicationService)
        {
            var commits = new ReactiveCollection<Commit>();
            Commits = commits.CreateDerivedCollection(x => x, 
                x => x.Message.IndexOf(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0,
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            GoToCommitCommand = ReactiveCommand.Create();
            GoToCommitCommand.OfType<Commit>().Subscribe(x =>
            {
                var vm = CreateViewModel<CommitViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Node = x.Id;
                ShowViewModel(vm);
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async x =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId].GetAllCommits().ExecuteAsync();
                commits.Reset(response.Data.Values);
            });
        }
    }
}


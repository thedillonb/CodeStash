using System;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.ViewModels.Build;
using System.Threading.Tasks;
using System.Linq;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : LoadableViewModel
    {
        private PullRequest _pullRequest;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public PullRequest PullRequest
        {
            get { return _pullRequest; }
            set { this.RaiseAndSetIfChanged(ref _pullRequest, value); }
        }

        private BuildStatus[] _buildStatus;
        public BuildStatus[] BuildStatus
        {
            get { return _buildStatus; }
            private set { this.RaiseAndSetIfChanged(ref _buildStatus, value); }
        }

        private MergeResult _mergeResult;
        public MergeResult MergeResult
        {
            get { return _mergeResult; }
            private set { this.RaiseAndSetIfChanged(ref _mergeResult, value); }
        }

        public ReactiveList<Commit> Commits { get; private set; }

        public ReactiveList<PullRequestParticipant> Participants { get; private set; }

        public ReactiveList<Activity> Activities { get; private set; }

        public IReactiveCommand GoToChangesCommand { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public IReactiveCommand GoToBuildStatusCommand { get; private set; }

        public IReactiveCommand GoToCommentsCommand { get; private set; }

        public IReactiveCommand GoToParticipantsCommand { get; private set; }


        public PullRequestViewModel(IApplicationService applicationService)
        {
            Commits = new ReactiveList<Commit>();
            Participants = new ReactiveList<PullRequestParticipant>();
            Activities = new ReactiveList<Activity>();
            GoToChangesCommand = new ReactiveCommand();
            GoToCommitsCommand = new ReactiveCommand();
            GoToCommentsCommand = new ReactiveCommand();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var pullRequest = applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId];
                PullRequest = (await pullRequest.Get().ExecuteAsync()).Data;
                Participants.Reset(PullRequest.Participants.Concat(PullRequest.Reviewers));
                Commits.Reset((await pullRequest.GetAllCommits().ExecuteAsync()).Data.Values);

                pullRequest.GetAllActivities().ExecuteAsync().ContinueInBackground(x => Activities.Reset(x.Data.Values));
                pullRequest.GetMergeResult().ExecuteAsync().ContinueInBackground(x => MergeResult = x.Data);

                var firstCommit = Commits.FirstOrDefault();
                if (firstCommit != null)
                {
                    applicationService.StashClient.BuildStatus[firstCommit.Id].GetStatus()
                        .ExecuteAsync().ContinueInBackground(x => BuildStatus = x.Data.Values.ToArray());
                }
            });

            GoToCommitsCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestCommitsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                vm.Title = "Commits";
                ShowViewModel(vm);
            });

            GoToChangesCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestChangesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                vm.PullRequestDestination = PullRequest.ToRef.Id;
                ShowViewModel(vm);
            });

            GoToBuildStatusCommand = new ReactiveCommand(this.WhenAnyValue(x => x.BuildStatus, x => x != null && x.Length > 0));
            GoToBuildStatusCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<BuildStatusesViewModel>();
                vm.Node = Commits[0].Id;
                ShowViewModel(vm);
            });

            GoToParticipantsCommand = new ReactiveCommand();
            GoToParticipantsCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestParticipantsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                ShowViewModel(vm);
            });

        }
    }
}
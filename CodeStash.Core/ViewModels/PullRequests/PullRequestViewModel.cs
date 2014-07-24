using System;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.ViewModels.Build;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive.Linq;
using CodeFramework.Core.Services;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : BaseViewModel, ILoadableViewModel
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

        public IReactiveCommand<object> GoToChangesCommand { get; private set; }

        public IReactiveCommand<object> GoToCommitsCommand { get; private set; }

        public IReactiveCommand<object> GoToBuildStatusCommand { get; private set; }

        public IReactiveCommand<object> GoToCommentsCommand { get; private set; }

        public IReactiveCommand<object> GoToParticipantsCommand { get; private set; }

        public IReactiveCommand<object> GoToStashCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public PullRequestViewModel(IApplicationService applicationService, IAccountsService accountsService)
        {
            Commits = new ReactiveList<Commit>();
            Participants = new ReactiveList<PullRequestParticipant>();
            Activities = new ReactiveList<Activity>();
            GoToChangesCommand = ReactiveCommand.Create();
            GoToCommitsCommand = ReactiveCommand.Create();
            GoToCommentsCommand = ReactiveCommand.Create();

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
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

            GoToBuildStatusCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.BuildStatus, x => x != null && x.Length > 0));
            GoToBuildStatusCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<BuildStatusesViewModel>();
                vm.Node = Commits[0].Id;
                ShowViewModel(vm);
            });

            GoToParticipantsCommand = ReactiveCommand.Create();
            GoToParticipantsCommand.Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestParticipantsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                ShowViewModel(vm);
            });

            GoToStashCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.PullRequest).Select(x => x != null));
            GoToStashCommand.Select(x => PullRequest).Subscribe(x =>
            {
                var vm = CreateViewModel<WebBrowserViewModel>();
                if (x.Links == null || !x.Links.ContainsKey("self"))
                {
                    if (x.Link.Url.StartsWith("http", StringComparison.Ordinal))
                        vm.Url = x.Link.Url;
                    else
                        vm.Url = accountsService.ActiveAccount.Domain.TrimEnd('/') + "/" + x.Link.Url.TrimStart('/');
                }
                else
                {
                    vm.Url = x.Links["self"][0].Href;
                }

                ShowViewModel(vm);
            });
        }
    }
}
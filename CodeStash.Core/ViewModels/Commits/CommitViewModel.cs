using System;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive.Linq;
using AtlassianStashSharp.Helpers;
using CodeStash.Core.ViewModels.Build;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Node { get; set; }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(Node) || Node.Length < 7)
                    return "Commit";
                else
                    return Node.Substring(0, 7);
            }
        }

        private Commit _commit;
        public Commit Commit
        {
            get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
        }

        private BuildStatus[] _buildStatus;
        public BuildStatus[] BuildStatus
        {
            get { return _buildStatus; }
            private set { this.RaiseAndSetIfChanged(ref _buildStatus, value); }
        }

        private bool? _watching;
        public bool? IsWatching
        {
            get { return _watching; }
            private set { this.RaiseAndSetIfChanged(ref _watching, value); }
        }

        public ReactiveList<Branch> Branches { get; private set; }

        public ReactiveList<Change> Changes { get; private set; }

        public IReactiveCommand GoToBuildStatusCommand { get; private set; }

        public IReactiveCommand GoToParentCommitCommand { get; private set; }

        public IReactiveCommand GoToBranchesCommand { get; private set; }

        public IReactiveCommand GoToCommentsCommand { get; private set; }

        public IReactiveCommand GoToDiffCommand { get; private set; }

        public CommitViewModel(IApplicationService applicationService)
        {
            Changes = new ReactiveList<Change>();
            Branches = new ReactiveList<Branch>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                applicationService.StashClient.BranchUtilities.GetBranches(ProjectKey, RepositorySlug, Node)
                    .ExecuteAsync().ContinueInBackground(x => Branches.Reset(x.Data.Values));

                applicationService.StashClient.BuildStatus[Node].GetStatus()
                    .ExecuteAsync().ContinueInBackground(x => BuildStatus = x.Data.Values.ToArray());

                Commit = (await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].Get().ExecuteAsync()).Data;

                Changes.Reset(await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].GetAllChanges().ExecuteAsyncAll());
            });

            GoToBuildStatusCommand = new ReactiveCommand(this.WhenAnyValue(x => x.BuildStatus, x => x != null && x.Length > 0));
            GoToBuildStatusCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<BuildStatusesViewModel>();
                vm.Node = Node;
                ShowViewModel(vm);
            });

            GoToParentCommitCommand = new ReactiveCommand(this.WhenAnyValue(x => x.Commit, x => x != null && x.Parents != null && x.Parents.Count > 0));
            GoToParentCommitCommand.Subscribe(_ =>
            {
//                if (Commit.Parents.Count > 1)
//                {
//                    // Oh shit... More than one...
//                }
//                else
                {
                    var firstParent = Commit.Parents.FirstOrDefault();
                    var vm = CreateViewModel<CommitViewModel>();
                    vm.ProjectKey = ProjectKey;
                    vm.RepositorySlug = RepositorySlug;
                    vm.Node = firstParent.Id;
                    ShowViewModel(vm);
                }
            });

            GoToBranchesCommand = new ReactiveCommand(this.Branches.CountChanged.Select(x => x > 0));
            GoToBranchesCommand.Subscribe(_ =>
            {
//                if (Branches.Count > 1)
//                {
//                    // Oh shit... More than one...
//                }
//                else
                {
                    var firstParent = Branches.FirstOrDefault();
                    var vm = CreateViewModel<CommitsViewModel>();
                    vm.ProjectKey = ProjectKey;
                    vm.RepositorySlug = RepositorySlug;
                    vm.Branch = firstParent.Id;
                    vm.Title = firstParent.DisplayId;
                    ShowViewModel(vm);
                }
            });

            GoToCommentsCommand = new ReactiveCommand();

            GoToDiffCommand = new ReactiveCommand(this.WhenAnyValue(x => x.Commit, x => x != null));
            GoToDiffCommand.OfType<Change>().Subscribe(x =>
            {
                var vm = CreateViewModel<CommitDiffViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Node = Node;
                vm.Path = x.Path.ToString;
                vm.Name = x.Path.Name;

                var parentCommit = Commit.Parents.FirstOrDefault();
                if (parentCommit != null)
                    vm.NodeParent = parentCommit.Id;

                ShowViewModel(vm);
            });
        }
    }
}


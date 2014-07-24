using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using AtlassianStashSharp.Helpers;
using System.Reactive.Linq;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestChangesViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public string PullRequestDestination { get; set; }

        public ReactiveList<Change> Changes { get; private set; }

        public IReactiveCommand<object> GoToDiffCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public PullRequestChangesViewModel(IApplicationService applicationService)
        {
            Changes = new ReactiveList<Change>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Changes.Reset(await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId].GetAllChanges().ExecuteAsyncAll());
            });
         
            GoToDiffCommand = ReactiveCommand.Create();
            GoToDiffCommand.OfType<Change>().Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestDiffViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                vm.Path = x.Path.ToString;
                vm.Name = x.Path.Name;
                ShowViewModel(vm);
            });
        }
    }
}


using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using ReactiveUI;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private int _selectedView;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<PullRequest> PullRequests { get; private set; }

        public IReactiveCommand GoToPullRequestCommand { get; private set; }

        public int SelectedView
        {
            get { return _selectedView; }
            set { this.RaiseAndSetIfChanged(ref _selectedView, value); }
        }

        public PullRequestsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToPullRequestCommand = new ReactiveCommand();
            PullRequests = new ReactiveList<PullRequest>();

            this.WhenAnyValue(x => x.SelectedView).Skip(1).Subscribe(_ => LoadCommand.Execute(null));
        }

        public override async Task Load()
        {
            string state;
            switch (SelectedView)
            {
                case 1:
                    state = "MERGED";
                    break;
                case 2:
                    state = "DECLINED";
                    break;
                default:
                    state = "OPEN";
                    break;
            }

            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests.GetAll(state: state).ExecuteAsync();
            PullRequests.Reset(response.Data.Values);
        }
    }
}
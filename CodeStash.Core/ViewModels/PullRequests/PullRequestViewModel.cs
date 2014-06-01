using System.Threading.Tasks;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : LoadableViewModel
    {
        private PullRequest _pullRequest;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public int PullRequestId { get; set; }

        public PullRequest PullRequest
        {
            get { return _pullRequest; }
            set { this.RaiseAndSetIfChanged(ref _pullRequest, value); }
        }

        public ReactiveList<Commit> Commits { get; private set; }

        public ReactiveList<PullRequestParticipant> Participants { get; private set; }

        public ReactiveList<Change> Changes { get; private set; }

        public ReactiveList<Comment> Comments { get; private set; } 

        public PullRequestViewModel(IApplicationService applicationService)
        {
            Commits = new ReactiveList<Commit>();
            Participants = new ReactiveList<PullRequestParticipant>();
            Changes = new ReactiveList<Change>();
            Comments = new ReactiveList<Comment>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var pullRequest = applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId];
                PullRequest = (await pullRequest.Get().ExecuteAsync()).Data;
                Commits.Reset((await pullRequest.GetAllCommits().ExecuteAsync()).Data.Values);
                Participants.Reset((await pullRequest.GetAllParticipates().ExecuteAsync()).Data.Values);
                Changes.Reset((await pullRequest.GetAllChanges().ExecuteAsync()).Data.Values);
                Comments.Reset((await pullRequest.GetAllComments().ExecuteAsync()).Data.Values);
            });
        }
    }
}
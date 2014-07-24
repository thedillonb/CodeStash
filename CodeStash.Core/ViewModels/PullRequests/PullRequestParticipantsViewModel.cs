using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Users;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestParticipantsViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public IReactiveCommand<object> GoToUserCommand { get; private set; }

        public ReactiveCollection<PullRequestParticipant> Participants { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public PullRequestParticipantsViewModel(IApplicationService applicationService)
        {
            GoToUserCommand = ReactiveCommand.Create();
            Participants = new ReactiveCollection<PullRequestParticipant>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var pullRequest = applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId];
                Participants.Reset((await pullRequest.GetAllParticipates().ExecuteAsync()).Data.Values);
            });

            GoToUserCommand.OfType<PullRequestParticipant>().Subscribe(x =>
            {
                var vm = CreateViewModel<ProfileViewModel>();
                vm.UserSlug = x.User.Slug;
                ShowViewModel(vm);
            });
        }
    }
}


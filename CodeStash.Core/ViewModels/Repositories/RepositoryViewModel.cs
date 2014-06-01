using System;
using ReactiveUI;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.ViewModels.Source;
using CodeStash.Core.ViewModels.PullRequests;
using CodeStash.Core.ViewModels.Commits;
using System.Reactive.Linq;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        private Repository _repository;
        public Repository Repository
        {
            get { return _repository; }
            private set { this.RaiseAndSetIfChanged(ref _repository, value); }
        }

        private string _avatarPath;
        public string AvatarPath
        {
            get { return _avatarPath; }
            private set { this.RaiseAndSetIfChanged(ref _avatarPath, value); }
        }

        private int _relatedRepositories;
        public int RelatedRepositories
        {
            get { return _relatedRepositories; }
            private set { this.RaiseAndSetIfChanged(ref _relatedRepositories, value); }
        }

        private int _forkedRepositories;
        public int ForkedRepositories
        {
            get { return _forkedRepositories; }
            private set { this.RaiseAndSetIfChanged(ref _forkedRepositories, value); }
        }

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public IReactiveCommand GoToPullRequestsCommand { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public IReactiveCommand GoToForksCommand { get; private set; }

        public IReactiveCommand GoToRelatedCommand { get; private set; }


        public RepositoryViewModel(IApplicationService applicationService)
        {
            LoadCommand.RegisterAsyncTask(async _ => 
            {
                applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetRelated().ExecuteAsync().ContinueWith(t =>
                {
                    if (t.Exception == null)
                        RelatedRepositories = t.Result.Data.Values.Count;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetForks().ExecuteAsync().ContinueWith(t =>
                {
                    if (t.Exception == null)
                        ForkedRepositories = t.Result.Data.Values.Count;
                }, TaskScheduler.FromCurrentSynchronizationContext());
     
                Repository = (await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Get().ExecuteAsync()).Data;
            });

            GoToSourceCommand = new ReactiveCommand();
            GoToSourceCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<SourceViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToPullRequestsCommand = new ReactiveCommand();
            GoToPullRequestsCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<PullRequestsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToCommitsCommand = new ReactiveCommand();
            GoToCommitsCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<CommitsBranchViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToForksCommand = new ReactiveCommand(this.WhenAnyValue(x => x.ForkedRepositories).Any(x => x > 0));
            GoToForksCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<ForkedRepositoriesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Name = "Forks";
                ShowViewModel(vm);
            });

            GoToRelatedCommand = new ReactiveCommand(this.WhenAnyValue(x => x.RelatedRepositories).Any(x => x > 0));
            GoToRelatedCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<RelatedRepositoriesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Name = "Related";
                ShowViewModel(vm);
            });
        }

        private async Task<string> LoadAvatar(IApplicationService applicationService)
        {
            var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
            var avatarPath = System.IO.Path.Combine (documents, "..", "Library", "Caches", "Avatars");
            if (!System.IO.Directory.Exists(avatarPath))
                System.IO.Directory.CreateDirectory(avatarPath);

            avatarPath = System.IO.Path.Combine(avatarPath, "avatar." + ProjectKey + ".png");
            if (!System.IO.File.Exists(avatarPath))
            {
                var image = await applicationService.StashClient.Projects[ProjectKey].GetAvatar("128").ExecuteAsync();
                Console.WriteLine("Length: " + image.Data.Length);
                await Task.Run(() => System.IO.File.WriteAllBytes(avatarPath, System.Text.Encoding.UTF8.GetBytes(image.Data)));
            }

            return avatarPath;
        }
    }
}


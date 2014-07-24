using System;
using ReactiveUI;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using CodeStash.Core.ViewModels.Source;
using CodeStash.Core.ViewModels.PullRequests;
using CodeStash.Core.ViewModels.Commits;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ViewModels;
using CodeFramework.Core.Services;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : BaseViewModel, ILoadableViewModel
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

        public IReactiveCommand<object> GoToSourceCommand { get; private set; }

        public IReactiveCommand<object> GoToPullRequestsCommand { get; private set; }

        public IReactiveCommand<object> GoToCommitsCommand { get; private set; }

        public IReactiveCommand<object> GoToForksCommand { get; private set; }

        public IReactiveCommand<object> GoToRelatedCommand { get; private set; }

        public IReactiveCommand<object> GoToStashCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public RepositoryViewModel(IApplicationService applicationService, IAccountsService accountsService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => 
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

            GoToSourceCommand = ReactiveCommand.Create();
            GoToSourceCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<SourceViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToPullRequestsCommand = ReactiveCommand.Create();
            GoToPullRequestsCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<PullRequestsViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToCommitsCommand = ReactiveCommand.Create();
            GoToCommitsCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<CommitsBranchViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                ShowViewModel(vm);
            });

            GoToForksCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.ForkedRepositories).Any(x => x > 0));
            GoToForksCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<ForkedRepositoriesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Name = "Forks";
                ShowViewModel(vm);
            });

            GoToRelatedCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.RelatedRepositories).Any(x => x > 0));
            GoToRelatedCommand.Subscribe(_ =>
            {
                var vm = CreateViewModel<RelatedRepositoriesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Name = "Related";
                ShowViewModel(vm);
            });

            GoToStashCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.Repository).Select(x => x != null));
            GoToStashCommand.Select(x => Repository).Subscribe(x =>
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


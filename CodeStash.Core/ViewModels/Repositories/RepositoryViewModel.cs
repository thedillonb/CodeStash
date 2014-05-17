using System;
using ReactiveUI;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;

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

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public IReactiveCommand GoToPullRequestsCommand { get; private set; }

        public IReactiveCommand GoToCommitsCommand { get; private set; }

        public RepositoryViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToSourceCommand = new ReactiveCommand();
            GoToPullRequestsCommand = new ReactiveCommand();
            GoToCommitsCommand = new ReactiveCommand();
        }

        protected override async Task Load()
        {
            Repository = (await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Get().ExecuteAsync()).Data;
            //AvatarPath = await LoadAvatar();
        }

        private async Task<string> LoadAvatar()
        {
            var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
            var avatarPath = System.IO.Path.Combine (documents, "..", "Library", "Caches", "Avatars");
            if (!System.IO.Directory.Exists(avatarPath))
                System.IO.Directory.CreateDirectory(avatarPath);

            avatarPath = System.IO.Path.Combine(avatarPath, "avatar." + ProjectKey + ".png");
            if (!System.IO.File.Exists(avatarPath))
            {
                var image = await ApplicationService.StashClient.Projects[ProjectKey].GetAvatar("128").ExecuteAsync();
                Console.WriteLine("Length: " + image.Data.Length);
                await Task.Run(() => System.IO.File.WriteAllBytes(avatarPath, System.Text.Encoding.UTF8.GetBytes(image.Data)));
            }

            return avatarPath;
        }
    }
}


using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Source
{
    public class FilesViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private ContentPath _contentPath;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public ReactiveList<Content> Contents { get; private set; }

        public ContentPath ContentPath
        {
            get { return _contentPath; }
            private set { this.RaiseAndSetIfChanged(ref _contentPath, value); }
        }

        public FilesViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToSourceCommand = new ReactiveCommand();
            Contents = new ReactiveList<Content>();
        }

        protected override async Task Load()
        {
            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetContents(Path, Branch, false).ExecuteAsync();
            ContentPath = response.Data.Path;
            Contents.Reset(response.Data.Children.Values);
        }
    }
}


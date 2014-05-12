using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using ReactiveUI;
using System.Text;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Source
{
    public class FileViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private string _content;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public string Content
        {
            get { return _content; }
            private set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public FileViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
        }

        protected override async Task Load()
        {
            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetFileContent(Path, Branch).ExecuteAsync();
            var content = new StringBuilder();
            foreach (var line in response.Data.Lines)
                content.AppendLine(line.Text);
            Content = content.ToString();
        }
    }
}


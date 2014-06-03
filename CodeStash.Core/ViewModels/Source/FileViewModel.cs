using System;
using CodeStash.Core.Services;
using ReactiveUI;
using System.Text;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Source
{
    public class FileViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public string FileName { get; set; }

        private string _content;
        public string Content
        {
            get { return _content; }
            private set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public FileViewModel(IApplicationService applicationService)
        {
            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetFileContent(Path, Branch).ExecuteAsync();
                if (response.Data.Lines == null)
                    throw new Exception("Unable to render this type of file.");

                var content = new StringBuilder();
                foreach (var line in response.Data.Lines)
                    content.AppendLine(line.Text);
                Content = content.ToString().Trim();
            });
        }
    }
}


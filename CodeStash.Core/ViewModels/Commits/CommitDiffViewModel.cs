using System;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using System.Threading.Tasks;
using System.Text;
using AtlassianStashSharp.Models;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitDiffViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Node { get; set; }

        public string NodeParent { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
  
        private string _newContent;
        public string NewContent
        {
            get { return _newContent; }
            private set { this.RaiseAndSetIfChanged(ref _newContent, value); }
        }

        private string _oldContent;
        public string OldContent
        {
            get { return _oldContent; }
            private set { this.RaiseAndSetIfChanged(ref _oldContent, value); }
        }

        public CommitDiffViewModel(IApplicationService applicationService)
        {
            LoadCommand.RegisterAsyncTask(async _ =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(Node))
                        NewContent = await GetContent(applicationService, Node);
                } catch {}

                try
                {
                    if (!string.IsNullOrEmpty(NodeParent))
                        OldContent = await GetContent(applicationService, NodeParent);
                } catch {}

                if (NewContent == null && OldContent == null)
                    throw new Exception("Unable to diff this type of file.");
            });
        }

        private async Task<string> GetContent(IApplicationService applicationService, string node)
        {
            var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetFileContent(Path, node).ExecuteAsync();
            if (response.Data.Lines == null)
                throw new Exception("Unable to render this type of file.");

            var content = new StringBuilder();
            foreach (var line in response.Data.Lines)
                content.AppendLine(line.Text);
            return content.ToString().Trim();
        }
    }
}


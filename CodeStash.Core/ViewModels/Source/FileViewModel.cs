using System;
using CodeStash.Core.Services;
using ReactiveUI;
using System.Text;
using CodeFramework.Core.ViewModels.Source;

namespace CodeStash.Core.ViewModels.Source
{
    public class FileViewModel : FileSourceViewModel<string>
    {
        private IReactiveCommand _loadCommand;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public string FileName { get; set; }

        public override IReactiveCommand LoadCommand 
        {
            get { return _loadCommand; }
        }

        public FileViewModel(IApplicationService applicationService)
        {
            _loadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetFileContent(Path, Branch).ExecuteAsync();
                if (response.Data.Lines == null)
                    throw new Exception("Unable to render this type of file.");

                var file = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "source");
                using (var s = new System.IO.StreamWriter(file, false, Encoding.UTF8))
                    foreach (var line in response.Data.Lines)
                        s.WriteLine(line.Text);

                SourceItem = new FileSourceItemViewModel() { IsBinary = false, FilePath = file };
            });

            SetupRx();
        }
    }
}


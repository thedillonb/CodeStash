using System;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Source
{
    public class FilesViewModel : BaseViewModel, ILoadableViewModel
    {
        private ContentPath _contentPath;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public string Folder { get; set; }

        public IReactiveCommand<object> GoToSourceCommand { get; private set; }

        public IReadOnlyReactiveList<Content> Contents { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { this.RaiseAndSetIfChanged(ref _searchKeyword, value); }
        }

        public ContentPath ContentPath
        {
            get { return _contentPath; }
            private set { this.RaiseAndSetIfChanged(ref _contentPath, value); }
        }

        public FilesViewModel(IApplicationService applicationService)
        {
            GoToSourceCommand = ReactiveCommand.Create();

            var contents = new ReactiveCollection<Content>();
            Contents = contents.CreateDerivedCollection(x => x, 
                x => x.Path.Name.IndexOf(SearchKeyword ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0,
                signalReset: this.WhenAnyValue(x => x.SearchKeyword));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetContents(Path, Branch, false).ExecuteAsync();
                ContentPath = response.Data.Path;
                contents.Reset(response.Data.Children.Values);
            });

            GoToSourceCommand.OfType<Content>().Subscribe(x =>
            {
                if (string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase))
                {
                    var vm = CreateViewModel<FileViewModel>();
                    vm.ProjectKey = ProjectKey;
                    vm.RepositorySlug = RepositorySlug;
                    vm.Branch = Branch;
                    vm.Path = Path + "/" + x.Path.Name;
                    vm.FileName = x.Path.Name;
                    ShowViewModel(vm);
                }
                else
                {
                    var vm = CreateViewModel<FilesViewModel>();
                    vm.ProjectKey = ProjectKey;
                    vm.RepositorySlug = RepositorySlug;
                    vm.Branch = Branch;
                    vm.Path = Path + "/" + x.Path.Name;
                    vm.Folder = x.Path.Name;
                    ShowViewModel(vm);
                }
            });
        }
    }
}


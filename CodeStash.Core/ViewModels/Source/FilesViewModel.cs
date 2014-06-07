using System;
using CodeStash.Core.Services;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Source
{
    public class FilesViewModel : LoadableViewModel
    {
        private ContentPath _contentPath;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Path { get; set; }

        public string Branch { get; set; }

        public string Folder { get; set; }

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public ReactiveCollection<Content> Contents { get; private set; }

        public ContentPath ContentPath
        {
            get { return _contentPath; }
            private set { this.RaiseAndSetIfChanged(ref _contentPath, value); }
        }

        public FilesViewModel(IApplicationService applicationService)
        {
            GoToSourceCommand = new ReactiveCommand();
            Contents = new ReactiveCollection<Content>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].GetContents(Path, Branch, false).ExecuteAsync();
                ContentPath = response.Data.Path;
                Contents.Reset(response.Data.Children.Values);
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


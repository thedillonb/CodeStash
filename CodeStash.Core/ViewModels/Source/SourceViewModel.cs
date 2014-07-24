using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Source
{
    public class SourceViewModel : BaseViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<Tag> Tags { get; private set; }

        public ReactiveList<Branch> Branches { get; private set; }

        public IReactiveCommand<object> GoToSourceCommand { get; private set; }

        private int _selectedView;
        public int SelectedView
        {
            get { return _selectedView; }
            set { this.RaiseAndSetIfChanged(ref _selectedView, value); }
        }

        public IReactiveCommand LoadCommand { get; private set; }

        public SourceViewModel(IApplicationService applicationService)
        {
            GoToSourceCommand = ReactiveCommand.Create();
            Tags = new ReactiveList<Tag>();
            Branches = new ReactiveList<Branch>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => Load(applicationService));

            this.WhenAnyValue(x => x.SelectedView).Skip(1).Subscribe(_ => LoadCommand.Execute(null));

            GoToSourceCommand.OfType<Tag>().Subscribe(x => 
            {
                var vm = CreateViewModel<FilesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Branch = x.LatestChangeset;
                vm.Folder = x.DisplayId;
                ShowViewModel(vm);
            });

            GoToSourceCommand.OfType<Branch>().Subscribe(x => 
            {
                var vm = CreateViewModel<FilesViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.Branch = x.LatestChangeset;
                vm.Folder = x.DisplayId;
                ShowViewModel(vm);
            });
        }

        private async Task Load(IApplicationService applicationService)
        {
            if (SelectedView == 0)
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Branches.GetAll().ExecuteAsync();
                Branches.Reset(response.Data.Values);
            }
            else
            {
                var response = await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Tags.GetAll().ExecuteAsync();
                Tags.Reset(response.Data.Values);
            }
        }
    }
}


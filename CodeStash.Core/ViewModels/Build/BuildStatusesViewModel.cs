using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeStash.Core.ViewModels.Build
{
    public class BuildStatusesViewModel : LoadableViewModel
    {
        public string Node { get; set; }

        public ReactiveCollection<BuildStatus> BuildStatues { get; private set; }

        public IReactiveCommand GoToBuildStatusCommand { get; private set; }

        public BuildStatusesViewModel(IApplicationService applicationService)
        {
            BuildStatues = new ReactiveCollection<BuildStatus>();
            GoToBuildStatusCommand = new ReactiveCommand();

            GoToBuildStatusCommand.OfType<BuildStatus>().Subscribe(x =>
            {
                var vm = CreateViewModel<WebBrowserViewModel>();
                vm.Url = x.Url;
                ShowViewModel(vm);
            });

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                BuildStatues.Reset((await applicationService.StashClient.BuildStatus[Node].GetStatus().ExecuteAsync()).Data.Values);
            });
        }
    }
}


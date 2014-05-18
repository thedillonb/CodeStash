using System;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.Core.ViewModels.Application
{
    public class SettingsViewModel : BaseViewModel
    {
        protected readonly IApplicationService ApplicationService;
        protected readonly IEnvironmentalService EnvironmentalService;

        public string Version
        {
            get { return EnvironmentalService.ApplicationVersion; }
        }

        private bool _saveCredientials;
        public bool SaveCredentials
        {
            get { return _saveCredientials; }
            set { this.RaiseAndSetIfChanged(ref _saveCredientials, value); }
        }

        public IReactiveCommand DeleteCacheCommand { get; private set; }

        public SettingsViewModel(IApplicationService applicationService, IEnvironmentalService environmentalService)
        {
            ApplicationService = applicationService;
            EnvironmentalService = environmentalService;
            DeleteCacheCommand = new ReactiveCommand();
            SaveCredentials = ApplicationService.Account.SaveCredentials;

            this.WhenAnyValue(x => x.SaveCredentials).Skip(1).Subscribe(x =>
            {
                ApplicationService.Account.SaveCredentials = x;
                ApplicationService.Accounts.Update(ApplicationService.Account);
            });
        }
    }
}
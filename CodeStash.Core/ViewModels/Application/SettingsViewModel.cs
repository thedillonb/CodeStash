using System;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using System.Reactive.Linq;

namespace CodeStash.Core.ViewModels.Application
{
    public class SettingsViewModel : BaseViewModel
    {
        protected readonly IApplicationService ApplicationService;

        public string Version
        {
            get;
            set;
        }

        private bool _saveCredientials;
        public bool SaveCredentials
        {
            get { return _saveCredientials; }
            set { this.RaiseAndSetIfChanged(ref _saveCredientials, value); }
        }

        public IReactiveCommand DeleteCacheCommand { get; private set; }

        public SettingsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
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
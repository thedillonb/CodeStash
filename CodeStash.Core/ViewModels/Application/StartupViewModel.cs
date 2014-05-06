using System;
using System.Threading.Tasks;
using ReactiveUI;
using CodeStash.Core.Services;

namespace CodeStash.Core.ViewModels.Application
{
    public class StartupViewModel : LoadableViewModel
    {
        public IReactiveCommand GoToMainCommand { get; private set; }

        public StartupViewModel()
        {
            GoToMainCommand = new ReactiveCommand();

            IoC.Resolve<IApplicationService>().StashClient = 
                AtlassianStashSharp.StashClient.CrateBasic("http://192.168.2.11:7990/stash/rest/api/1.0", "admin", "admin");
        }

        public override Task Load()
        {
            throw new NotImplementedException();
        }
    }
}


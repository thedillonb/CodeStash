using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ReactiveUI;

namespace CodeStash.Core.ViewModels.Application
{
    public class SettingsViewModel : ReactiveObject
    {
        public IReactiveCommand DismissCommand { get; private set; }

        public SettingsViewModel()
        {
            DismissCommand = new ReactiveCommand();
        }
    }
}
using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class SettingsViewController : DialogViewController
    {
        public SettingsViewController()
            : base(UITableViewStyle.Grouped, null, true)
        {
        }
    }
}


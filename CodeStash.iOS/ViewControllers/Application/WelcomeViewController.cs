using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class WelcomeViewController : DialogViewController
    {
        public WelcomeViewController()
            : base(UITableViewStyle.Plain, null, true)
        {
        }

        public override void ViewDidLoad()
        {
            Title = "Welcome!";
            base.ViewDidLoad();
        }
    }
}


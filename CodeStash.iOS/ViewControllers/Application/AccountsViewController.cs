using System;
using System.Linq;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class AccountsViewController : ViewModelDialogViewController<AccountsViewModel>
    {
        public AccountsViewController()
        {
            ViewModel.Accounts.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Accounts.Select(x =>
                {
                    var element = new StyledStringElement(x.Username, x.Domain, UITableViewCellStyle.Subtitle)
                    {
                        Accessory = UITableViewCellAccessory.DisclosureIndicator
                    };
                    element.Tapped += () => ViewModel.LoadCommand.Execute(x);
                    return element;
                }));
                Root = new RootElement(Title) {sec};
            });

            ViewModel.AddAccountCommand.Subscribe(_ =>
            {
                var ctrl = new LoginViewController();
                ctrl.ViewModel.DismissCommand.Subscribe(__ => NavigationController.PopViewControllerAnimated(true));
                NavigationController.PushViewController(ctrl, true);
            });
        }

        public override void ViewDidLoad()
        {
            Title = "Accounts";
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => ViewModel.AddAccountCommand.Execute(null));
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel.LoadCommand.Execute(null);
        }
    }
}


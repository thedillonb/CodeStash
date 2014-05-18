using System;
using System.Linq;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.iOS.Elements;
using MonoTouch.Foundation;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class AccountsViewController : ViewModelDialogViewController<AccountsViewModel>
    {
        public AccountsViewController()
        {
            ViewModel.Accounts.ItemsRemoved.Where(x => Root != null && Root.Count > 0).Subscribe(x =>
            {
                foreach (var element in Root[0].Elements.ToList())
                {
                    if (element is ProfileElement && object.Equals(((ProfileElement)element).Tag, x))
                        Root[0].Remove(element);
                }
            });

            ViewModel.Accounts.Changed.Where(x => x.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove).Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Accounts.Select(x =>
                {
                    var shortenedDomain = new Uri(x.Domain);
                    var element = new ProfileElement(x.Username, shortenedDomain.Host) 
                    { 
                        Accessory = UITableViewCellAccessory.DisclosureIndicator,
                        Tag = x
                    };
                    element.Image = Images.LoginUserUnknown;
                    element.ImageUri = x.AvatarUrl;
                    element.Tapped += () => ViewModel.LoginCommand.ExecuteIfCan(x);
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
            TableView.RowHeight = 74f;
            TableView.SeparatorInset = new UIEdgeInsets(0, TableView.RowHeight, 0, 0);

            base.ViewDidLoad();
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new DialogDeleteSource(this, x => 
            {
                var profileElement = x as ProfileElement;
                if (profileElement == null)
                    return;

                var account = profileElement.Tag as CodeStash.Core.Data.Account;
                if (account != null)
                    ViewModel.DeleteAccountCommand.ExecuteIfCan(account);
            });
        }

        public class DialogDeleteSource : DialogViewController.Source
        {
            private readonly Action<Element> _deleteAction;

            public DialogDeleteSource(DialogViewController container, Action<Element> deleteAction)
                : base(container)
            {
                _deleteAction = deleteAction;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                var section = Container.Root[indexPath.Section];
                var element = section[indexPath.Row];
                if (_deleteAction != null)
                    _deleteAction(element);
            }
        }
    }
}


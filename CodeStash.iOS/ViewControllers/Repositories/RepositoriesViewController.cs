using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : ViewModelDialogViewController<RepositoriesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement(ViewModel.Name) { sec };

            ViewModel.Repositories.Changed.Subscribe(_ => sec.Reset(ViewModel.Repositories.Select(x =>
            {
                var el = new StyledStringElement(x.Name);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            })));
        }
    }
}


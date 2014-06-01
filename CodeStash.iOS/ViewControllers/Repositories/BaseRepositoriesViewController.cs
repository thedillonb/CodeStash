using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public abstract class BaseRepositoriesViewController<TViewModel> : ViewModelDialogViewController<TViewModel> where TViewModel : BaseRepositoriesViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.SeparatorInset = UIEdgeInsets.Zero;

            var sec = new Section();
            Root = new RootElement(ViewModel.Name) { sec };

            ViewModel.Repositories.Changed.Subscribe(_ => sec.Reset(ViewModel.Repositories.Select(x =>
            {
                var el = new StyledStringElement(x.Name, ViewModel.ShowOwner ? x.Project.Name : null, UITableViewCellStyle.Subtitle);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            })));
        }
    }
}


using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeStash.Core.ViewModels.Repositories;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public abstract class BaseRepositoriesViewController<TViewModel> : ViewModelCollectionView<TViewModel> where TViewModel : BaseRepositoriesViewModel
    {
        public override void ViewDidLoad()
        {
            Title = ViewModel.Name;
            TableView.SeparatorInset = UIEdgeInsets.Zero;

            base.ViewDidLoad();

            this.Bind(ViewModel.WhenAnyValue(x => x.Repositories), x =>
            {
                var el = new StyledStringElement(x.Name, ViewModel.ShowOwner ? x.Project.Name : null, UITableViewCellStyle.Subtitle);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            });
        }
    }
}


using System;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Repositories;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public abstract class BaseRepositoriesViewController<TViewModel> : ViewModelCollectionViewController<TViewModel> where TViewModel : BaseRepositoriesViewModel
    {
        protected BaseRepositoriesViewController(bool searchbar = true)
            : base(searchbarEnabled: searchbar)
        {
            this.WhenActivated(d =>
            {
                d(SearchTextChanging.Subscribe(x => ViewModel.SearchKeyword = x));
            });
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.Name;
            TableView.SeparatorInset = UIEdgeInsets.Zero;

            base.ViewDidLoad();

            this.BindList(ViewModel.Repositories, x =>
            {
                var el = new StyledStringElement(x.Name, ViewModel.ShowOwner ? x.Project.Name : null, UITableViewCellStyle.Subtitle);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            });
        }
    }
}


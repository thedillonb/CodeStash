using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using Xamarin.Utilities.Core.Services;
using System;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers
{
    public abstract class ViewModelDialogViewController<TViewModel> : DialogViewController, IViewFor<TViewModel> where TViewModel : ReactiveObject
    {
        protected readonly INetworkActivityService NetworkActivityService = IoC.Resolve<INetworkActivityService>();
        private UIRefreshControl _refreshControl;
        private bool _loaded;

        public TViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected ViewModelDialogViewController(UITableViewStyle style = UITableViewStyle.Plain)
            : base(style, null, true)
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = string.Empty };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var loadableViewModel = ViewModel as LoadableViewModel;
            if (loadableViewModel != null)
            {
                _refreshControl = new UIRefreshControl();
                RefreshControl = _refreshControl;
                _refreshControl.ValueChanged += (s, e) => loadableViewModel.LoadCommand.ExecuteIfCan();
                loadableViewModel.LoadCommand.IsExecuting.Where(x => !x).Subscribe(x => _refreshControl.EndRefreshing());
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!_loaded)
            {
                _loaded = true;
                var loadableViewModel = ViewModel as LoadableViewModel;
                if (loadableViewModel != null)
                    loadableViewModel.LoadCommand.ExecuteIfCan();
            }
        }
    }
}


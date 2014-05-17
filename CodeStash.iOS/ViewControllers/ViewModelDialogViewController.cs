using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using Xamarin.Utilities.Core.Services;
using System;

namespace CodeStash.iOS.ViewControllers
{
    public abstract class ViewModelDialogViewController<TViewModel> : DialogViewController where TViewModel : ReactiveObject
    {
        private readonly TViewModel _viewModel = IoC.Resolve<TViewModel>();
        protected readonly INetworkActivityService NetworkActivityService = IoC.Resolve<INetworkActivityService>();
        private UIRefreshControl _refreshControl;


        public TViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected ViewModelDialogViewController(UITableViewStyle style = UITableViewStyle.Plain)
            : base(style, null, true)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var loadableViewModel = _viewModel as LoadableViewModel;
            if (loadableViewModel != null)
            {
                _refreshControl = new UIRefreshControl();
                RefreshControl = _refreshControl;
                _refreshControl.ValueChanged += (s, e) => loadableViewModel.LoadCommand.ExecuteIfCan();

                loadableViewModel.LoadCommand.IsExecuting.Subscribe(x =>
                {
                    if (x)
                    {
                        NetworkActivityService.PushNetworkActive();
                    }
                    else
                    {
                        NetworkActivityService.PopNetworkActive();
                        _refreshControl.EndRefreshing(); 
                    }
                });

                loadableViewModel.LoadCommand.ExecuteIfCan();
            }
        }
    }
}


using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.iOS.ViewControllers
{
    public abstract class ViewModelViewController<TViewModel> : UIViewController where TViewModel : ReactiveObject
    {
        private readonly TViewModel _viewModel = IoC.Resolve<TViewModel>();

        public TViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var loadableViewModel = _viewModel as LoadableViewModel;
            if (loadableViewModel != null)
                loadableViewModel.LoadCommand.Execute(null);
        }
    }
}


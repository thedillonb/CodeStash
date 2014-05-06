using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers
{
    public abstract class ViewModelDialogViewController<TViewModel> : DialogViewController where TViewModel : ReactiveObject
    {
        private readonly TViewModel _viewModel = IoC.Resolve<TViewModel>();

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
                loadableViewModel.LoadCommand.Execute(null);
        }
    }
}


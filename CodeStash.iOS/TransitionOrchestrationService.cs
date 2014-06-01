using System;
using ReactiveUI;
using MonoTouch.UIKit;
using Xamarin.Utilities.Core.Services;
using CodeStash.iOS.ViewControllers.Repositories;
using CodeStash.iOS.ViewControllers.Application;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.iOS
{
    public class TransitionOrchestrationService : ITransitionOrchestrationService
    {
        public void Transition(IViewFor fromView, IViewFor toView)
        {
            var fromViewController = (UIViewController)fromView;
            var toViewController = (UIViewController)toView;
            var toViewDismissCommand = ((BaseViewModel)toView.ViewModel).DismissCommand;

            //Root
            if (toViewController is SettingsViewController)
            {
                toViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.Cancel, UIBarButtonItemStyle.Plain, (s, e) => toViewDismissCommand.Execute(null));
                toViewDismissCommand.Subscribe(__ => toViewController.DismissViewController(true, null));
                fromViewController.PresentViewController(new UINavigationController(toViewController), true, null);
            }
            else if (toViewController is AccountsViewController)
            {
                var rootNav = (UINavigationController)UIApplication.SharedApplication.Delegate.Window.RootViewController;
                toViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.Cancel, UIBarButtonItemStyle.Plain, (s, e) => toViewDismissCommand.Execute(null));
                toViewDismissCommand.Subscribe(_ => toViewController.DismissViewController(true, null));
                rootNav.PresentViewController(new UINavigationController(toViewController), true, null);
            }
            else if (fromViewController is RepositoriesViewController)
                fromViewController.NavigationController.PresentViewController(toViewController, true, null);
            else if (toViewController is MainViewController)
            {
                var nav = ((UINavigationController)UIApplication.SharedApplication.Delegate.Window.RootViewController);
                UIView.Transition(nav.View, 0.1, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.TransitionCrossDissolve, 
                    () => nav.PushViewController(toViewController, false), null);
            }
            else if (toViewController is LoginViewController && fromViewController is StartupViewController)
            {
                toViewDismissCommand.Subscribe(_ => fromViewController.DismissViewController(true, null));
                fromViewController.PresentViewController(new UINavigationController(toViewController), true, null);
            }
            else
            {
                toViewDismissCommand.Subscribe(_ => toViewController.NavigationController.PopToViewController(fromViewController, true));
                fromViewController.NavigationController.PushViewController(toViewController, true);
            }
        }
    }
}


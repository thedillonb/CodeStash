using System;
using System.Reactive.Linq;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Dialog.Utilities;
using MonoTouch.SlideoutNavigation;
using CodeStash.iOS.ViewControllers.Projects;
using MonoTouch.Foundation;
using System.Drawing;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class StartupViewController : UIViewController, IImageUpdated
    {
        public readonly StartupViewModel ViewModel = IoC.Resolve<StartupViewModel>();

        const float ImageSize = 128f;

        private UIImageView _imgView;
        private UILabel _statusLabel;
        private UIActivityIndicatorView _activityView;
        private UIStatusBarStyle _previousStatusbarStyle;


        public StartupViewController()
        {
            ViewModel.GoToMainCommand.Subscribe(x =>
            {
                var slideout = new SimpleSlideoutNavigationController {MenuViewController = new UIViewController()};

                var c = new CustomMenuNavigationController(new ProjectsViewController(), slideout);
                slideout.MenuViewController.AddChildViewController(c);
                c.View.Frame = new RectangleF(0, 0, slideout.MenuViewController.View.Bounds.Width, slideout.MenuViewController.View.Bounds.Height - 44f);
                c.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                slideout.MenuViewController.View.Add(c.View);

                var toolbar = new UIToolbar(new RectangleF(0, slideout.MenuViewController.View.Bounds.Height - 44f, slideout.MenuViewController.View.Bounds.Width, 44f))
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin,
                    Items = new[]
                    {
                        new UIBarButtonItem(UIBarButtonSystemItem.Action,
                            (s, e) => PresentViewController(new SettingsViewController(), true, null))
                    }
                };
                slideout.MenuViewController.View.Add(toolbar);

                var mainNavigationController = new MainNavigationController(new WelcomeViewController(), slideout, new UIBarButtonItem(Images.MenuButton, UIBarButtonItemStyle.Plain, (s, e) => slideout.Open(true)));
                slideout.MainViewController = mainNavigationController;
                UIApplication.SharedApplication.Delegate.Window.RootViewController = slideout;
            });

            ViewModel.GoToAccountsCommand.Subscribe(_ =>
            {
                var ctrl = new AccountsViewController();
                ctrl.ViewModel.DismissCommand.Subscribe(__ => DismissViewController(true, null));
                PresentViewController(new UINavigationController(ctrl), true, null);
            });

            ViewModel.GoToNewUserCommand.Subscribe(_ =>
            {
                var ctrl = new LoginViewController();
                ctrl.ViewModel.DismissCommand.Subscribe(__ => DismissViewController(true, null));
                PresentViewController(new UINavigationController(ctrl), true, null);
            });
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            _imgView.Frame = new RectangleF(View.Bounds.Width / 2 - ImageSize / 2, View.Bounds.Height / 2 - ImageSize / 2 - 30f, ImageSize, ImageSize);
            _statusLabel.Frame = new RectangleF(0, _imgView.Frame.Bottom + 10f, View.Bounds.Width, 15f);
            _activityView.Center = new PointF(View.Bounds.Width / 2, _statusLabel.Frame.Bottom + 16f + 16F);

            try
            {
                //View.BackgroundColor = MonoTouch.Utilities.CreateRepeatingBackground();
            }
            catch (Exception e)
            {
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;

            _imgView = new UIImageView();
            _imgView.Layer.CornerRadius = ImageSize / 2;
            _imgView.Layer.MasksToBounds = true;
            Add(_imgView);

            _statusLabel = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("HelveticaNeue", 13f),
                TextColor = UIColor.FromWhiteAlpha(0.34f, 1f)
            };
            Add(_statusLabel);

            _activityView = new UIActivityIndicatorView
            {
                HidesWhenStopped = true,
                Color = UIColor.FromRGB(0.33f, 0.33f, 0.33f)
            };
            Add(_activityView);


            ViewModel.WhenAnyValue(x => x.Account).Where(x => x != null).Subscribe(x =>
            {
                UpdatedImage(null);
                _statusLabel.Text = "Logging in " + x.Username;
                _activityView.Hidden = false;
                _statusLabel.Hidden = false;
            });

            ViewModel.LoadCommand.IsExecuting.Subscribe(x =>
            {
                if (x)
                {
                    _activityView.StartAnimating();
                }
                else
                {
                    _activityView.StopAnimating();
                }
            });

        }

        public void UpdatedImage(Uri uri)
        {
            if (uri == null)
            {
                AssignUnknownUserImage();
            }
            else
            {
                var img = ImageLoader.DefaultRequestImage(uri, this);
                if (img == null)
                {
                    AssignUnknownUserImage();
                }
                else
                {
                    UIView.Transition(_imgView, 0.50f, UIViewAnimationOptions.TransitionCrossDissolve, () => _imgView.Image = img, null);
                }
            }
        }

        private void AssignUnknownUserImage()
        {
            var img = Images.LoginUserUnknown.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            _imgView.Image = img;
            _imgView.TintColor = UIColor.FromWhiteAlpha(0.34f, 1f);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.LoadCommand.Execute(null);
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.Default;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
            return UIInterfaceOrientationMask.All;
        }

        private class CustomMenuNavigationController : UINavigationController
        {
            private readonly SlideoutNavigationController _slideoutNavigationController;

            /// <summary>
            /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
            /// </summary>
            /// <param name="rootViewController">Root view controller.</param>
            /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
            public CustomMenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
                : base(rootViewController)
            {
                _slideoutNavigationController = slideoutNavigationController;
            }

            public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, NSAction completionHandler)
            {
                var openMenuButton = new UIBarButtonItem(Images.MenuButton, UIBarButtonItemStyle.Plain, (s, e) => _slideoutNavigationController.Open(true));
                var ctrl = new MainNavigationController(viewControllerToPresent, _slideoutNavigationController, openMenuButton);
                _slideoutNavigationController.SetMainViewController(ctrl, animated);
            }
        }

        /// <summary>
        /// A custom navigation controller specifically for iOS6 that locks the orientations to what the StartupControler's is.
        /// </summary>
        protected class CustomNavigationController : UINavigationController
        {
            readonly UIViewController _parent;
            public CustomNavigationController(UIViewController parent, UIViewController root) : base(root) 
            { 
                _parent = parent;
            }

            public override bool ShouldAutorotate()
            {
                return _parent.ShouldAutorotate();
            }

            public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
            {
                return _parent.GetSupportedInterfaceOrientations();
            }
        }
    }
}


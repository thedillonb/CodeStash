using System;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Dialog.Utilities;
using MonoTouch.SlideoutNavigation;
using CodeStash.iOS.ViewControllers.Projects;
using MonoTouch.Foundation;
using System.Drawing;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class StartupViewController : UIViewController, IImageUpdated
    {
        public readonly StartupViewModel ViewModel = IoC.Resolve<StartupViewModel>();

        const float imageSize = 128f;

        private UIImageView _imgView;
        private UILabel _statusLabel;
        private UIActivityIndicatorView _activityView;
        private UIStatusBarStyle _previousStatusbarStyle;


        public StartupViewController()
        {
            ViewModel.GoToMainCommand.Subscribe(x =>
            {
                var slideout = new SimpleSlideoutNavigationController();
                slideout.MenuViewController = new UIViewController();


                var c = new CustomMenuNavigationController(new ProjectsViewController(), slideout);
                slideout.MenuViewController.AddChildViewController(c);
                c.View.Frame = new RectangleF(0, 0, slideout.MenuViewController.View.Bounds.Width, slideout.MenuViewController.View.Bounds.Height - 44f);
                c.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                slideout.MenuViewController.View.Add(c.View);

                var toolbar = new UIToolbar(new RectangleF(0, slideout.MenuViewController.View.Bounds.Height - 44f, slideout.MenuViewController.View.Bounds.Width, 44f));
                toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
                toolbar.Items = new UIBarButtonItem[] 
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => PresentViewController(new SettingsViewController(), true, null))
                };
                slideout.MenuViewController.View.Add(toolbar);

                var mainNavigationController = new MainNavigationController(new WelcomeViewController(), slideout, new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => slideout.Open(true)));
                slideout.MainViewController = mainNavigationController;
                UIApplication.SharedApplication.Delegate.Window.RootViewController = slideout;
            });
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            _imgView.Frame = new System.Drawing.RectangleF(View.Bounds.Width / 2 - imageSize / 2, View.Bounds.Height / 2 - imageSize / 2 - 30f, imageSize, imageSize);
            _statusLabel.Frame = new System.Drawing.RectangleF(0, _imgView.Frame.Bottom + 10f, View.Bounds.Width, 15f);
            _activityView.Center = new System.Drawing.PointF(View.Bounds.Width / 2, _statusLabel.Frame.Bottom + 16f + 16F);

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
            _imgView.Layer.CornerRadius = imageSize / 2;
            _imgView.Layer.MasksToBounds = true;
            Add(_imgView);

            _statusLabel = new UILabel();
            _statusLabel.TextAlignment = UITextAlignment.Center;
            _statusLabel.Font = UIFont.FromName("HelveticaNeue", 13f);
            _statusLabel.TextColor = UIColor.FromWhiteAlpha(0.34f, 1f);
            Add(_statusLabel);

            _activityView = new UIActivityIndicatorView() { HidesWhenStopped = true };
            _activityView.Color = UIColor.FromRGB(0.33f, 0.33f, 0.33f);
            Add(_activityView);

//            var vm = (BaseStartupViewModel)ViewModel;
//            vm.Bind(x => x.IsLoggingIn, x =>
//            {
//                if (x)
//                {
//                    _activityView.StartAnimating();
//                }
//                else
//                {
//                    _activityView.StopAnimating();
//                }
//            });
//
//            vm.Bind(x => x.ImageUrl, UpdatedImage);
//            vm.Bind(x => x.Status, x => _statusLabel.Text = x);

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
//            var img = Theme.CurrentTheme.LoginUserUnknown.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
//            _imgView.Image = img;
//            _imgView.TintColor = UIColor.FromWhiteAlpha(0.34f, 1f);
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _previousStatusbarStyle = UIApplication.SharedApplication.StatusBarStyle;
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UIApplication.SharedApplication.SetStatusBarStyle(_previousStatusbarStyle, true);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.GoToMainCommand.Execute(null);
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
            /// The view controller transform.
            /// </summary>
            public Func<UIViewController, UIViewController> ViewControllerTransform;

            /// <summary>
            /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
            /// </summary>
            /// <param name="rootViewController">Root view controller.</param>
            /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
            public CustomMenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
                : base(rootViewController)
            {
                _slideoutNavigationController = slideoutNavigationController;
                ViewControllerTransform = x => new MainNavigationController(x, slideoutNavigationController);
            }

            public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, NSAction completionHandler)
            {
                _slideoutNavigationController.SetMainViewController(ViewControllerTransform(viewControllerToPresent), animated);
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


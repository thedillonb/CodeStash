using System;
using System.Drawing;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using Xamarin.Utilities.ViewControllers;

namespace CodeStash.iOS.ViewControllers.Application
{
    public partial class LoginViewController : ViewModelViewController<LoginViewModel>
    {
        protected readonly IStatusIndicatorService StatusIndicatorService = IoC.Resolve<IStatusIndicatorService>();

        public LoginViewController()
            : base("AddAccountView", null)
        {
            Title = "Login";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            ViewModel.WhenAnyValue(x => x.Username).Subscribe(x => User.Text = x);
//            ViewModel.WhenAnyValue(x => x.Password).Subscribe(x => Password.Text = x);
//            ViewModel.WhenAnyValue(x => x.Domain).Subscribe(x => Domain.Text = x);

            User.EditingChanged += (sender, args) => 
                ViewModel.Username = User.Text;
            Password.EditingChanged += (sender, args) => 
                ViewModel.Password = Password.Text;
            Domain.EditingChanged += (sender, args) => 
                ViewModel.Domain = Domain.Text;

            LoginButton.TouchUpInside += (sender, args) => ViewModel.LoginCommand.Execute(null);

            ViewModel.LoginCommand.IsExecuting.Subscribe(x => 
			{
				if (x)
                    StatusIndicatorService.Show("Logging in...");
				else
                    StatusIndicatorService.Hide();
			});

			View.BackgroundColor = UIColor.FromRGB(239, 239, 244);
            Logo.Image = Images.StashLogo;

            LoginButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            LoginButton.SetBackgroundImage(Images.GreyButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);

            //Set some generic shadowing
            LoginButton.Layer.ShadowColor = UIColor.Black.CGColor;
            LoginButton.Layer.ShadowOffset = new SizeF(0, 1);
            LoginButton.Layer.ShadowOpacity = 0.3f;

            Domain.ShouldReturn = delegate {
                User.BecomeFirstResponder();
                return true;
            };

            User.ShouldReturn = delegate {
                Password.BecomeFirstResponder();
                return true;
            };

            Password.ShouldReturn = delegate {
                Password.ResignFirstResponder();
                LoginButton.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                return true;
            };


            ScrollView.ContentSize = new SizeF(View.Frame.Width, LoginButton.Frame.Bottom + 10f);
        }

        NSObject _hideNotification, _showNotification;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            _showNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_hideNotification);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_showNotification);
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            if (IsViewLoaded) {

                //Check if the keyboard is becoming visible
                bool visible = notification.Name == UIKeyboard.WillShowNotification;

                //Start an animation, using values from the keyboard
                UIView.BeginAnimations ("AnimateForKeyboard");
                UIView.SetAnimationBeginsFromCurrentState (true);
                UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
                UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

                //Pass the notification, calculating keyboard height, etc.
                bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                if (visible) {
                    var keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);

                    OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
                } else {
                    var keyboardFrame = UIKeyboard.FrameBeginFromNotification (notification);

                    OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
                }

                //Commit the animation
                UIView.CommitAnimations (); 
            }
        }

        /// <summary>
        /// Override this method to apply custom logic when the keyboard is shown/hidden
        /// </summary>
        /// <param name='visible'>
        /// If the keyboard is visible
        /// </param>
        /// <param name='keyboardHeight'>
        /// Calculated height of the keyboard (width not generally needed here)
        /// </param>
        protected virtual void OnKeyboardChanged (bool visible, float keyboardHeight)
        {
            if (visible)
            {
                var frame = ScrollView.Frame;
                frame.Height -= keyboardHeight;
                ScrollView.Frame = frame;
            }
            else
            {
                var frame = ScrollView.Frame;
                frame.Height = View.Bounds.Height;
                ScrollView.Frame = frame;
            }
        }
    }
}


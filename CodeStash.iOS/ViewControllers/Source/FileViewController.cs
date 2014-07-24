using System;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Source;
using ReactiveUI;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using Xamarin.Utilities.ViewControllers;
using CodeFramework.iOS.SourceBrowser;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FileViewController : ViewModelViewController<FileViewModel>
    {
        private UIWebView _webView;

        public override void ViewDidLoad()
        {
            Title = ViewModel.FileName;

            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _webView = new UIWebView();
            _webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _webView.Frame = new System.Drawing.RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
            View.Add(_webView);

            ViewModel.WhenAnyValue(x => x.Content).Skip(1).Subscribe(x =>
            {
                var model = new SourceBrowserModel { Content = x };
                var view = new SyntaxHighlighterView { Model = model };
                _webView.LoadHtmlString(view.GenerateString(), NSBundle.MainBundle.BundleUrl);
            });
        }
    }
}


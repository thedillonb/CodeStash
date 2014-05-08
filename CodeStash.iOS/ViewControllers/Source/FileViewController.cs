using System;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Source;
using ReactiveUI;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FileViewController : UIViewController
    {
        public readonly FileViewModel ViewModel = IoC.Resolve<FileViewModel>();
        private UIWebView _webView;

        public FileViewController(string projectKey, string repositorySlug, string branch, string path)
        {
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            ViewModel.Path = path;
            ViewModel.Branch = branch;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _webView = new UIWebView();
            _webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _webView.Frame = new System.Drawing.RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
            View.Add(_webView);

            var path = System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, "Highlighter", "highlight.html");
            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            _webView.LoadRequest(new NSUrlRequest(new NSUrl(uri)));

            ViewModel.WhenAnyValue(x => x.Content).Skip(1).Subscribe(x =>
            {
                var json = IoC.Resolve<IJsonSerializationService>().Serialize(new { text = x });
                _webView.EvaluateJavascript("render(" + json + ".text);");
            });

            ViewModel.LoadCommand.Execute(null);
        }
    }
}


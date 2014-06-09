using System;
using Xamarin.Utilities.ViewControllers;
using CodeStash.Core.ViewModels.Commits;
using ReactiveUI;
using MonoTouch.Foundation;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitDiffViewController : WebViewController, IViewFor<CommitDiffViewModel>
    {
        public CommitDiffViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CommitDiffViewModel)value; }
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.Name;

            base.ViewDidLoad();

//            var path = System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, "Diff", "diffindex.html");
//            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
//            Web.LoadRequest(new NSUrlRequest(new NSUrl(uri)));
//
//            ViewModel.LoadCommand.IsExecuting.Skip(1).Where(x => !x).Subscribe(x =>
//            {
//                if (string.IsNullOrEmpty(ViewModel.NewContent) && string.IsNullOrEmpty(ViewModel.OldContent))
//                    return;
//
//                var newContent = JavaScriptStringEncode(ViewModel.NewContent);
//                var oldContent = JavaScriptStringEncode(ViewModel.OldContent);
//                ExecuteJavascript("var a = \"" + newContent + "\"; var b = \"" + oldContent + "\"; diff(b, a);");
//            });

            ViewModel.WhenAnyValue(x => x.Diff).Where(x => x != null).Subscribe(x =>
            {
                var template = new DiffRazorView () { Model = x };
                var page = template.GenerateString ();
                Web.LoadHtmlString(page, NSBundle.MainBundle.BundleUrl);
            });

            ViewModel.LoadCommand.ExecuteIfCan();
        }
    }
}


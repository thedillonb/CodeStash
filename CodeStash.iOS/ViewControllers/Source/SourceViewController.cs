using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class SourceViewController : ViewModelDialogViewController<SourceViewModel>
    {
        public SourceViewController()
            : base(style: UITableViewStyle.Plain)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var selector = new UISegmentedControl(new object[] { "Branches", "Tags" });
            ViewModel.WhenAnyValue(x => x.SelectedView).Subscribe(x => selector.SelectedSegment = x);
            selector.ValueChanged += (sender, e) => ViewModel.SelectedView = selector.SelectedSegment;
            NavigationItem.TitleView = selector;

            var sec = new Section();
            Root.Reset(sec);

            var branchIcon = Images.Branch.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var tagIcon = Images.Tag.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            ViewModel.Branches.Changed.Where(_ => ViewModel.SelectedView == 0).Subscribe(_ =>
            sec.Reset(ViewModel.Branches.Select(x =>
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = branchIcon;
                return element;
            })));

            ViewModel.Tags.Changed.Where(_ => ViewModel.SelectedView == 1).Subscribe(_ =>
            sec.Reset(ViewModel.Tags.Select(x =>
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x));
                    element.Image = tagIcon;
                return element;
            })));
        }
    }
}


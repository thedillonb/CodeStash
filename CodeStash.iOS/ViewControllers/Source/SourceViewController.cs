using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using MonoTouch.Dialog;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class SourceViewController : ViewModelDialogViewController<SourceViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var selector = new UISegmentedControl(new object[] { "Branches", "Tags" });
            ViewModel.WhenAnyValue(x => x.SelectedView).Subscribe(x => selector.SelectedSegment = x);
            selector.ValueChanged += (sender, e) => ViewModel.SelectedView = selector.SelectedSegment;
            NavigationItem.TitleView = selector;

            var sec = new Section();
            Root = new RootElement("Source") { sec };

            ViewModel.Branches.Changed.Where(_ => ViewModel.SelectedView == 0).Subscribe(_ =>
            sec.Reset(ViewModel.Branches.Select(x =>
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = Images.Branch;
                return element;
            })));

            ViewModel.Tags.Changed.Where(_ => ViewModel.SelectedView == 1).Subscribe(_ =>
            sec.Reset(ViewModel.Tags.Select(x =>
            {
                var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = Images.Tag;
                return element;
            })));
        }
    }
}


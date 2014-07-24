using System;
using CodeStash.Core.ViewModels.Source;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FilesViewController : ViewModelCollectionViewController<FilesViewModel>
    {
        public override void ViewDidLoad()
        {
            Title = ViewModel.Folder;

            this.WhenActivated(d =>
            {
                d(SearchTextChanging.Subscribe(x => ViewModel.SearchKeyword = x));
            });

            base.ViewDidLoad();

            var fileIcon = Images.File.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            var folderIcon = Images.Folder.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            this.BindList(ViewModel.Contents, x =>
            {
                var element = new StyledStringElement(x.Path.ToString, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase) ? fileIcon : folderIcon;
                return element;
            });
        }
    }
}


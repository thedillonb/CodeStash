using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.Dialog;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FilesViewController : ViewModelCollectionView<FilesViewModel>
    {
        public override void ViewDidLoad()
        {
            Title = ViewModel.Folder;

            base.ViewDidLoad();

            var fileIcon = Images.File.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            var folderIcon = Images.Folder.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            Bind(ViewModel.WhenAnyValue(x => x.Contents), x =>
            {
                var element = new StyledStringElement(x.Path.ToString, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase) ? fileIcon : folderIcon;
                return element;
            });
        }
    }
}


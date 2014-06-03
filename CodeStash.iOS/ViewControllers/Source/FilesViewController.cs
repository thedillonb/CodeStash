using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.Dialog;
using System.Linq;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FilesViewController : ViewModelDialogViewController<FilesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement(ViewModel.Folder) { sec };
            var fileIcon = Images.File.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);
            var folderIcon = Images.Folder.ImageWithRenderingMode(MonoTouch.UIKit.UIImageRenderingMode.AlwaysTemplate);

            ViewModel.Contents.Changed.Subscribe(_ => sec.Reset(ViewModel.Contents.Select(x =>
            {
                var element = new StyledStringElement(x.Path.ToString, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase) ? fileIcon : folderIcon;
                return element;
            })));
        }
    }
}


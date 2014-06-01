using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.Dialog;
using System.Linq;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FilesViewController : ViewModelDialogViewController<FilesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement(ViewModel.Folder) { sec };

            ViewModel.Contents.Changed.Subscribe(_ => sec.Reset(ViewModel.Contents.Select(x =>
            {
                var element = new StyledStringElement(x.Path.ToString, () => ViewModel.GoToSourceCommand.Execute(x));
                element.Image = string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase) ? Images.File : Images.Folder;
                return element;
            })));
        }
    }
}


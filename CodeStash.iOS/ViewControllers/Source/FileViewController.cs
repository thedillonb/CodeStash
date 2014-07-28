using System;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Source;
using CodeFramework.iOS.Views.Source;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FileViewController : FileSourceView<FileViewModel>
    {
        private UIActionSheet _actionSheet;

        public override void ViewDidLoad()
        {
            Title = ViewModel.FileName;

            base.ViewDidLoad();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowMenu());
        }

        private void ShowMenu()
        {
            _actionSheet = new UIActionSheet();
            _actionSheet.Title = Title;
            var changeTheme = _actionSheet.AddButton("Change Theme");
            _actionSheet.CancelButtonIndex = _actionSheet.AddButton("Cancel");
            _actionSheet.Clicked += (sender, e) => 
            {
                if (e.ButtonIndex == changeTheme)
                    ShowThemePicker();
                _actionSheet = null;
            };

            _actionSheet.ShowInView(View);
        }
    }
}


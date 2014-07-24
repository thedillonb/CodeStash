using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Build;
using CodeFramework.iOS.Views;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.Build
{
    public class BuildStatusesViewController : ViewModelCollectionViewController<BuildStatusesViewModel>
    {
        public BuildStatusesViewController()
            : base(searchbarEnabled: false)
        {
        }

        public override void ViewDidLoad()
        {
            Title = "Build Status";
            var okIcon = Images.BuildOk.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var failedIcon = Images.Error.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var loadingIcon = Images.Update.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            this.BindList(ViewModel.BuildStatues, x =>
            {
                var element = new StyledStringElement(x.Name, string.Empty, UITableViewCellStyle.Subtitle);

                if (string.Equals(x.State, "SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
                {
                    element.Image = okIcon;
                    element.Value = "Successful";
                }
                else if (string.Equals(x.State, "FAILED", StringComparison.OrdinalIgnoreCase))
                {
                    element.Image = failedIcon;
                    element.Value = "Failed";
                }
                else
                {
                    element.Image = loadingIcon;
                    element.Value = "In Progress";
                }

                element.Value += string.Format(" - {0}", AtlassianStashSharp.Helpers.UnixDateTimeHelper.FromUnixTime(x.DateAdded).ToDaysAgo());

                element.Tapped += () => ViewModel.GoToBuildStatusCommand.Execute(x);
                return element;
            });

            base.ViewDidLoad();

        }
    }
}


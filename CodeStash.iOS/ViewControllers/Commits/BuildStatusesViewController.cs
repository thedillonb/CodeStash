using System;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.UIKit;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class BuildStatusesViewController : ViewModelDialogViewController<BuildStatusesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement("Build Status") { sec };

            ViewModel.BuildStatues.Changed.Subscribe(_ => sec.Reset(ViewModel.BuildStatues.Select(x =>
            {
                var element = new StyledStringElement(x.Name, string.Empty, UITableViewCellStyle.Subtitle);

                if (string.Equals(x.State, "SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
                {
                    element.Image = Images.BuildOk;
                    element.Value = "Successful";
                }
                else if (string.Equals(x.State, "FAILED", StringComparison.OrdinalIgnoreCase))
                {
                    element.Image = Images.Error;
                    element.Value = "Failed";
                }
                else
                {
                    element.Image = Images.Update;
                    element.Value = "In Progress";
                }

                element.Value += string.Format(" - {0}", AtlassianStashSharp.Helpers.UnixDateTimeHelper.FromUnixTime(x.DateAdded).ToDaysAgo());

                element.Tapped += () => ViewModel.GoToBuildStatusCommand.Execute(x);
                return element;
            })));
        }
    }
}


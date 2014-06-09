using System;
using ReactiveUI;
using CodeStash.Core.ViewModels.PullRequests;
using CodeStash.iOS.Views;
using MonoTouch.UIKit;
using System.Reactive.Linq;
using System.Linq;
using MonoTouch.Dialog;
using CodeFramework.iOS.Views;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestViewController : ViewModelDialogView<PullRequestViewModel>
    {
        public PullRequestViewController()
            : base(UITableViewStyle.Grouped)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.CreateTopBackground(UIColor.GroupTableViewBackgroundColor);
            var header = new ImageAndTitleHeaderView 
            { 
                BackgroundColor = UIColor.GroupTableViewBackgroundColor,
                Image = Images.Avatar
            };
            TableView.TableHeaderView = header;

            var root = new RootElement(string.Format("Pull Request #{0}", ViewModel.PullRequestId)) { UnevenRows = true };

            var description = new StyledMultilineElement(string.Empty);
            description.Font = UIFont.SystemFontOfSize(12f);

            var statusElement = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Status.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "Open"),
                Button2 = new SplitElement.SplitButton(Images.Group.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Participants"),
            };

            var buildElement = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Build.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Builds", () => ViewModel.GoToBuildStatusCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Comment.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Comments", () => ViewModel.GoToCommentsCommand.ExecuteIfCan()),
            };
            root.Add(new Section { statusElement, buildElement });

            var changesElement = new StyledStringElement("Changes", () => ViewModel.GoToChangesCommand.ExecuteIfCan(), Images.File.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
            var commitsElement = new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.ExecuteIfCan(), Images.Commit.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
            root.Add(new Section { changesElement, commitsElement });

            Root = root;

            ViewModel.WhenAnyValue(x => x.PullRequest).Where(x => x != null).Subscribe(x =>
            {
                header.Text = x.Title;
                TableView.TableHeaderView = header;
                statusElement.Button1.Text = x.State;

                description.Caption = x.Description;
                if (description.GetImmediateRootElement() == null)
                    Root[0].Insert(0, UITableViewRowAnimation.Fade, description);

                var selfLink = x.Author.User.Links["self"].FirstOrDefault();
                if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    header.ImageUri = selfLink.Href + "/avatar.png";
            });

            ViewModel.Participants.Changed.Subscribe(_ =>
            {
                statusElement.Button2.Text = string.Format("{0} Participant{1}", ViewModel.Participants.Count, ViewModel.Participants.Count > 1 ? "s" : string.Empty);
            });

            ViewModel.WhenAnyValue(x => x.BuildStatus).Where(x => x != null && x.Length > 0).Subscribe(x =>
            {
                var first = x.FirstOrDefault();
                if (string.Equals(first.State, "SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
                    buildElement.Button1.Image = Images.BuildOk.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else if (string.Equals(first.State, "FAILED", StringComparison.OrdinalIgnoreCase))
                    buildElement.Button1.Image = Images.Error.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else
                    buildElement.Button1.Image = Images.Update.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

                buildElement.Button1.Text = string.Format("{0} Build{1}", x.Length, x.Length == 1 ? string.Empty : "s");
            });
        }
    }
}


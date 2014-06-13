using System;
using ReactiveUI;
using CodeStash.Core.ViewModels.PullRequests;
using CodeStash.iOS.Views;
using MonoTouch.UIKit;
using System.Reactive.Linq;
using System.Linq;
using MonoTouch.Dialog;
using CodeFramework.iOS.Views;
using CodeFramework.iOS.Elements;
using Xamarin.Utilities.Core.Services;
using MonoTouch.Foundation;

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

            Root = new RootElement(string.Format("Pull Request #{0}", ViewModel.PullRequestId)) { UnevenRows = true };

            var description = new StyledMultilineElement(string.Empty);
            description.Font = UIFont.SystemFontOfSize(12f);

            var statusElement = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Status.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "Open") { Enabled = false },
                Button2 = new SplitElement.SplitButton(Images.Group.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Participants", () => ViewModel.GoToParticipantsCommand.ExecuteIfCan()),
            };

            var buildElement = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Build.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Builds", () => ViewModel.GoToBuildStatusCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Comment.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Comments", () => ViewModel.GoToCommentsCommand.ExecuteIfCan()),
            };
            Root.Add(new Section { statusElement, buildElement });

            var changesElement = new StyledStringElement("Changes", () => ViewModel.GoToChangesCommand.ExecuteIfCan(), Images.File.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
            var commitsElement = new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.ExecuteIfCan(), Images.Commit.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
            Root.Add(new Section { changesElement, commitsElement });

            var mergeElement = new StyledStringElement("Merge", string.Empty);
            mergeElement.Image = Images.Merge;

            var commentsElement = new WebElement("comments");
            var commentsSection = new Section() { commentsElement };

            var header = new ImageAndTitleHeaderView 
            { 
                BackgroundColor = UIColor.GroupTableViewBackgroundColor,
                Image = Images.Avatar,
                Text = " "
            };
            TableView.TableHeaderView = header;
            TableView.SectionFooterHeight = 0.3f;

            ViewModel.GoToCommentsCommand.Subscribe(_ =>
            {
                if (commentsElement.GetImmediateRootElement() != null)
                    TableView.ScrollToRow(commentsElement.IndexPath, UITableViewScrollPosition.Middle, true);
            });

            ViewModel.WhenAnyValue(x => x.PullRequest).Where(x => x != null).Subscribe(x =>
            {
                header.Text = x.Title;
                statusElement.Button1.Text = x.State;
                description.Caption = x.Description;
                if (description.GetImmediateRootElement() == null)
                    Root[0].Insert(0, UITableViewRowAnimation.Fade, description);

                var selfLink = x.Author.User.Links["self"].FirstOrDefault();
                if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    header.ImageUri = selfLink.Href + "/avatar.png";
                TableView.TableHeaderView = header;
            });

            ViewModel.Participants.Changed.Subscribe(_ =>
            {
                statusElement.Button2.Text = string.Format("{0} Participant{1}", ViewModel.Participants.Count, ViewModel.Participants.Count != 1 ? "s" : string.Empty);
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

            ViewModel.Activities.Changed.Subscribe(_ =>
            {
                var commentCount = ViewModel.Activities.Where(x => x.Comment != null).Sum(x => CommentCount(x.Comment));
                buildElement.Button2.Text = string.Format("{0} Comment{1}", commentCount, commentCount != 1 ? "s" : string.Empty);

                if (ViewModel.Activities.Count > 0)
                {
                    if (commentsSection.GetImmediateRootElement() == null)
                        Root.Add(commentsSection);

                    var template = new CommentCellView { Model = ViewModel.Activities.ToList() };
                    commentsElement.Value = template.GenerateString();
                }
                else
                {
                    if (commentsSection.GetImmediateRootElement() != null)
                        Root.Remove(commentsSection, UITableViewRowAnimation.Fade);
                }
            });
        }

        private static int CommentCount(AtlassianStashSharp.Models.Comment comment)
        {
            var comments = 0;
            if (comment.Comments != null)
            {
                foreach (var c in comment.Comments)
                    comments += CommentCount(c);
            }

            return 1 + comments;
        }
    }
}


using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using CodeStash.iOS.Views;
using System.Reactive.Linq;
using ReactiveUI;
using MonoTouch.UIKit;
using System.Collections.Generic;
using CodeFramework.iOS.Views;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitViewController : ViewModelDialogView<CommitViewModel>
    {
        public CommitViewController()
            : base(UITableViewStyle.Grouped)
        {
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.Title;

            base.ViewDidLoad();

            Root = new RootElement(Title) { UnevenRows = true };

            var splitElement1 = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Commit.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "-", () => ViewModel.GoToParentCommitCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Branch.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "-", () => ViewModel.GoToBranchesCommand.ExecuteIfCan()),
            };
            var splitElement2 = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Build.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Builds", () => ViewModel.GoToBuildStatusCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Comment.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), "0 Comments", () => ViewModel.GoToCommentsCommand.ExecuteIfCan()),
            };

            Root.Add(new Section() { splitElement1, splitElement2 });

            var header = new ImageAndTitleHeaderView
            {
//                EnableSeperator = true,
//                SeperatorColor = TableView.SeparatorColor
                BackgroundColor = UIColor.GroupTableViewBackgroundColor,
                Image = Images.Avatar,
                Text = ViewModel.RepositorySlug
            };

            TableView.TableHeaderView = header;
            TableView.SectionFooterHeight = 0.3f;

            ViewModel.WhenAnyValue(x => x.Commit).Where(x => x != null).Subscribe(x =>
            {
                header.Text = x.Message;
                TableView.TableHeaderView = header;
                var firstParent = x.Parents.FirstOrDefault();
                if (firstParent != null)
                    splitElement1.Button1.Text = firstParent.DisplayId;
                else
                    splitElement1.Button1.Text = "No Parent";
            });

            ViewModel.WhenAnyValue(x => x.BuildStatus).Where(x => x != null && x.Length > 0).Subscribe(x =>
            {
                var first = x.FirstOrDefault();
                if (string.Equals(first.State, "SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
                    splitElement2.Button1.Image = Images.BuildOk.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else if (string.Equals(first.State, "FAILED", StringComparison.OrdinalIgnoreCase))
                    splitElement2.Button1.Image = Images.Error.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else
                    splitElement2.Button1.Image = Images.Update.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

                splitElement2.Button1.Text = string.Format("{0} Build{1}", x.Length, x.Length == 1 ? string.Empty : "s");
            });

            ViewModel.Branches.Changed.Subscribe(_ =>
            {
                if (ViewModel.Branches.Count > 1)
                    splitElement1.Button2.Text = string.Format("{0} Branches", ViewModel.Branches.Count);
                else
                {
                    var firstBranch = ViewModel.Branches.FirstOrDefault();
                    if (firstBranch != null)
                        splitElement1.Button2.Text = firstBranch.DisplayId;
                }
            });

            var fileIcon = Images.File.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            ViewModel.Changes.Changed.Subscribe(_ =>
            {
                var sections = new List<Section>();
                sections.Add(new Section() { splitElement1, splitElement2 });
                foreach (var @group in ViewModel.Changes.GroupBy(x => x.Path.Parent))
                {
                    var sec = new Section("/" + @group.Key);
                    foreach (var entry in @group)
                    {
                        var entryClosed = entry;
                        var element = new StyledStringElement(entry.Path.Name, () => ViewModel.GoToDiffCommand.ExecuteIfCan(entryClosed));
                        element.Image = fileIcon;
                        sec.Add(element);
                    }
                    sections.Add(sec);
                }

                Root.Reset(sections);
            });
        }
    }
}


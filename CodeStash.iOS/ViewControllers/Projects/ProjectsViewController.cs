using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Projects;
using CodeFramework.iOS.Views;
using ReactiveUI;
using System.Drawing;
using System.Linq;
using System;

namespace CodeStash.iOS.ViewControllers.Projects
{
    public class ProjectsViewController : ViewModelCollectionView<ProjectsViewModel>
    {
        public ProjectsViewController()
        {
            Title = "Projects";
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            TableView.SeparatorInset = new UIEdgeInsets(0, 44f, 0, 0);
            Bind(ViewModel.WhenAnyValue(x => x.Projects), x =>
            {
                var el = new ProjectElement(x.Name, x.Description);
                el.Image = Images.ProjectAvatar;

                if (x.Links != null)
                {
                    var selfLink = x.Links["self"].FirstOrDefault();
                    if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    {
                        Uri avatarUri;
                        if (Uri.TryCreate(selfLink.Href + "/avatar.png", UriKind.Absolute, out avatarUri))
                            el.ImageUri = avatarUri;
                    }
                }

                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToProjectCommand.Execute(x);
                return el;
            });

            base.ViewDidLoad();
        }


        private class ProjectElement : StyledStringElement
        {
            public ProjectElement(string name, string description)
                : base(name, description, UITableViewCellStyle.Subtitle)
            {
            }

            protected override UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
            {
                return new ProjectTableViewCell(style, key);
            }

            private class ProjectTableViewCell : UITableViewCell
            {
                public ProjectTableViewCell(UITableViewCellStyle style, string key)
                    : base(style, key)
                {
                }

                public override void LayoutSubviews()
                {
                    base.LayoutSubviews();

                    if (ImageView != null)
                    {
                        ImageView.Frame = new RectangleF(6, 6, 32f, 32f); 
                        ImageView.Layer.MasksToBounds = true;
                        ImageView.Layer.CornerRadius = ImageView.Frame.Height / 2;
                    }

                    if (TextLabel != null)
                    {
                        var textLabelPos = new PointF(ImageView.Frame.Right + 6f, TextLabel.Frame.Y);
                        TextLabel.Frame = new RectangleF(textLabelPos, new SizeF(ContentView.Bounds.Width - textLabelPos.X, TextLabel.Frame.Height));
                    }

                    if (DetailTextLabel != null)
                    {
                        var detailLabelPos = new PointF(ImageView.Frame.Right + 6f, DetailTextLabel.Frame.Y);
                        DetailTextLabel.Frame = new RectangleF(detailLabelPos, new SizeF(ContentView.Bounds.Width - detailLabelPos.X, DetailTextLabel.Frame.Height));
                    }
                }
            }
        }
    }
}


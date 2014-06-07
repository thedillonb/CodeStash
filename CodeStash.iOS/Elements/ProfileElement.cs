using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;
using System.Drawing;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeStash.iOS.Elements
{
    public class ProfileElement : Element, IImageUpdated
    {
        private readonly string _title, _subtitle;

        public UITableViewCellAccessory Accessory;

        private UIImage _image;
        public UIImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                var cell = GetActiveCell() as ProfileTableViewCell;
                if (cell != null)
                {
                    cell.ImageView.Image = _image;
                    cell.ImageView.SetNeedsDisplay();
                }
            }
        }

        public string ImageUri
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                var img = ImageLoader.DefaultRequestImage(new Uri(value), this);
                if (img != null)
                    Image = img;
            }
        }

        public event Action Tapped;

        public object Tag { get; set; }

        public ProfileElement(string title, string subtitle)
            : base(string.Empty)
        {
            _title = title;
            _subtitle = subtitle;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell("profile_element") as ProfileTableViewCell;
            if (cell == null)
                cell = new ProfileTableViewCell();

            cell.Accessory = Accessory;
            cell.ImageView.Image = Image;
            cell.TitleLabel.Text = _title;
            cell.SubtitleLabel.Text = _subtitle;
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
        {
            var handler = Tapped;
            if (handler != null)
                handler();
            tableView.DeselectRow(path, true);
        }

        public void UpdatedImage(Uri uri)
        {
            var cell = this.GetActiveCell() as ProfileTableViewCell;
            if (cell != null)
                cell.ImageView.Image = ImageLoader.DefaultRequestImage(uri, this);
        }

        public class ProfileTableViewCell : ReactiveUI.Cocoa.ReactiveTableViewCell, IViewFor<CodeStash.Core.Data.Account>
        {
            public new UIImageView ImageView;
            public UILabel TitleLabel;
            public UILabel SubtitleLabel;
            private CodeStash.Core.Data.Account _account;

 
            public CodeStash.Core.Data.Account ViewModel
            {
                get { return _account; }
                set { this.RaiseAndSetIfChanged(ref _account, value); }
            }

            object IViewFor.ViewModel
            {
                get { return ViewModel; }
                set { ViewModel = (CodeStash.Core.Data.Account)value; }
            }

            public ProfileTableViewCell()
            {
                Initialize();
            }

            public ProfileTableViewCell(IntPtr handle)
                : base(handle)
            {
                Initialize();
            }

            private void Initialize()
            {
                ImageView = new UIImageView();
                ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                ImageView.Layer.MinificationFilter = MonoTouch.CoreAnimation.CALayer.FilterTrilinear;
                ImageView.Layer.MasksToBounds = true;

                TitleLabel = new UILabel();
                TitleLabel.TextColor = UIColor.FromWhiteAlpha(0.0f, 1f);
                TitleLabel.Font = UIFont.FromName("HelveticaNeue", 17f);

                SubtitleLabel = new UILabel();
                SubtitleLabel.TextColor = UIColor.FromWhiteAlpha(0.1f, 1f);
                SubtitleLabel.Font = UIFont.FromName("HelveticaNeue-Thin", 14f);

                ContentView.Add(ImageView);
                ContentView.Add(TitleLabel);
                ContentView.Add(SubtitleLabel);

                this.WhenAnyValue(x => x.ViewModel).Where(x => x != null).Subscribe(x =>
                {
                    //ImageView.Image = ImageLoader.DefaultRequestImage(x.AvatarUrl, this);
                    TitleLabel.Text = x.Username;
                    SubtitleLabel.Text = x.Domain;
                });
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var imageSize = this.Bounds.Height - 30f;
                ImageView.Layer.CornerRadius = imageSize / 2;
                ImageView.Frame = new RectangleF(15, 15, imageSize, imageSize);

                var titlePoint = new PointF(ImageView.Frame.Right + 15f, 19f);
                TitleLabel.Frame = new RectangleF(titlePoint.X, titlePoint.Y, this.ContentView.Bounds.Width - titlePoint.X - 10f, TitleLabel.Font.LineHeight);
                SubtitleLabel.Frame = new RectangleF(titlePoint.X, TitleLabel.Frame.Bottom, this.ContentView.Bounds.Width - titlePoint.X - 10f, SubtitleLabel.Font.LineHeight + 1);
            }
        }
    }
}


using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeStash.iOS.Views
{
    public class RepositoryHeaderView : UIView
    {
        public readonly UIImageView ImageView;
        public readonly UILabel NameLabel;

        public RepositoryHeaderView()
            : base(new RectangleF(0, 0, 320f, 150f))
        {
            ImageView = new UIImageView();
            ImageView.Frame = new RectangleF(0, 0, 80, 80);
            ImageView.Center = new PointF(Bounds.Width / 2, 15 + ImageView.Frame.Height / 2);
            ImageView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            ImageView.Layer.MasksToBounds = true;
            ImageView.Layer.CornerRadius = ImageView.Frame.Width / 2f;
            Add(ImageView);

            NameLabel = new UILabel();
            NameLabel.Frame = new RectangleF(0, ImageView.Frame.Bottom + 5f, Bounds.Width, 16);
            NameLabel.TextAlignment = UITextAlignment.Center;
            NameLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            Add(NameLabel);
        }
    }
}


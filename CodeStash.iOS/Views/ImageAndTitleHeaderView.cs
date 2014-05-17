using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog.Utilities;

namespace CodeStash.iOS.Views
{
    public class ImageAndTitleHeaderView : UIView, IImageUpdated
    {
        private readonly UIImageView _imageView;
        private readonly UILabel _label;

        public string ImageUri
        {
            set
            {
                var img = ImageLoader.DefaultRequestImage(new Uri(value), this);
                if (img != null)
                    _imageView.Image = img;
            }
        }

        public UIImage Image
        {
            get { return _imageView.Image; }
            set { _imageView.Image = value; }
        }

        public string Text
        {
            get { return _label.Text; }
            set 
            { 
                _label.Text = value; 
                this.SetNeedsLayout();
                this.LayoutIfNeeded();
            }
        }

        public ImageAndTitleHeaderView()
            : base(new RectangleF(0, 0, 320f, 100f))
        {
            _imageView = new UIImageView();
            _imageView.Frame = new RectangleF(0, 0, 80, 80);
            _imageView.Layer.MasksToBounds = true;
            _imageView.Layer.CornerRadius = _imageView.Frame.Width / 2f;
            Add(_imageView);

            _label = new UILabel();
            _label.TextAlignment = UITextAlignment.Center;
            _label.Lines = 0;
            Add(_label);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _imageView.Center = new PointF(Bounds.Width / 2, 15 + _imageView.Frame.Height / 2);

            _label.Frame = new RectangleF(20, _imageView.Frame.Bottom + 10f, Bounds.Width - 40, Bounds.Height - (_imageView.Frame.Bottom + 5f));
            _label.SizeToFit();
            _label.Frame = new RectangleF(20, _imageView.Frame.Bottom + 10f, Bounds.Width - 40, _label.Frame.Height);

            var f = Frame;
            f.Height = _label.Frame.Bottom + 10f;
            Frame = f;
        }

        public void UpdatedImage(Uri uri)
        {
            _imageView.Image = ImageLoader.DefaultRequestImage(uri, this);
        }
    }
}


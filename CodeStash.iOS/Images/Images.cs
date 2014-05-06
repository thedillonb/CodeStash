using System;
using MonoTouch.UIKit;

namespace CodeStash.iOS
{
    public static class Images
    {
        public static UIImage Folder { get { return UIImage.FromBundle("Images/folder"); } }
        public static UIImage File { get { return UIImage.FromBundle("Images/file"); } }
        public static UIImage Tag { get { return UIImage.FromBundle("Images/tag"); } }
    }
}


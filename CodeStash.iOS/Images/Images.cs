using System;
using MonoTouch.UIKit;

namespace CodeStash.iOS
{
    public static class Images
    {
        public static UIImage Folder { get { return UIImage.FromBundle("Images/folder"); } }
        public static UIImage File { get { return UIImage.FromBundle("Images/file"); } }
        public static UIImage Gear { get { return UIImage.FromBundle("Images/gear"); } }
        public static UIImage Tag { get { return UIImage.FromBundle("Images/tag"); } }
        public static UIImage Branch { get { return UIImage.FromBundle("Images/branch"); } }

        public static UIImage GreyButton { get { return UIImageHelper.FromFileAuto("Images/grey_button"); } }
        public static UIImage StashLogo { get { return UIImage.FromFile("Images/logo.png"); } }

        public static UIImage Build { get { return UIImageHelper.FromFileAuto("Images/build"); } }
        public static UIImage Update { get { return UIImageHelper.FromFileAuto("Images/update"); } }
        public static UIImage BuildOk { get { return UIImageHelper.FromFileAuto("Images/build_ok"); } }
        public static UIImage Error { get { return UIImageHelper.FromFileAuto("Images/error"); } }
        public static UIImage Commit { get { return UIImageHelper.FromFileAuto("Images/commit"); } }
        public static UIImage Comment { get { return UIImageHelper.FromFileAuto("Images/comment"); } }

        public static UIImage Megaphone { get { return UIImageHelper.FromFileAuto("Images/megaphone"); } }
        public static UIImage Info { get { return UIImageHelper.FromFileAuto("Images/info"); } }
        public static UIImage Merge { get { return UIImageHelper.FromFileAuto("Images/merge"); } }
        public static UIImage Readme { get { return UIImageHelper.FromFileAuto("Images/readme"); } }

    }
}


using System;
using MonoTouch.UIKit;

namespace CodeStash.iOS
{
    public static class Images
    {
        public static UIImage Folder { get { return UIImage.FromBundle("Images/folder"); } }
        public static UIImage File { get { return UIImage.FromBundle("Images/file"); } }
        public static UIImage Tag { get { return UIImage.FromBundle("Images/tag"); } }

        public static UIImage LoginUserUnknown { get { return UIImageHelper.FromFileAuto("Images/login_user_unknown"); } }
        public static UIImage GreyButton { get { return UIImageHelper.FromFileAuto("Images/grey_button"); } }
        public static UIImage StashLogo { get { return UIImage.FromFile("/Images/logo.png"); } }

    }
}


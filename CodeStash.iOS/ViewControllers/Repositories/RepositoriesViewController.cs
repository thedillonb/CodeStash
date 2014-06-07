using System;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;
using CodeStash.iOS.Views;
using ReactiveUI;
using System.Reactive.Linq;
using MonoTouch.UIKit;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : BaseRepositoriesViewController<RepositoriesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.CreateTopBackground(UIColor.GroupTableViewBackgroundColor);
            var header = new ImageAndTitleHeaderView
            { 
                EnableSeperator = true, 
                SeperatorColor = TableView.SeparatorColor,
                Image = Images.LoginUserUnknown, 
                BackgroundColor = UIColor.GroupTableViewBackgroundColor,
            };
            TableView.TableHeaderView = header;

            ViewModel.WhenAnyValue(x => x.Project).Where(x => x != null).Subscribe(x =>
            {
                header.Text = x.Description;
                TableView.TableHeaderView = header;
                var selfLink = x.Links["self"].FirstOrDefault();
                if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    header.ImageUri = selfLink.Href + "/avatar.png";
            });
        }
    }
}


using System;
using CodeStash.Core.ViewModels.Repositories;
using ReactiveUI;
using System.Reactive.Linq;
using MonoTouch.UIKit;
using Xamarin.Utilities.Views;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : BaseRepositoriesViewController<RepositoriesViewModel>
    {
        public RepositoriesViewController()
            : base(false)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.CreateTopBackground(UIColor.GroupTableViewBackgroundColor);
            var header = new ImageAndTitleHeaderView
            { 
                EnableSeperator = true, 
                SeperatorColor = TableView.SeparatorColor,
                Image = Images.ProjectAvatar, 
                BackgroundColor = UIColor.GroupTableViewBackgroundColor,
            };
            TableView.TableHeaderView = header;

            ViewModel.WhenAnyValue(x => x.DisplayName).Where(x => x != null).Subscribe(x => 
            {
                header.Text = x;
                TableView.TableHeaderView = header;
            });

            ViewModel.WhenAnyValue(x => x.ImageUrl).Where(x => x != null).Subscribe(x =>
            {
                    header.ImageUri = x;
            });
        }
    }
}


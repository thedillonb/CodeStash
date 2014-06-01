using System;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;
using CodeStash.iOS.Views;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : BaseRepositoriesViewController<RepositoriesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.WhenAnyValue(x => x.Project).Where(x => x != null).Subscribe(x =>
            {
                var header = new ImageAndTitleHeaderView
                { 
                    EnableSeperator = true, 
                    SeperatorColor = TableView.SeparatorColor,
                    Image = Images.LoginUserUnknown, 
                    Text = x.Description 
                };

                var selfLink = x.Links["self"].FirstOrDefault();
                if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                    header.ImageUri = selfLink.Href + "/avatar.png";

                TableView.TableHeaderView = header;
            });
        }
    }
}


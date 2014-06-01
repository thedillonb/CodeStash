using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;
using CodeStash.iOS.Views;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : ViewModelDialogViewController<RepositoriesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.SeparatorInset = UIEdgeInsets.Zero;

            var sec = new Section();
            Root = new RootElement(ViewModel.Name) { sec };


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

            ViewModel.Repositories.Changed.Subscribe(_ => sec.Reset(ViewModel.Repositories.Select(x =>
            {
                var el = new StyledStringElement(x.Name);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                return el;
            })));
        }
    }
}


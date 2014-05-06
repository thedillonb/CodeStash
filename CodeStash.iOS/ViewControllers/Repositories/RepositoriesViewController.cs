using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Repositories;
using System.Linq;
using System.Reactive.Linq;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Repositories
{
    public class RepositoriesViewController : ViewModelDialogViewController<RepositoriesViewModel>
    {
        public RepositoriesViewController()
        {
            ViewModel.Repositories.Changed.Subscribe(_ =>
            {
                var root = new RootElement(Title);
                var sec = new Section();
                sec.AddAll(ViewModel.Repositories.Select(x => 
                {
                    var el = new StyledStringElement(x.Name);
                    el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    el.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(x);
                    return el;
                }));
                root.Add(sec);
                Root = root;
            });

            ViewModel.GoToRepositoryCommand.OfType<Repository>().Subscribe(x =>
            {
                var ctrl = new RepositoryViewController();
                ctrl.ViewModel.ProjectKey = x.Project.Key;
                ctrl.ViewModel.RepositorySlug = x.Slug;
                NavigationController.PresentViewController(ctrl, true, null);
            });
        }
    }
}


using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Projects;
using System.Linq;
using CodeStash.iOS.ViewControllers.Repositories;
using AtlassianStashSharp.Models;
using System.Reactive.Linq;

namespace CodeStash.iOS.ViewControllers.Projects
{
    public class ProjectsViewController : ViewModelDialogViewController<ProjectsViewModel>
    {
        public ProjectsViewController()
        {
            Title = "Projects";

            ViewModel.Projects.Changed.Subscribe(_ =>
            {
                var root = new RootElement(Title);
                var sec = new Section();
                sec.AddAll(ViewModel.Projects.Select(x => 
                {
                    var el = new StyledStringElement(x.Key, x.Description, UITableViewCellStyle.Subtitle);
                    el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    el.Tapped += () => ViewModel.GoToProjectCommand.Execute(x);
                    return el;
                }));
                root.Add(sec);
                Root = root;
            });

            ViewModel.GoToProjectCommand.OfType<Project>().Subscribe(x =>
            {
                var ctrl = new RepositoriesViewController { Title = x.Name };
                ctrl.ViewModel.ProjectKey = x.Key;
                NavigationController.PushViewController(ctrl, true);
            });
        }
    }
}


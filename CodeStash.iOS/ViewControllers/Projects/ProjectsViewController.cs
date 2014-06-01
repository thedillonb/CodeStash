using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Projects;
using System.Linq;
using CodeStash.iOS.ViewControllers.Repositories;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Repositories;

namespace CodeStash.iOS.ViewControllers.Projects
{
    public class ProjectsViewController : ViewModelDialogViewController<ProjectsViewModel>
    {
        public ProjectsViewController()
        {
            Title = "Projects";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement(Title) { sec };

            ViewModel.Projects.Changed.Subscribe(_ => sec.Reset(ViewModel.Projects.Select(x =>
            {
                var el = new StyledStringElement(x.Name, x.Description, UITableViewCellStyle.Subtitle);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToProjectCommand.Execute(x);
                return el;
            })));
        }
    }
}


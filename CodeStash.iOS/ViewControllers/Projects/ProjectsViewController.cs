using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Projects;
using CodeFramework.iOS.Views;
using ReactiveUI;

namespace CodeStash.iOS.ViewControllers.Projects
{
    public class ProjectsViewController : ViewModelCollectionView<ProjectsViewModel>
    {
        public ProjectsViewController()
        {
            Title = "Projects";
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            Bind(ViewModel.WhenAnyValue(x => x.Projects), x =>
            {
                var el = new StyledStringElement(x.Name, x.Description, UITableViewCellStyle.Subtitle);
                el.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => ViewModel.GoToProjectCommand.Execute(x);
                return el;
            });

            base.ViewDidLoad();
        }
    }
}


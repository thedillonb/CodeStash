using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.Dialog;
using System.Linq;
using System.Reactive.Linq;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class FilesViewController : ViewModelDialogViewController<FilesViewModel>
    {
        public FilesViewController(string projectKey, string repositorySlug, string branch, string path = null)
        {
            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            ViewModel.Branch = branch;
            ViewModel.Path = path;

            ViewModel.Contents.Changed.Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Contents.Select(x => 
                {
                    var element = new StyledStringElement(x.Path.ToString, () => ViewModel.GoToSourceCommand.Execute(x));
                    element.Image = string.Equals(x.Type, "FILE", StringComparison.OrdinalIgnoreCase) ? Images.File : Images.Folder;
                    return element;
                }));
                Root = new RootElement(Title) { sec};
            });

            ViewModel.GoToSourceCommand.OfType<Content>().Subscribe(x => NavigationController.PushViewController(
                new FilesViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, ViewModel.Branch, ViewModel.Path + "/" + x.Path.Name), true));
        }
    }
}


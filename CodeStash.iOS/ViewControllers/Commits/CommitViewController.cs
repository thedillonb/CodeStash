using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitViewController : ViewModelDialogViewController<CommitViewModel>
    {
        public CommitViewController(string projectKey, string repositorySlug, string node)
        {
            if (string.IsNullOrEmpty(node) || node.Length < 7)
                Title = "Commit";
            else
                Title = node.Substring(0, 7);

            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            ViewModel.Node = node;

            ViewModel.Changes.Changed.Subscribe(_ =>
            {
                var root = new RootElement(Title);

                foreach (var @group in ViewModel.Changes.GroupBy(x => x.Path.Parent))
                {
                    var sec = new Section("/" + @group.Key);
                    foreach (var entry in @group)
                    {
                        var element = new StyledStringElement(entry.Path.Name, () => {});
                        element.Image = Images.File;
                        sec.Add(element);
                    }

                    root.Add(sec);
                }

                Root = root;
            });
        }
    }
}


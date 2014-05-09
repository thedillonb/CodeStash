using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using CodeStash.iOS.Views;
using System.Reactive.Linq;
using ReactiveUI;
using MonoTouch.UIKit;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitViewController : ViewModelDialogViewController<CommitViewModel>
    {
        private ImageAndTitleHeaderView _header;

        public CommitViewController(string projectKey, string repositorySlug, string node)
        {
            if (string.IsNullOrEmpty(node) || node.Length < 7)
                Title = "Commit";
            else
                Title = node.Substring(0, 7);

            ViewModel.ProjectKey = projectKey;
            ViewModel.RepositorySlug = repositorySlug;
            ViewModel.Node = node;

            _header = new ImageAndTitleHeaderView();
            _header.Image = Images.LoginUserUnknown;
            _header.Text = repositorySlug;


            ViewModel.WhenAnyValue(x => x.Commit).Where(x => x != null).Subscribe(x =>
            {
                _header.Text = x.Message;
                TableView.TableHeaderView = _header;
            });

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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.TableHeaderView = _header;
        }
    }
}


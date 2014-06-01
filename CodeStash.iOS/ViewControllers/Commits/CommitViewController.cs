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

        public override void ViewDidLoad()
        {
            Title = ViewModel.Title;

            base.ViewDidLoad();

            _header = new ImageAndTitleHeaderView();
            _header.Image = Images.LoginUserUnknown;
            _header.Text = ViewModel.RepositorySlug;
            TableView.TableHeaderView = _header;

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
    }
}


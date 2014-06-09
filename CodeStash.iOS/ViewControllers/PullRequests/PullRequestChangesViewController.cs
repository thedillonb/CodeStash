using System;
using CodeStash.Core.ViewModels.PullRequests;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using CodeFramework.iOS.Views;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestChangesViewController : ViewModelCollectionView<PullRequestChangesViewModel>
    {
        public PullRequestChangesViewController()
        {
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Root = new RootElement("Changes");
            var fileIcon = Images.File.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            ViewModel.Changes.Changed.Subscribe(_ =>
            {
                var sections = new List<Section>();
                foreach (var @group in ViewModel.Changes.GroupBy(x => x.Path.Parent))
                {
                    var sec = new Section("/" + @group.Key);
                    foreach (var entry in @group)
                    {
                        var entryClosed = entry;
                        var element = new StyledStringElement(entry.Path.Name, () => ViewModel.GoToDiffCommand.ExecuteIfCan(entryClosed));
                        element.Image = fileIcon;
                        sec.Add(element);
                    }
                    sections.Add(sec);
                }

                Root.Reset(sections);
            });
        }
    }
}


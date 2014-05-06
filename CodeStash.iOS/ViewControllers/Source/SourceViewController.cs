﻿using System;
using CodeStash.Core.ViewModels.Source;
using MonoTouch.UIKit;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using MonoTouch.Dialog;
using AtlassianStashSharp.Models;

namespace CodeStash.iOS.ViewControllers.Source
{
    public class SourceViewController : ViewModelDialogViewController<SourceViewModel>
    {
        public SourceViewController()
        {
            ViewModel.Branches.Changed.Where(_ => ViewModel.SelectedView == 0).Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Branches.Select(x => new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x))));
                Root = new RootElement(Title) { sec};
            });

            ViewModel.Tags.Changed.Where(_ => ViewModel.SelectedView == 1).Subscribe(_ =>
            {
                var sec = new Section();
                sec.AddAll(ViewModel.Tags.Select(x => 
                {
                    var element = new StyledStringElement(x.DisplayId, () => ViewModel.GoToSourceCommand.Execute(x));
                    element.Image = Images.Tag;
                    return element;
                }));
                Root = new RootElement(Title) { sec};
            });

            ViewModel.GoToSourceCommand.OfType<Tag>().Subscribe(x => NavigationController.PushViewController(
                new FilesViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, x.LatestChangeset), true));

            ViewModel.GoToSourceCommand.OfType<Branch>().Subscribe(x => NavigationController.PushViewController(
                new FilesViewController(ViewModel.ProjectKey, ViewModel.RepositorySlug, x.LatestChangeset), true));
        }

        public override void ViewDidLoad()
        {
            Title = "Source";

            base.ViewDidLoad();

            var selector = new UISegmentedControl(new object[] { "Branches", "Tags" });
            ViewModel.WhenAnyValue(x => x.SelectedView).Subscribe(x => selector.SelectedSegment = x);
            selector.ValueChanged += (sender, e) => ViewModel.SelectedView = selector.SelectedSegment;
            NavigationItem.TitleView = selector;
        }
    }
}


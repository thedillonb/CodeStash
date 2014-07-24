using System;
using CodeStash.Core.ViewModels.PullRequests;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;
using Xamarin.Utilities.DialogElements;

namespace CodeStash.iOS.ViewControllers.PullRequests
{
    public class PullRequestChangesViewController : ViewModelCollectionViewController<PullRequestChangesViewModel>
    {
        public PullRequestChangesViewController()
            : base(searchbarEnabled: false)
        {
        }

        public override void ViewDidLoad()
        {
            Title = "Changes";

            base.ViewDidLoad();

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
                        var element = new StyledStringElement(entry.Path.Name, FirstCharToUpper(entry.Type), UITableViewCellStyle.Subtitle);
                        element.Tapped += () => ViewModel.GoToDiffCommand.ExecuteIfCan(entryClosed);
                        element.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                        element.Image = fileIcon;
                        sec.Add(element);
                    }
                    sections.Add(sec);
                }

                Root.Reset(sections);
            });
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;
            input = input.ToLower();
            return input[0].ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }
}


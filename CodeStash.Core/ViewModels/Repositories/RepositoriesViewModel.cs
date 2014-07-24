using AtlassianStashSharp.Models;
using ReactiveUI;
using CodeStash.Core.Services;
using Xamarin.Utilities.Core.ViewModels;
using System;
using System.Linq;

namespace CodeStash.Core.ViewModels.Repositories
{
    public class RepositoriesViewModel : BaseRepositoriesViewModel, ILoadableViewModel
    {
        public string ProjectKey { get; set; }

        public bool Personal { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            private set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            private set { this.RaiseAndSetIfChanged(ref _imageUrl, value); }
        }

        public IReactiveCommand LoadCommand { get; private set; }

        public RepositoriesViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (Personal)
                {
                    DisplayName = Name + "'s Personal Repositories";
                }
                else
                {
                    var project = (await applicationService.StashClient.Projects[ProjectKey].Get().ExecuteAsync()).Data;
                    if (string.Equals(project.Type, "personal", StringComparison.OrdinalIgnoreCase))
                        DisplayName = Name + "'s Personal Repositories";
                    else
                        DisplayName = project.Description;

                    var selfLink = project.Links["self"].FirstOrDefault();
                    if (selfLink != null && !string.IsNullOrEmpty(selfLink.Href))
                        ImageUrl = selfLink.Href + "/avatar.png";
                }

                var d = await applicationService.StashClient.Projects[ProjectKey].Repositories.GetAll().ExecuteAsync();
                InternalRepositories.Reset(d.Data.Values);
            });
        }
    }
}


using System;
using ReactiveUI;
using CodeStash.Core.Services;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Repositories;
using Xamarin.Utilities.Core.ReactiveAddons;
using AtlassianStashSharp.Helpers;
using CodeFramework.Core.Services;
using CodeFramework.Core.Data;

namespace CodeStash.Core.ViewModels.Projects
{
    public class ProjectsViewModel  : BaseViewModel, ILoadableViewModel
    {
        public ReactiveCollection<Project> Projects { get; private set; }

        public IAccount Account { get; private set; }

        public IReactiveCommand<object> GoToProjectCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public ProjectsViewModel(IApplicationService applicationService, IAccountsService accountsService)
        {
            Account = accountsService.ActiveAccount;
            GoToProjectCommand = ReactiveCommand.Create();
            Projects = new ReactiveCollection<Project>(new [] { CreatePersonalProject(accountsService.ActiveAccount) });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async x =>
            {
                var getAllProjects = applicationService.StashClient.Projects.GetAll();

                using (Projects.SuppressChangeNotifications())
                {
                    Projects.Clear();
                    Projects.Add(CreatePersonalProject(accountsService.ActiveAccount));
                    Projects.AddRange(await getAllProjects.ExecuteAsyncAll());
                }
            });

            GoToProjectCommand.OfType<Project>().Subscribe(x =>
            {
                var vm = this.CreateViewModel<RepositoriesViewModel>();
                vm.ProjectKey = x.Key;
                vm.Name = x.Name;
                vm.Personal = x.Type == null;
                ShowViewModel(vm);
            });
        }

        private static Project CreatePersonalProject(IAccount account)
        {
            return new Project()
            {
                Id = 0,
                Key = "~" + account.Username,
                Description = account.Username + "'s Personal Repositories",
                Name = account.Username
            };
        }
    }
}


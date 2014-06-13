using CodeFramework.Core.ViewModels.Application;
using ReactiveUI;
using System.Reactive;
using System;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.Core
{
    public static class Bootstrap
    {
        public static void Init()
        {
            RxApp.DefaultExceptionHandler = Observer.Create((Exception e) =>
            {
                IoC.Resolve<IAlertDialogService>().Alert("Error", e.Message);
                Console.WriteLine("Exception occured: " + e.Message + " at " + e.StackTrace);
            });

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            IoC.RegisterAsInstance<IAddAccountViewModel, CodeStash.Core.ViewModels.Application.LoginViewModel>();
            IoC.RegisterAsInstance<IMainViewModel, CodeStash.Core.ViewModels.Application.MainViewModel>();
        }
    }
}


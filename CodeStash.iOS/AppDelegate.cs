using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeStash.iOS.ViewControllers.Application;
using System.Reactive.Concurrency;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using System;
using System.Reactive;
using System.Threading;

namespace CodeStash.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public override UIWindow Window
        {
            get;
            set;
        }

        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            System.Console.WriteLine("UI Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            RxApp.MainThreadScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
            RxApp.DefaultExceptionHandler = Observer.Create((Exception e) =>
            {
                IoC.Resolve<IAlertDialogService>().Alert("Unhandled Exception", e.Message);
                Console.WriteLine("Exception occured: " + e.Message + " at " + e.StackTrace);
            });

            // Load the IoC
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Core.Services.IDefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Services.DefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Core.Services.IApplicationService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(GetType().Assembly);

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new StartupViewController();
            Window.MakeKeyAndVisible();
            return true;
        }
    }
}


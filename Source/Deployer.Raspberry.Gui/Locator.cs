using System;
using System.Reactive.Subjects;
using Deployer.Gui.Common;
using Deployer.Gui.Common.Services;
using Deployer.Lumia.NetFx;
using Deployer.Raspberry.Gui.Specifics;
using Deployer.Raspberry.Gui.ViewModels;
using Deployer.Raspberry.Gui.Views;
using Deployer.Tasks;
using Grace.DependencyInjection;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using Serilog.Events;

namespace Deployer.Raspberry.Gui
{
    public class Locator
    {
        private readonly DependencyInjectionContainer container;
        
        public Locator()
        {
            container = new DependencyInjectionContainer();

            IObservable<LogEvent> logEvents = null;

            IViewService viewService = new ViewService();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(@"Logs\Log-{Date}.txt")
                .WriteTo.Observers(x => logEvents = x)
                .MinimumLevel.Verbose()
                .CreateLogger();

            var optionsProvider = new WindowsDeploymentOptionsProvider();

            container.Configure(x =>
            {
                x.Configure(optionsProvider);
                x.Export<DeviceProvider>().As<IDeviceProvider>().Lifestyle.Singleton();
                x.Export<WpfMarkdownDisplayer>().As<IMarkdownDisplayer>();
                x.ExportFactory(() => new BehaviorSubject<double>(double.NaN))
                    .As<IObserver<double>>()
                    .As<IObservable<double>>()
                    .Lifestyle.Singleton();
                x.ExportFactory(() => logEvents).As<IObservable<LogEvent>>();
                x.Export<WimPickViewModel>().ByInterfaces().As<WimPickViewModel>().Lifestyle.Singleton();
                x.Export<AdvancedViewModel>().ByInterfaces().As<AdvancedViewModel>().Lifestyle.Singleton();
                x.Export<DeploymentViewModel>().ByInterfaces().As<DeploymentViewModel>().Lifestyle.Singleton();
                x.Export<UIServices>();
                x.ExportFactory(() => viewService).As<IViewService>();
                x.Export<Dialog>().ByInterfaces();
                x.Export<FilePicker>().As<IFilePicker>();
                x.Export<SettingsService>().As<ISettingsService>();
                x.ExportFactory(() => DialogCoordinator.Instance).As<IDialogCoordinator>();
            });
        }

        public MainViewModel MainViewModel => container.Locate<MainViewModel>();

        public WimPickViewModel WimPickViewModel => container.Locate<WimPickViewModel>();

        public DeploymentViewModel DeploymentViewModel => container.Locate<DeploymentViewModel>();

        public AdvancedViewModel AdvancedViewModel => container.Locate<AdvancedViewModel>();
    }
}
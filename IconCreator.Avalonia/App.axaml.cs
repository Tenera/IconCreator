using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using IconCreator.Avalonia.ViewModels;
using IconCreator.Avalonia.Views;

namespace IconCreator.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var view = new MainWindow();
            desktop.MainWindow = view;
            view.DataContext = new MainViewModel(TopLevel.GetTopLevel(desktop.MainWindow));
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var view = new MainView();
            singleViewPlatform.MainView = view;
            view.DataContext = new MainViewModel(TopLevel.GetTopLevel(singleViewPlatform.MainView));
        }

        base.OnFrameworkInitializationCompleted();
    }
}

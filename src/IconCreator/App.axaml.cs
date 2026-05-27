using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using IconCreator.ViewModels;
using IconCreator.Views;

namespace IconCreator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var view = new MainWindow();
            desktop.MainWindow = view;
            view.DataContext = new MainViewModel(view);
        }
        base.OnFrameworkInitializationCompleted();
    }
}

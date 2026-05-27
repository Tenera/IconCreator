using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var view = new MainWindow();
            desktop.MainWindow = view;
            view.DataContext = new MainViewModel(TopLevel.GetTopLevel(desktop.MainWindow));
        }
        base.OnFrameworkInitializationCompleted();
    }
}

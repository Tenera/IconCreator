using SukiUI.Controls;
using SukiUI.Toasts;

namespace IconCreator.Views;

public partial class MainWindow : SukiWindow
{
    public static ISukiToastManager ToastManager { get; } = new SukiToastManager();

    public MainWindow()
    {
        InitializeComponent();
        ToastHost.Manager = ToastManager;
    }
}

using System.Windows;
using TaskManagerWpf.App.Services;
using TaskManagerWpf.App.ViewModels;
using TaskManagerWpf.App.Views;

namespace TaskManagerWpf.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var store = new JsonTaskStore();
        var messageService = new MessageBoxMessageService();
        var dialogService = new DialogService();

        var mainVm = new MainViewModel(store, dialogService, messageService);
        var mainWindow = new MainWindow
        {
            DataContext = mainVm
        };

        MainWindow = mainWindow;
        mainWindow.Show();

        await mainVm.InitializeAsync();
    }
}


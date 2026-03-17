using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
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
        ConfigureRussianCulture();
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

    private static void ConfigureRussianCulture()
    {
        var culture = new CultureInfo("ru-RU");

        // Ensure a consistent global date format for parsing/formatting.
        culture.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
        culture.DateTimeFormat.LongDatePattern = "dd.MM.yyyy";

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
    }
}


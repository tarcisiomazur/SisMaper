using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Tools.Events;

namespace SisMaper.Tools;

public static class Helper
{
    public static void SystemDefaultMessage(object? sender, ShowMessageEventArgs e)
    {
        e.SystemResult = MessageBox.Show(e.Title, e.Message, e.SystemStyle);
    }

    public static void MahAppsDefaultMessage(object? sender, ShowMessageEventArgs e)
    {
        e.Result = DialogCoordinator.Instance.ShowModalMessageExternal(sender, e.Title, e.Message, e.Style, e.Settings);
    }

    public static async Task MahAppsDefaultProgress(object? sender, ShowProgressEventArgs e)
    {
        e.DialogController = await DialogCoordinator.Instance.ShowProgressAsync(sender, "", "", false, e.Settings);
    }

    public static void MahAppsDefaultInput(object? sender, ShowInputEventArgs e)
    {
        e.Result = DialogCoordinator.Instance.ShowModalInputExternal(sender, e.Title, e.Message, e.Settings);
    }
}
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Tools.Events;

namespace SisMaper.Tools
{
    public static class Helper
    {
        public static void MahAppsDefaultMessage(object? sender, ShowMessageEventArgs e)
        {
            e.Result = DialogCoordinator.Instance.ShowModalMessageExternal(sender, e.Title, e.Message, e.Style, e.Settings);
        }

        public static async void MahAppsDefaultProgress(object? sender, ShowProgressEventArgs e)
        {
            e.DialogController = await DialogCoordinator.Instance.ShowProgressAsync(sender, null, null, false, e.Settings);
        }
        
        public static void MahAppsDefaultInput(object? sender, ShowInputEventArgs e)
        {
            e.Result = DialogCoordinator.Instance.ShowModalInputExternal(sender, e.Title, e.Message, e.Settings);
        }
    }
}
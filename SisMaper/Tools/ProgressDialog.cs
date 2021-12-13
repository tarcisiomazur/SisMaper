using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools;

public static class ProgressDialogHelper
{
    public static async Task SetMessageDelayed(this ProgressDialogController controller, string message, int delay)
    {
        await Task.Delay(delay);
        if (controller.IsOpen) controller.SetMessage(message);
    }

    public static async Task SetCancelableDelayed(this ProgressDialogController controller, bool value, int delay)
    {
        await Task.Delay(delay);
        if (controller.IsOpen) controller.SetCancelable(value);
    }

    public static void TryCloseAsync(this ProgressDialogController controller)
    {
        try
        {
            if (controller.IsOpen) controller.CloseAsync().Wait();
        }
        catch
        {
            //
        }
    }
}
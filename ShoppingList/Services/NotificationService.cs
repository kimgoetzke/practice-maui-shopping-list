using CommunityToolkit.Maui.Alerts;

namespace ShoppingList.Services;

public static class NotificationService
{
    public static async Task ShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make(message);
        await toast.Show(cancellationTokenSource.Token);
    }
}
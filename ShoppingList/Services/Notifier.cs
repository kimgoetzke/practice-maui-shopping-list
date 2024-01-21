using CommunityToolkit.Maui.Alerts;

namespace ShoppingList.Services;

public static class Notifier
{
    public static async Task AwaitShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make(message);
        await toast.Show(cancellationTokenSource.Token);
    }

    public static void ShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make(message);
        toast.Show(cancellationTokenSource.Token);
    }
}
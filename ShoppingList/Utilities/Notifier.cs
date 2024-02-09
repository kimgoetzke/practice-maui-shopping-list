using AsyncAwaitBestPractices;
using CommunityToolkit.Maui.Alerts;

namespace ShoppingList.Utilities;

public static class Notifier
{
    public static void ShowToast(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make(message);
        toast.Show(cancellationTokenSource.Token).SafeFireAndForget();
    }
}

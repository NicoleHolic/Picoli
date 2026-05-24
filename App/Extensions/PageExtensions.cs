using CommunityToolkit.Mvvm.ComponentModel;

namespace App.Extensions;

public static class PageExtensions
{
    public static async ValueTask<T> Handle<T>(this Page page, Func<ValueTask<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            await page.DisplayAlertAsync("Error", e.Message, "OK");
            return default(T);
        }
    }
    
    public static async Task Handle(this Page page, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            await page.DisplayAlertAsync("Error", e.Message, "OK");
        }
    }
    
    public static async Task Handle(this ObservableObject observableObject, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlertAsync("Error", e.Message, "OK");
        }
    }
}
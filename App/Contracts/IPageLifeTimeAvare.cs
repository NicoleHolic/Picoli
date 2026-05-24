namespace App.Contracts;

public interface IPageLifeTimeAware
{
    public void OnAppearing() {}
    public void OnDisappearing() {}
}
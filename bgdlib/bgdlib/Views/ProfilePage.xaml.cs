namespace bgdlib.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(bgdlib.ViewModels.ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((bgdlib.ViewModels.ProfileViewModel)BindingContext).LoadCommand.ExecuteAsync(null);
    }
}

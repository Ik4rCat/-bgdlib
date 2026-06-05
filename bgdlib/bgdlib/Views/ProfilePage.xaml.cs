using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((ProfileViewModel)BindingContext).LoadCommand.ExecuteAsync(null);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // TODO: Firebase Auth Google Sign-In
        await DisplayAlert("Скоро", "Авторизация через Google будет добавлена в следующей версии", "OK");
    }
}

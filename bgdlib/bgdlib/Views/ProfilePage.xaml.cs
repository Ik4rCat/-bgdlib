using bgdlib.Services;
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
        var L = LocalizationService.Instance;
        await DisplayAlert("Google Sign-In", L["Profile_GuestNote"], L["OK"]);
    }
}

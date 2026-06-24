using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    private async void OnJobsTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(JobsPage));

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.RebuildLocalized();
        await _vm.LoadAsync();
    }
}

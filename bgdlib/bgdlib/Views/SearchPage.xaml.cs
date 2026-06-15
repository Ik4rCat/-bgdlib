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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.RebuildLocalized();
        await _vm.LoadAsync();
    }
}

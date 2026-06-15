using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class MainRibbonPage : ContentPage
{
    private readonly MainRibbonViewModel _vm;

    public MainRibbonPage(MainRibbonViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_vm.Posts.Any())
            await _vm.LoadAsync();
    }
}

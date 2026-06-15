using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class JobsPage : ContentPage
{
    private readonly JobsViewModel _vm;

    public JobsPage(JobsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

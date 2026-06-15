using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class DocsPage : ContentPage
{
    private readonly DocsViewModel _vm;

    public DocsPage(DocsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        vm.Init();
    }
}

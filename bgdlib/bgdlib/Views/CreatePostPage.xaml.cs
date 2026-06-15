using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class CreatePostPage : ContentPage
{
    public CreatePostPage(CreatePostViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

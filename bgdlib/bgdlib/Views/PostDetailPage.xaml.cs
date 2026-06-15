using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

[QueryProperty(nameof(Post), "Post")]
public partial class PostDetailPage : ContentPage
{
    private readonly PostDetailViewModel _vm;

    public CommunityPost? Post
    {
        set { if (value != null) _vm.SetPost(value); }
    }

    public PostDetailPage(PostDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommentsAsync();
    }
}

using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

[QueryProperty(nameof(Post), "Post")]
public partial class PostDetailPage : ContentPage
{
    private readonly PostDetailViewModel _vm;

    public PostDetailPage(PostDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public CommunityPost? Post
    {
        set { if (value != null) _vm.SetPost(value); }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommentsAsync();
    }

    // Tap handlers for per-comment voting (can't bind directly inside DataTemplate)
    private void OnUpvoteTapped(object sender, TappedEventArgs e)
    {
        if (((Element)sender).BindingContext is FlatComment c)
            c.VoteDelta = c.VoteDelta == 1 ? 0 : 1;
    }

    private void OnDownvoteTapped(object sender, TappedEventArgs e)
    {
        if (((Element)sender).BindingContext is FlatComment c)
            c.VoteDelta = c.VoteDelta == -1 ? 0 : -1;
    }
}

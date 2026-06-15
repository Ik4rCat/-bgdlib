using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

[QueryProperty(nameof(Note), "Note")]
public partial class NoteEditorPage : ContentPage
{
    private readonly NoteEditorViewModel _vm;
    public Note? Note { get; set; }

    public NoteEditorPage(NoteEditorViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        _vm.ContentLoaded += async content =>
        {
            var escaped = content.Replace("\\", "\\\\").Replace("`", "\\`").Replace("'", "\\'");
            await EditorWebView.EvaluateJavaScriptAsync($"setContent(`{escaped}`)");
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        EditorWebView.Navigated += OnEditorLoaded;
        EditorWebView.Source = new HtmlWebViewSource
        {
            Html = await LoadEditorHtmlAsync(),
            BaseUrl = "file:///android_asset/"
        };
    }

    private async void OnEditorLoaded(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success) return;
        EditorWebView.Navigated -= OnEditorLoaded;
        await _vm.InitAsync(Note);
    }

    private async void OnEditorNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (!e.Url.StartsWith("bgdlib://")) return;
        e.Cancel = true;

        if (e.Url.StartsWith("bgdlib://save"))
        {
            var encoded = e.Url["bgdlib://save?content=".Length..];
            var content = Uri.UnescapeDataString(encoded);
            await _vm.SaveCommand.ExecuteAsync(content);
        }
        else if (e.Url.StartsWith("bgdlib://export"))
        {
            await _vm.ExportCommand.ExecuteAsync(null);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var content = await EditorWebView.EvaluateJavaScriptAsync("easyMDE.value()");
        await _vm.SaveCommand.ExecuteAsync(content?.Trim('"') ?? string.Empty);
    }

    private async void OnExportClicked(object sender, EventArgs e)
        => await _vm.ExportCommand.ExecuteAsync(null);

    private static async Task<string> LoadEditorHtmlAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("editor.html");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}

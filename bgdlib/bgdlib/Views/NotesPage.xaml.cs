using bgdlib.Models;
using bgdlib.Services;
using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class NotesPage : ContentPage
{
    private readonly NotesViewModel _vm;

    public NotesPage(NotesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Title = LocalizationService.Instance["Notes_Title"];
        await _vm.LoadCommand.ExecuteAsync(null);
    }

    private async void OnNewNoteClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(NoteEditorPage));

    private async void OnNoteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Note note)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync(nameof(NoteEditorPage),
                new Dictionary<string, object> { ["Note"] = note });
        }
    }
}

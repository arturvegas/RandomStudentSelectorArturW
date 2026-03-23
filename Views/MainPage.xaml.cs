using Microsoft.Maui.Controls.PlatformConfiguration;

namespace RandomStudentSelectorArturW.Views;

using RandomStudentSelectorArturW.Models;
using RandomStudentSelectorArturW.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class MainPage : ContentPage
{
    private ClassRoom currentClass;
    private DrawService drawService = new();
    private ObservableCollection<string> classes;

    public MainPage()
    {
        InitializeComponent();
        classes = new ObservableCollection<string>();
        ClassesCollection.ItemsSource = classes;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var classList = FileService.GetAllClasses();
            classes.Clear();
            foreach (var className in classList)
            {
                classes.Add(className);
            }

            UpdateUI();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex.Message}");
        }
    }

    private void UpdateUI()
    {
        EmptyClassesLabel.IsVisible = classes.Count == 0;

        ClassPicker.Items.Clear();
        foreach (var className in classes)
        {
            ClassPicker.Items.Add(className);
        }
    }

    private async void OnAddClassClicked(object sender, EventArgs e)
    {
        try
        {
            string result = await DisplayPromptAsync(
                "Nowa klasa",
                "Wpisz nazwe klasy (np. 4J, 3A, IB):",
                "Dodaj",
                "Anuluj",
                "Wpisz tu...",
                maxLength: 20,
                keyboard: Keyboard.Default);

            if (string.IsNullOrWhiteSpace(result))
                return;

            FileService.AddClass(result);
            LoadData();
            await DisplayAlert("Sukces", $"Klasa {result} została dodana!", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnAddClassClicked Error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się dodać klasy", "OK");
        }
    }

    private async void OnEditClassClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            string className = button?.BindingContext as string;

            if (string.IsNullOrEmpty(className))
                return;

            string newName = await DisplayPromptAsync(
                "Edytuj klasę",
                "Nowa nazwa:",
                "Zapisz",
                "Anuluj",
                className,
                maxLength: 20);

            if (string.IsNullOrWhiteSpace(newName) || newName == className)
                return;

            FileService.RenameClass(className, newName);
            LoadData();
            await DisplayAlert("Sukces", $"Klasa została zmieniona na {newName}", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnEditClassClicked Error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się edytować klasy", "OK");
        }
    }

    private async void OnDeleteClassClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            string className = button?.BindingContext as string;

            if (string.IsNullOrEmpty(className))
                return;

            bool confirm = await DisplayAlert(
                "Potwierdz",
                $"Czy naprawdę chcesz usunąć klasę {className} wraz ze wszystkimi uczniami?",
                "Tak",
                "Nie");

            if (!confirm)
                return;

            FileService.DeleteClass(className);
            LoadData();
            await DisplayAlert("Sukces", "Klasa została usunięta", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnDeleteClassClicked Error: {ex.Message}");
            await DisplayAlert("Błąd", "Nie udało się usunać klasy", "OK");
        }
    }

    private void OnClassChanged(object sender, EventArgs e)
    {
        try
        {
            if (ClassPicker.SelectedItem == null)
            {
                currentClass = null;
                ResultLabel.Text = "Wybierz klasę aby rozpocząć";
                return;
            }

            string className = ClassPicker.SelectedItem.ToString();
            currentClass = FileService.Load(className);
            ResultLabel.Text = $"Wybrana: {className}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnClassChanged Error: {ex.Message}");
            ResultLabel.Text = "Błąd przy ladowaniu klasy";
        }
    }

    private void OnDrawClicked(object sender, EventArgs e)
    {
        try
        {
            if (currentClass == null)
            {
                ResultLabel.Text = "Najpierw wybierz klasę!";
                return;
            }

            if (currentClass.Students == null || currentClass.Students.Count == 0)
            {
                ResultLabel.Text = "W tej klasie nie ma jeszcze uczniów";
                return;
            }

            var student = drawService.Draw(currentClass.Students);

            if (student == null)
            {
                ResultLabel.Text = "Nie można wylosować ucznia";
                return;
            }

            ResultLabel.Text = $"Wylosowany: {student.Number}. {student.Name}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnDrawClicked Error: {ex.Message}");
            ResultLabel.Text = "Błąd przy losowaniu";
        }
    }

    private async void OnEditStudentsClicked(object sender, EventArgs e)
    {
        try
        {
            if (ClassPicker.SelectedItem == null)
            {
                ResultLabel.Text = "Najpierw wybierz klasę!";
                return;
            }

            string className = ClassPicker.SelectedItem.ToString();
            await Navigation.PushAsync(new EditStudentsPage(className));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnEditStudentsClicked Error: {ex.Message}");
            ResultLabel.Text = "Błąd przy otwieraniu edytora";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }
}
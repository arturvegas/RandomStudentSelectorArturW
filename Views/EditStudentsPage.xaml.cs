namespace RandomStudentSelectorArturW.Views;

using RandomStudentSelectorArturW.Models;
using RandomStudentSelectorArturW.Services;
using System.Collections.ObjectModel;

public partial class EditStudentsPage : ContentPage
{
	private ClassRoom classRoom;
	private ObservableCollection<Student> students;
	private string selectedClassName;

	public EditStudentsPage(string className)
	{
		InitializeComponent();
		selectedClassName = className;
		LoadStudents();
	}

	private void LoadStudents()
	{
		try
		{
			classRoom = FileService.Load(selectedClassName);
			if (classRoom?.Students != null)
			{
				students = new ObservableCollection<Student>(classRoom.Students);
				StudentsCollection.ItemsSource = students;
				ClassNameLabel.Text = $"Klasa: {selectedClassName}";
				RefreshUI();
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"LoadStudents Error: {ex.Message}");
			DisplayAlert("Blad", $"Nie udało się załadować uczniów: {ex.Message}", "OK");
		}
	}

	private void RefreshUI()
	{
		EmptyMessage.IsVisible = students.Count == 0;
		System.Diagnostics.Debug.WriteLine($"UI Updated. Student count: {students.Count}");
	}

	private async void OnEditClassNameClicked(object sender, EventArgs e)
	{
		try
		{
			string newClassName = await DisplayPromptAsync(
				"Edytuj nazwe klasy",
				"Nowa nazwa:",
				"Zapisz",
				"Anuluj",
				selectedClassName,
				maxLength: 20);

			if (string.IsNullOrWhiteSpace(newClassName) || newClassName == selectedClassName)
				return;

			FileService.RenameClass(selectedClassName, newClassName);
			selectedClassName = newClassName;
			LoadStudents();
			await DisplayAlert("Sukces", $"Klasa została zmieniona na {newClassName}", "OK");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"EditClassName Error: {ex.Message}");
			await DisplayAlert("Blad", "Nie udało się zmienic nazwy klasy", "OK");
		}
	}

	private async void OnAddStudentClicked(object sender, EventArgs e)
	{
		try
		{
			string name = StudentEntry.Text?.Trim();

			if (string.IsNullOrEmpty(name))
			{
				await DisplayAlert("Blad", "Wpisz imie i nazwisko ucznia", "OK");
				return;
			}

			if (classRoom == null)
			{
				await DisplayAlert("Blad", "Dane klasy nie zostały załadowane", "OK");
				return;
			}

			if (students.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
			{
				await DisplayAlert("Blad", $"Uczeń {name} już istnieje w tej klasie", "OK");
				return;
			}

			var newStudent = new Student { Number = students.Count + 1, Name = name };

			students.Add(newStudent);
			classRoom.Students.Add(newStudent);

			FileService.Save(classRoom);

			StudentEntry.Text = string.Empty;
			StudentEntry.Focus();

			RefreshUI();

			System.Diagnostics.Debug.WriteLine($"Added student: {newStudent.Number}. {name}. Total: {students.Count}");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"AddStudent Error: {ex.Message}\n{ex.StackTrace}");
			await DisplayAlert("Blad", $"Nie udało się dodać ucznia: {ex.Message}", "OK");
		}
	}

	private async void OnEditStudentClicked(object sender, EventArgs e)
	{
		try
		{
			var button = sender as Button;
			var student = button?.BindingContext as Student;
			if (student == null || classRoom == null)
				return;

			string newName = await DisplayPromptAsync(
				"Edytuj ucznia",
				"Nowe imie i nazwisko:",
				"Zapisz",
				"Anuluj",
				student.Name,
				maxLength: 50);

			if (string.IsNullOrWhiteSpace(newName) || newName == student.Name)
				return;

			if (students.Any(s => s.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
			{
				await DisplayAlert("Blad", "Ten uczeń już istnieje w klasie", "OK");
				return;
			}

			student.Name = newName;
			classRoom.Students[classRoom.Students.IndexOf(student)] = student;
			FileService.Save(classRoom);

			int index = students.IndexOf(student);
			students.RemoveAt(index);
			students.Insert(index, student);

			await DisplayAlert("Sukces", "Uczeń został zmieniony", "OK");
			System.Diagnostics.Debug.WriteLine($"Edited student to: {newName}");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"EditStudent Error: {ex.Message}");
			await DisplayAlert("Blad", $"Nie udało się edytować ucznia: {ex.Message}", "OK");
		}
	}

	private async void OnDeleteStudentClicked(object sender, EventArgs e)
	{
		try
		{
			var button = sender as Button;
			var student = button?.BindingContext as Student;
			if (student == null || classRoom == null)
				return;

			bool confirm = await DisplayAlert(
				"Potwierdz usuniecie",
				$"Czy chcesz usunac {student.Name}?",
				"Tak",
				"Nie");

			if (!confirm)
				return;

			students.Remove(student);

			var updatedStudents = new List<Student>();
			for (int i = 0; i < students.Count; i++)
			{
				updatedStudents.Add(new Student { Number = i + 1, Name = students[i].Name });
			}

			students.Clear();
			foreach (var s in updatedStudents)
			{
				students.Add(s);
			}

			classRoom.Students.Clear();
			classRoom.Students.AddRange(students);

			FileService.Save(classRoom);
			RefreshUI();

			System.Diagnostics.Debug.WriteLine($"Deleted student: {student.Name}. Total: {students.Count}");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"DeleteStudent Error: {ex.Message}");
			await DisplayAlert("Blad", $"Nie udało się usunac ucznia: {ex.Message}", "OK");
		}
	}
}

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.StyleSheets;
using System.Reflection;

namespace RandomStudentSelectorArturW
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            try
            {
                Resources.Add(StyleSheet.FromResource("RandomStudentSelectorArturW.Resources.Styles.AppStyles.css", typeof(App).Assembly));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading CSS: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
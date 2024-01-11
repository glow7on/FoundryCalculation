using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;

namespace FoundryCalculation
{
    public partial class StartPage : Window
    {
        string version = "v1.1";
        public StartPage()
        {
            InitializeComponent();
            AppVersion.Content = version;
        }

        private void CreateNewForm(object sender, RoutedEventArgs e)
        {
            FormCalculation Window = new FormCalculation(true);
            Window.Show();
        }

        private void CreateEmptyForm(object sender, RequestNavigateEventArgs e)
        {
            FormCalculation Window = new FormCalculation(false);
            Window.Show();
        }
    }
}

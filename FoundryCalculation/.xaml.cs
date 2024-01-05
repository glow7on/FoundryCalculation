using System;
using System.Windows;

namespace FoundryCalculation
{
    public partial class StartPage : Window
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void CreateNewForm(object sender, RoutedEventArgs e)
        {
                FormCalculation Window = new FormCalculation();
                Window.Show();
        }
    }
}

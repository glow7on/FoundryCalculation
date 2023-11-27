using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using AdonisUI.Controls;

using GitViewer.Desktop.ViewModels;

namespace GitViewer.Desktop.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : AdonisWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel model)
                if (sender is PasswordBox password)
                    model.Password = password.Password;
        }

        public (string clientID, string clientSecret, string login, string password) GetUserInput()
        {
            if (DataContext is SettingsViewModel model)
                return model.GetUserInput();
            return (string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}

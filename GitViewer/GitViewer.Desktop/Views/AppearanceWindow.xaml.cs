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

using AdonisUI;
using AdonisUI.Controls;

namespace GitViewer.Desktop.Views
{
    /// <summary>
    /// Логика взаимодействия для AppearanceWindow.xaml
    /// </summary>
    public partial class AppearanceWindow : AdonisWindow
    {
        public AppearanceWindow()
        {
            InitializeComponent();
        }

        bool _isDark = false;

        private void ColorModeChanged(object sender, RoutedEventArgs e)
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

            _isDark = !_isDark;
        }
    }
}

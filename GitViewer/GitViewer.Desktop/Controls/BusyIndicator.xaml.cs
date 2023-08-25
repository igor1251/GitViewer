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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitViewer.Desktop.Controls
{
    /// <summary>
    /// Логика взаимодействия для BusyIndicator.xaml
    /// </summary>
    public partial class BusyIndicator : UserControl
    {
        public readonly static DependencyProperty IsBusyProperty = 
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, new PropertyChangedCallback((obj, e) =>
            {
                if (obj is BusyIndicator indicator)
                {
                    indicator.PART_Content.IsHitTestVisible = !indicator.IsBusy;
                    indicator.PART_Content.Opacity = indicator.IsBusy ? 0 : 1;
                    indicator.PART_Indicator.IsHitTestVisible = indicator.IsBusy;
                    indicator.PART_Indicator.Opacity = indicator.IsBusy ? 1 : 0;
                }
            })));

        public readonly static new DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(BusyIndicator), new PropertyMetadata(null));

        public new object Content
        {
            get => (object)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public BusyIndicator()
        {
            InitializeComponent();
        }
    }
}

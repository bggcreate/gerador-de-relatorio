using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ThermalCamera.window
{
    /// <summary>
    /// Interaction logic for HeatMapWnd.xaml
    /// </summary>
    public partial class HeatMapWnd : Window
    {
        public HeatMapWnd()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(((TextBox)sender).Text + e.Text);
                e.Handled = false;
            }
            catch
            {
                e.Handled = true;
            } 
        }
    }
}

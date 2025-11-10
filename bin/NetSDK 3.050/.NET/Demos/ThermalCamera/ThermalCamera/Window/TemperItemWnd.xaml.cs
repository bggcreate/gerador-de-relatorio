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
    /// Interaction logic for TemperItemWnd.xaml
    /// </summary>
    public partial class TemperItemWnd : Window
    {
        public TemperItemWnd()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput_PresetId(object sender, TextCompositionEventArgs e)
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

        private void TextBox_PreviewTextInput_RuleId(object sender, TextCompositionEventArgs e)
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

        private void TextBox_PreviewTextInput_Channel(object sender, TextCompositionEventArgs e)
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

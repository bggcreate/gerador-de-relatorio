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
    /// Interaction logic for TemperPointWnd.xaml
    /// </summary>
    public partial class TemperPointWnd : Window
    {
        public TemperPointWnd()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput_X(object sender, TextCompositionEventArgs e)
        {
            try
            {
                short n = Convert.ToInt16(((TextBox)sender).Text+e.Text);
                if (n > 8191)
                {
                    ((TextBox)sender).Text = "8191";
                    ((TextBox)sender).Select(4, 0);
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            catch
            {
                e.Handled = true;
            } 
        }

        private void TextBox_PreviewTextInput_Y(object sender, TextCompositionEventArgs e)
        {
            try
            {
                short n = Convert.ToInt16(((TextBox)sender).Text + e.Text);
                if (n > 8191)
                {
                    ((TextBox)sender).Text = "8191";
                    ((TextBox)sender).Select(4, 0);
                    e.Handled = true;
                    return;
                }
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

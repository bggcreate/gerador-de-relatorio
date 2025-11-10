using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OSDDemo
{
    public partial class AreaTextBox : TextBox
    {
        private const int MaxAreaValue = 8191;
        private const int MinAreaValue = 0;
        public AreaTextBox()
        {
            InitializeComponent();
            this.Text = MinAreaValue.ToString();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (this.Text == "")
            {
                this.Text = MinAreaValue.ToString();
            }
            if (this.Text.Length == 4)
            {
                try
                {
                    short value = Convert.ToInt16(this.Text);
                    if (value > MaxAreaValue)
                    {
                        this.Text = MaxAreaValue.ToString();
                    }
                }
                catch
                {
                    this.Text = MaxAreaValue.ToString();
                }
            }
            if (this.Text.Length > 4)
            {
                this.Text = MaxAreaValue.ToString();
            }
        }
    }
}

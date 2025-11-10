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
    public partial class ByteTextBox : TextBox
    {
        public ByteTextBox()
        {
            InitializeComponent();
            this.Text = Byte.MinValue.ToString();
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
                this.Text = Byte.MinValue.ToString();
            }
            if (this.Text.Length == 3)
            {
                try
                {
                    Convert.ToByte(this.Text);
                }
                catch
                {
                    this.Text = Byte.MaxValue.ToString();
                }
            }
            if (this.Text.Length > 3)
            {
                this.Text = Byte.MaxValue.ToString();
            }
        }
    }
}

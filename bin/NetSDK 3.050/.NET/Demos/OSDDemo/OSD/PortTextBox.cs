using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OSDDemo
{
    public partial class PortTextBox : TextBox
    {
        public PortTextBox()
        {
            InitializeComponent();
            this.Text = ushort.MinValue.ToString();
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
                this.Text = ushort.MinValue.ToString();
            }
            if (this.Text.Length == 5)
            {
                try
                {
                    Convert.ToUInt16(this.Text);
                }
                catch
                {
                    this.Text = ushort.MaxValue.ToString();
                }
            }
            if (this.Text.Length > 5)
            {
                this.Text = ushort.MaxValue.ToString();
            }
        }
    }
}

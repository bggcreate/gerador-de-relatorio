using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace DeviceInit
{
    public partial class PointSetDialog : Form
    {
        public int IPCount { get; set; }
        public List<string> IPList = new List<string>();
        public PointSetDialog()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (textBox_startip.Text == "" || textBox_endip.Text == "")
            {
                MessageBox.Show(this,"Please input ip address(请输入IP地址)");
                return;
            }
            try
            {
                if (!textBox_startip.Text.Trim().Contains(".") || !textBox_endip.Text.Trim().Contains("."))
                {
                    MessageBox.Show(this,"Please input a valid IP address(请输入有效的IP地址)");
                    return;
                }
                string[] splitStart = textBox_startip.Text.Trim().Split('.');
                string[] splitEnd = textBox_endip.Text.Trim().Split('.');
                if (splitStart.Length != 4 || splitEnd.Length != 4)
                {
                    MessageBox.Show(this,"Please input a valid IP address(请输入有效的IP地址)");
                    return;
                }
                byte[] startIP = IPAddress.Parse(textBox_startip.Text.Trim()).GetAddressBytes();
                byte[] endIP = IPAddress.Parse(textBox_endip.Text.Trim()).GetAddressBytes();
                if (startIP[0] == endIP[0] && startIP[1] == endIP[1])
                {
                    if (startIP[2] != endIP[2])
                    {
                        if ((startIP[2] + 1) != endIP[2])
                        {
                            MessageBox.Show(this,"IP amount exceed 256(IP数量超过256)");
                            return;
                        }
                        else
                        {
                            if ((startIP[3] - 1) < endIP[3])
                            {
                                MessageBox.Show(this,"IP amount exceed 256(IP数量超过256)");
                                return;
                            }
                            else
                            {
                                IPCount = 256 + startIP[3] - 1 - endIP[3];
                                for (int i = startIP[3]; i <= 255; i++)
                                {
                                    byte[] byIP = new byte[4];
                                    byIP[0] = startIP[0];
                                    byIP[1] = startIP[1];
                                    byIP[2] = startIP[2];
                                    byIP[3] = (byte)i;
                                    IPAddress ip = new IPAddress(byIP);
                                    IPList.Add(ip.ToString());
                                }
                                for (int i = 0; i <= endIP[3]; i++)
                                {
                                    byte[] byIP = new byte[4];
                                    byIP[0] = startIP[0];
                                    byIP[1] = startIP[1];
                                    byIP[2] = endIP[2];
                                    byIP[3] = (byte)i;
                                    IPAddress ip = new IPAddress(byIP);
                                    IPList.Add(ip.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        IPCount = endIP[3] - startIP[3] + 1;
                        for (int i = startIP[3]; i <= endIP[3]; i++)
                        {
                            byte[] byIP = new byte[4];
                            byIP[0] = startIP[0];
                            byIP[1] = startIP[1];
                            byIP[2] = startIP[2];
                            byIP[3] = (byte)i;
                            IPAddress ip = new IPAddress(byIP);
                            IPList.Add(ip.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this,"IP amount exceed 256(IP数量超过256)");
                    return;
                }
            }
            catch
            {
                MessageBox.Show(this,"Please input a valid IP address(请输入有效的IP地址)");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

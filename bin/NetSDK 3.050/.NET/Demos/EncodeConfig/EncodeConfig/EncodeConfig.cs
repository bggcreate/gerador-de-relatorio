using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EncodeConfig
{
    public partial class EncodeConfig : Form
    {
        private string[] _ProfileName = { "H.264 B", "H.264  ", "H.264 E", "H.264 H" };
        private string[] _VideoCompression = {"MPEG4","MS_MPEG4","MPEG2","MPEG1","H263","MJPG","FCC_MPEG4","H264","H.265","SVAC"};
        private int[] _BitRateAll = {32, 48, 64, 80, 96, 128, 160, 192, 224,256, 320, 384, 448, 512, 640, 768, 896, 
        1024, 1280, 1536, 1792, 2048, 3072, 4096, 6144, 7168,8192, 10240, 12288, 14336, 16384, 18432, 20480, 22528};
        private IntPtr _LoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex _DeviceInfo = new NET_DEVICEINFO_Ex();
        private NET_ENCODE_VIDEO_INFO _VideoInfo;
        private NET_ENCODE_VIDEO_PROFILE_INFO _ProFileInfo; 

        public EncodeConfig()
        {
            InitializeComponent();
            try
            {
                NETClient.Init(null, IntPtr.Zero, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Load += new EventHandler(EncodeConfig_Load);
        }

        void EncodeConfig_Load(object sender, EventArgs e)
        {
            this.button_get.Enabled = false;
            this.button_set.Enabled = false;
            this.comboBox_channel.Enabled = false;
            this.comboBox_encodingtype.Enabled = false;
            this.comboBox_stream.Enabled = false;
            this.comboBox_frame.Enabled = false;
            this.textBox_i.Enabled = false;
            this.label_value.Visible = false;
            this.textBox_value.Visible = false;
            this.textBox_value.Text = "";
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == _LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(this.textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入的端口错误)!");
                    return;
                }
                _LoginID = NETClient.Login(this.textBox_ip.Text.Trim(), port, this.textBox_username.Text.Trim(), this.textBox_password.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref _DeviceInfo);
                if (IntPtr.Zero == _LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                for (int i = 0; i < _DeviceInfo.nChanNum; i++)
                {
                    this.comboBox_channel.Items.Add(i + 1);
                }
                this.button_login.Text = "Logout(登出)";
                this.button_get.Enabled = true;
                this.button_set.Enabled = true;
                this.comboBox_channel.Enabled = true;
                this.comboBox_encodingtype.Enabled = true;
                this.comboBox_stream.Enabled = true;
                this.comboBox_frame.Enabled = true;
                this.textBox_i.Enabled = true;
                this.comboBox_channel.SelectedIndex = 0;
            }
            else
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.button_login.Text = "Login(登录)";
                this.button_get.Enabled = false;
                this.button_set.Enabled = false;
                this.comboBox_channel.Enabled = false;
                this.comboBox_encodingtype.Enabled = false;
                this.comboBox_stream.Enabled = false;
                this.comboBox_frame.Enabled = false;
                this.textBox_i.Enabled = false;
                this.textBox_i.Text = "";
                this.comboBox_channel.Items.Clear();
                this.comboBox_encodingtype.Items.Clear();
                this.comboBox_stream.Items.Clear();
                this.comboBox_frame.Items.Clear();
                this.label_value.Visible = false;
                this.textBox_value.Visible = false;
                this.textBox_value.Text = "";
            }
        }

        private void button_get_Click(object sender, EventArgs e)
        {
            this.comboBox_encodingtype.Items.Clear();
            this.comboBox_frame.Items.Clear();
            this.comboBox_stream.Items.Clear();
            Config.NET_CFG_ENCODE_INFO info = new Config.NET_CFG_ENCODE_INFO();
            object obj = info;
            bool ret = NETClient.GetNewDevConfig(_LoginID, this.comboBox_channel.SelectedIndex, Config.CFG_CMD_ENCODE, ref obj, typeof(Config.NET_CFG_ENCODE_INFO), 5000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            info = (Config.NET_CFG_ENCODE_INFO)obj;
            uint outsize = 1024 * 1024;
            IntPtr inPtr = IntPtr.Zero;
            IntPtr outPtr = IntPtr.Zero;
            IntPtr inCapsPtr = IntPtr.Zero;
            IntPtr outCapPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Config.NET_CFG_ENCODE_INFO)));
                outPtr = Marshal.AllocHGlobal((int)outsize);
                Marshal.StructureToPtr(info, inPtr, true);
                for (int i = 0; i < outsize; i++)
                {
                    Marshal.WriteByte(outPtr, i, 0);
                }
                ret = NETClient.PacketData(Config.CFG_CMD_ENCODE, inPtr, (uint)Marshal.SizeOf(typeof(Config.NET_CFG_ENCODE_INFO)), outPtr, outsize);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                NET_IN_ENCODE_CFG_CAPS inCaps = new NET_IN_ENCODE_CFG_CAPS();
                NET_OUT_ENCODE_CFG_CAPS outCaps = new NET_OUT_ENCODE_CFG_CAPS();
                inCaps.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ENCODE_CFG_CAPS));
                inCaps.nChannelId = this.comboBox_channel.SelectedIndex;
                inCaps.pchEncodeJson = outPtr;
                outCaps.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ENCODE_CFG_CAPS));
                outCaps.stuExtraFormatCaps = new NET_STREAM_CFG_CAPS[3];
                outCaps.stuMainFormatCaps = new NET_STREAM_CFG_CAPS[3];
                outCaps.stuSnapFormatCaps = new NET_STREAM_CFG_CAPS[2];
                for (int i = 0; i < 3; i++)
                {
                    outCaps.stuExtraFormatCaps[i].dwSize = (uint)Marshal.SizeOf(typeof(NET_STREAM_CFG_CAPS));
                    outCaps.stuMainFormatCaps[i].dwSize = (uint)Marshal.SizeOf(typeof(NET_STREAM_CFG_CAPS));
                }
                for (int i = 0; i < 2; i++)
                {
                    outCaps.stuSnapFormatCaps[i].dwSize = (uint)Marshal.SizeOf(typeof(NET_STREAM_CFG_CAPS));
                }
                inCapsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_ENCODE_CFG_CAPS)));
                outCapPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_ENCODE_CFG_CAPS)));
                Marshal.StructureToPtr(inCaps, inCapsPtr, true);
                Marshal.StructureToPtr(outCaps, outCapPtr, true);
                ret = NETClient.GetDevCaps(_LoginID, EM_DEVCAP_TYPE.ENCODE_CFG_CAPS, inCapsPtr, outCapPtr, 5000);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                outCaps = (NET_OUT_ENCODE_CFG_CAPS)Marshal.PtrToStructure(outCapPtr, typeof(NET_OUT_ENCODE_CFG_CAPS));
                int encodeCount = Enum.GetNames(typeof(EM_VIDEO_COMPRESSION)).Length;
                for (int i = 0; i < encodeCount; i++)
                {
                    if ((outCaps.stuMainFormatCaps[0].dwEncodeModeMask & (0x01 << i)) > 0)
                    {
                        if (i == (int)EM_VIDEO_COMPRESSION.H264 && outCaps.stuMainFormatCaps[0].nH264ProfileRankNum > 0)
                        {
                            for (int j = 0; j < outCaps.stuMainFormatCaps[0].nH264ProfileRankNum; j++)
                            {
                                int selectIndex = this.comboBox_encodingtype.Items.Add(_ProfileName[outCaps.stuMainFormatCaps[0].bH264ProfileRank[j] - 1]);
                                if (info.stuMainStream[0].stuVideoFormat.emCompression == Config.EM_CFG_VIDEO_COMPRESSION.H264)
                                {
                                    if (outCaps.stuMainFormatCaps[0].bH264ProfileRank[j] == (int)info.stuMainStream[0].stuVideoFormat.emProfile)
                                    {
                                        this.comboBox_encodingtype.SelectedIndex = selectIndex;
                                    }
                                }
                            }
                        }
                        else
                        {
                            int selectIndex = this.comboBox_encodingtype.Items.Add(_VideoCompression[i]);
                            if ((int)info.stuMainStream[0].stuVideoFormat.emCompression == i)
                            {
                                this.comboBox_encodingtype.SelectedIndex = selectIndex;
                            }
                        }
                    }
                }
                for (int i = 1; i <= outCaps.stuMainFormatCaps[0].nFPSMax; i++)
                {
                    this.comboBox_frame.Items.Add(i);
                }
                this.comboBox_frame.SelectedIndex = (int)info.stuMainStream[0].stuVideoFormat.nFrameRate - 1;
                int minIndex = 0;
                int maxIndex = 0;
                for (int i = 0; i < _BitRateAll.Length; i++)
                {
                    if (_BitRateAll[i] >= outCaps.stuMainFormatCaps[0].nMinBitRateOptions)
                    {
                        minIndex = i;
                        break;
                    }
                }
                for (int i = _BitRateAll.Length - 1; i >= 0; i--)
                {
                    if (_BitRateAll[i] <= outCaps.stuMainFormatCaps[0].nMaxBitRateOptions)
                    {
                        maxIndex = i;
                        break;
                    }
                }
                for (int i = minIndex; i <= maxIndex; i++)
                {
                    this.comboBox_stream.Items.Add(_BitRateAll[i]);
                }
                this.comboBox_stream.Items.Add("User-define(自定义)");
                foreach (var item in comboBox_stream.Items)
                {
                    if (item.ToString() == info.stuMainStream[0].stuVideoFormat.nBitRate.ToString())
                    {
                        this.comboBox_stream.SelectedIndex = comboBox_stream.Items.IndexOf(item);
                    }
                }
                if (this.comboBox_stream.SelectedItem == null)
                {
                    this.comboBox_stream.SelectedIndex = this.comboBox_stream.Items.Count - 1;
                    this.textBox_value.Text = info.stuMainStream[0].stuVideoFormat.nBitRate.ToString();
                }
                this.textBox_i.Text = info.stuMainStream[0].stuVideoFormat.nIFrameInterval.ToString();
                _VideoInfo = new NET_ENCODE_VIDEO_INFO();
                _VideoInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_ENCODE_VIDEO_INFO));
                _VideoInfo.emFormatType = EM_FORMAT_TYPE.NORMAL;
                object videoObj = _VideoInfo;
                ret = NETClient.GetEncodeConfig(_LoginID, EM_CFG_ENCODE_TYPE.VIDEO, comboBox_channel.SelectedIndex, ref videoObj, 5000);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                _VideoInfo = (NET_ENCODE_VIDEO_INFO)videoObj;
                _ProFileInfo = new NET_ENCODE_VIDEO_PROFILE_INFO();
                _ProFileInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_ENCODE_VIDEO_PROFILE_INFO));
                _ProFileInfo.emFormatType = EM_FORMAT_TYPE.NORMAL;
                object profileObj = _ProFileInfo;
                ret = NETClient.GetEncodeConfig(_LoginID, EM_CFG_ENCODE_TYPE.VIDEO_PROFILE, comboBox_channel.SelectedIndex, ref profileObj, 5000);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                _ProFileInfo = (NET_ENCODE_VIDEO_PROFILE_INFO)profileObj;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
                Marshal.FreeHGlobal(outPtr);
                Marshal.FreeHGlobal(inCapsPtr);
                Marshal.FreeHGlobal(outCapPtr);
            }
        }

        private void button_set_Click(object sender, EventArgs e)
        {
            if (comboBox_encodingtype.SelectedItem == null)
            {
                MessageBox.Show("Please get config first(请先获取配置信息)");
                return;
            }
            if (textBox_i.Text == "")
            {
                MessageBox.Show("Please input IFrameInterval value(请输入I帧间隔值)");
                return;
            }
            if (comboBox_stream.SelectedIndex == comboBox_stream.Items.Count - 1)
            {
                if (textBox_value.Text == "")
                {
                    MessageBox.Show("Please input stream value(请输入码流值)");
                    return;
                }
            }
            int iFrameInterval = 0;
            int bitRate = 0;
            try
            {
                iFrameInterval = Convert.ToInt32(textBox_i.Text.Trim());
            }
            catch
            {
                MessageBox.Show("IFrameInterval'value must be Int Type value(I帧间隔值是Int类型的值) ");
                return;
            }
            if (comboBox_stream.SelectedIndex == comboBox_stream.Items.Count - 1)
            {
                try
                {
                    bitRate = Convert.ToInt32(textBox_value.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Stream'value must be Int Type value(码流值是Int类型的值) ");
                    return;
                }
            }
            else
            {
                bitRate = int.Parse(comboBox_stream.Text);
            }
            _VideoInfo.nIFrameInterval = iFrameInterval;
            _VideoInfo.nBitRate = bitRate;
            _VideoInfo.nFrameRate = comboBox_frame.SelectedIndex + 1;
            int encodingTypeIndex = -1;
            for (int i = 0; i < _ProfileName.Length; i++)
            {
                if (_ProfileName[i] == comboBox_encodingtype.Text)
                {
                    encodingTypeIndex = i;
                }
            }
            if (encodingTypeIndex == -1)
            {
                for (int i = 0; i < _VideoCompression.Length; i++)
                {
                    if (_VideoCompression[i] == comboBox_encodingtype.Text)
                    {
                        _VideoInfo.emCompression = (EM_VIDEO_COMPRESSION)i;
                    }
                }
            }
            else
            {
                _VideoInfo.emCompression = EM_VIDEO_COMPRESSION.H264;
                _ProFileInfo.emProfile = (EM_H264_PROFILE_RANK)encodingTypeIndex + 1;
            }
            _VideoInfo.nFrameRate = comboBox_frame.SelectedIndex + 1;
            object videoObj = _VideoInfo;
            object profileObj = _ProFileInfo;
            bool ret = NETClient.SetEncodeConfig(_LoginID, EM_CFG_ENCODE_TYPE.VIDEO, comboBox_channel.SelectedIndex, videoObj, 5000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            ret = NETClient.SetEncodeConfig(_LoginID, EM_CFG_ENCODE_TYPE.VIDEO_PROFILE, comboBox_channel.SelectedIndex, profileObj, 5000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            MessageBox.Show("Set successfully(设置成功)");
        }

        private void comboBox_stream_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_stream.SelectedIndex == comboBox_stream.Items.Count - 1)
            {
                this.label_value.Visible = true;
                this.textBox_value.Visible = true;
            }
            else
            {
                this.label_value.Visible = false;
                this.textBox_value.Visible = false;
                this.textBox_value.Text = "";
            }
        }

        private void textBox_i_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}

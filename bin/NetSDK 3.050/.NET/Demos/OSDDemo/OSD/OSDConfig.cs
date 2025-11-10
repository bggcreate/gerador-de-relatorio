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

namespace OSDDemo
{
    public partial class OSDConfig : Form
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fHaveReConnectCallBack m_ReConnectCallBack;
        private NET_DEVICEINFO_Ex m_DevInfo = new NET_DEVICEINFO_Ex();
        private IntPtr m_LoginID = IntPtr.Zero;
        private IntPtr m_PlayID = IntPtr.Zero;
        private const int m_WaitTime = 3000;

        public OSDConfig()
        {
            InitializeComponent();
            this.Load += new EventHandler(OSDConfig_Load);
        }

        void OSDConfig_Load(object sender, EventArgs e)
        {
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_ReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(m_ReConnectCallBack, IntPtr.Zero);
                InitUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void InitUI()
        {
            this.Text = "OSDConfig(OSD配置)";
            this.comboBox_channel.Enabled = false;
            this.comboBox_channel.Items.Clear();
            this.button_realplay.Enabled = false;
            this.button_channelget.Enabled = false;
            this.button_channelset.Enabled = false;
            this.button_timeget.Enabled = false;
            this.button_timeset.Enabled = false;
            this.button_customget.Enabled = false;
            this.button_customset.Enabled = false;
            this.comboBox_customalign.Enabled = false;
            this.comboBox_customalign.Items.Clear();
            this.comboBox_customalign.Items.Add("Invalid(无效的对齐)");
            this.comboBox_customalign.Items.Add("Left(左对齐)");
            this.comboBox_customalign.Items.Add("Xcenter(X坐标中对齐)");
            this.comboBox_customalign.Items.Add("Ycenter(Y坐标中对齐)");
            this.comboBox_customalign.Items.Add("Center(居中)");
            this.comboBox_customalign.Items.Add("Right(右对齐)");
            this.comboBox_customalign.Items.Add("Top(顶部对齐)");
            this.comboBox_customalign.Items.Add("Bottom(底部对齐)");
            this.comboBox_customalign.Items.Add("LeftTop(左上角对齐)");
            this.comboBox_customalign.Items.Add("ChangeLine(换行对齐)");
            this.button_Login.Text = "Login(登录)";
            this.button_realplay.Text = "RealPlay(监视)";
            m_LoginID = IntPtr.Zero;
            m_PlayID = IntPtr.Zero;
            pictureBox_realplay.Refresh();
            this.textBox_channelFR.Enabled = false;
            this.textBox_channelFG.Enabled = false;
            this.textBox_channelFB.Enabled = false;
            this.textBox_channelFA.Enabled = false;
            this.textBox_channelBR.Enabled = false;
            this.textBox_channelBR.Enabled = false;
            this.textBox_channelBG.Enabled = false;
            this.textBox_channelBB.Enabled = false;
            this.textBox_channelBA.Enabled = false;
            this.textBox_channelleft.Enabled = false;
            this.textBox_channeltop.Enabled = false;
            this.textBox_timeFR.Enabled = false;
            this.textBox_timeFG.Enabled = false;
            this.textBox_timeFB.Enabled = false;
            this.textBox_timeFA.Enabled = false;
            this.textBox_timeBR.Enabled = false;
            this.textBox_timeBR.Enabled = false;
            this.textBox_timeBG.Enabled = false;
            this.textBox_timeBB.Enabled = false;
            this.textBox_timeBA.Enabled = false;
            this.textBox_timeleft.Enabled = false;
            this.textBox_timetop.Enabled = false;
            this.textBox_customtitle.Enabled = false;
            this.textBox_customFR.Enabled = false;
            this.textBox_customFG.Enabled = false;
            this.textBox_customFB.Enabled = false;
            this.textBox_customFA.Enabled = false;
            this.textBox_customBR.Enabled = false;
            this.textBox_customBR.Enabled = false;
            this.textBox_customBG.Enabled = false;
            this.textBox_customBB.Enabled = false;
            this.textBox_customBA.Enabled = false;
            this.textBox_customleft.Enabled = false;
            this.textBox_customtop.Enabled = false;
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke(new Action(UpdateDisConnectUI));
        }
        private void UpdateDisConnectUI()
        {
            this.Text = "OSDConfig(OSD配置)---offline(离线)";
        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke(new Action(UpdateReConnectUI));
        }
        private void UpdateReConnectUI()
        {
            this.Text = "OSDConfig(OSD配置)---online(在线)";
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                m_LoginID = NETClient.Login(this.textBox_IP.Text.Trim(), Convert.ToUInt16(this.textBox_Port.Text.Trim()), this.textBox_Name.Text.Trim(), this.textBox_Pwd.Text.Trim(), EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DevInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                LoginUI();
            }
            else
            {
                NETClient.Logout(m_LoginID);
                this.Text = "OSDConfig(OSD配置)";
                this.comboBox_channel.Enabled = false;
                this.comboBox_channel.Items.Clear();
                this.button_realplay.Enabled = false;
                this.button_channelget.Enabled = false;
                this.button_channelset.Enabled = false;
                this.button_timeget.Enabled = false;
                this.button_timeset.Enabled = false;
                this.button_customget.Enabled = false;
                this.button_customset.Enabled = false;
                this.comboBox_customalign.Enabled = false;
                this.comboBox_customalign.Items.Clear();
                this.comboBox_customalign.Items.Add("Invalid(无效的对齐)");
                this.comboBox_customalign.Items.Add("Left(左对齐)");
                this.comboBox_customalign.Items.Add("Xcenter(X坐标中对齐)");
                this.comboBox_customalign.Items.Add("Ycenter(Y坐标中对齐)");
                this.comboBox_customalign.Items.Add("Center(居中)");
                this.comboBox_customalign.Items.Add("Right(右对齐)");
                this.comboBox_customalign.Items.Add("Top(顶部对齐)");
                this.comboBox_customalign.Items.Add("Bottom(底部对齐)");
                this.comboBox_customalign.Items.Add("LeftTop(左上角对齐)");
                this.comboBox_customalign.Items.Add("ChangeLine(换行对齐)");
                this.button_Login.Text = "Login(登录)";
                this.button_realplay.Text = "RealPlay(监视)";
                m_LoginID = IntPtr.Zero;
                m_PlayID = IntPtr.Zero;
                pictureBox_realplay.Refresh();
                this.textBox_channelFR.Text = "";
                this.textBox_channelFG.Text = "";
                this.textBox_channelFB.Text = "";
                this.textBox_channelFA.Text = "";
                this.textBox_channelBR.Text = "";
                this.textBox_channelBR.Text = "";
                this.textBox_channelBG.Text = "";
                this.textBox_channelBB.Text = "";
                this.textBox_channelBA.Text = "";
                this.textBox_channelleft.Text = "";
                this.textBox_channeltop.Text = "";
                this.textBox_timeFR.Text = "";
                this.textBox_timeFG.Text = "";
                this.textBox_timeFB.Text = "";
                this.textBox_timeFA.Text = "";            
                this.textBox_timeBR.Text = "";
                this.textBox_timeBR.Text = "";
                this.textBox_timeBG.Text = "";
                this.textBox_timeBB.Text = "";
                this.textBox_timeBA.Text = "";         
                this.textBox_timeleft.Text = "";
                this.textBox_timetop.Text = "";
                this.textBox_customtitle.Text = "";
                this.textBox_customFR.Text = "";
                this.textBox_customFG.Text = "";
                this.textBox_customFB.Text = "";
                this.textBox_customFA.Text = "";         
                this.textBox_customBR.Text = "";
                this.textBox_customBR.Text = "";
                this.textBox_customBG.Text = "";
                this.textBox_customBB.Text = "";
                this.textBox_customBA.Text = "";          
                this.textBox_customleft.Text = "";
                this.textBox_customtop.Text = "";

                this.textBox_channelFR.Enabled = false;
                this.textBox_channelFG.Enabled = false;
                this.textBox_channelFB.Enabled = false;
                this.textBox_channelFA.Enabled = false;
                this.textBox_channelBR.Enabled = false;
                this.textBox_channelBR.Enabled = false;
                this.textBox_channelBG.Enabled = false;
                this.textBox_channelBB.Enabled = false;
                this.textBox_channelBA.Enabled = false;
                this.textBox_channelleft.Enabled = false;
                this.textBox_channeltop.Enabled = false;
                this.textBox_timeFR.Enabled = false;
                this.textBox_timeFG.Enabled = false;
                this.textBox_timeFB.Enabled = false;
                this.textBox_timeFA.Enabled = false;
                this.textBox_timeBR.Enabled = false;
                this.textBox_timeBR.Enabled = false;
                this.textBox_timeBG.Enabled = false;
                this.textBox_timeBB.Enabled = false;
                this.textBox_timeBA.Enabled = false;
                this.textBox_timeleft.Enabled = false;
                this.textBox_timetop.Enabled = false;
                this.textBox_customtitle.Enabled = false;
                this.textBox_customFR.Enabled = false;
                this.textBox_customFG.Enabled = false;
                this.textBox_customFB.Enabled = false;
                this.textBox_customFA.Enabled = false;
                this.textBox_customBR.Enabled = false;
                this.textBox_customBR.Enabled = false;
                this.textBox_customBG.Enabled = false;
                this.textBox_customBB.Enabled = false;
                this.textBox_customBA.Enabled = false;
                this.textBox_customleft.Enabled = false;
                this.textBox_customtop.Enabled = false;
            }
        }

        private void LoginUI()
        {
            this.Text = "OSDConfig(OSD配置)---online(在线)";
            this.button_Login.Text = "Logout(登出)";
            this.button_realplay.Text = "RealPlay(监视)";
            this.comboBox_channel.Enabled = true;
            for (int i = 0; i < m_DevInfo.nChanNum; i++)
            {
                this.comboBox_channel.Items.Add(i + 1);
            }
            this.comboBox_channel.SelectedIndex = 0;
            this.button_realplay.Enabled = true;
            this.button_channelget.Enabled = true;
            this.button_channelset.Enabled = true;
            this.button_timeget.Enabled = true;
            this.button_timeset.Enabled = true;
            this.button_customget.Enabled = true;
            this.button_customset.Enabled = true;
            this.comboBox_customalign.Enabled = true;
            this.comboBox_customalign.SelectedIndex = 0;

            this.textBox_channelFR.Enabled = true;
            this.textBox_channelFG.Enabled = true;
            this.textBox_channelFB.Enabled = true;
            this.textBox_channelFA.Enabled = true;
            this.textBox_channelBR.Enabled = true;
            this.textBox_channelBR.Enabled = true;
            this.textBox_channelBG.Enabled = true;
            this.textBox_channelBB.Enabled = true;
            this.textBox_channelBA.Enabled = true;
            this.textBox_channelleft.Enabled = true;
            this.textBox_channeltop.Enabled = true;
            this.textBox_timeFR.Enabled = true;
            this.textBox_timeFG.Enabled = true;
            this.textBox_timeFB.Enabled = true;
            this.textBox_timeFA.Enabled = true;
            this.textBox_timeBR.Enabled = true;
            this.textBox_timeBR.Enabled = true;
            this.textBox_timeBG.Enabled = true;
            this.textBox_timeBB.Enabled = true;
            this.textBox_timeBA.Enabled = true;
            this.textBox_timeleft.Enabled = true;
            this.textBox_timetop.Enabled = true;
            this.textBox_customtitle.Enabled = true;
            this.textBox_customFR.Enabled = true;
            this.textBox_customFG.Enabled = true;
            this.textBox_customFB.Enabled = true;
            this.textBox_customFA.Enabled = true;
            this.textBox_customBR.Enabled = true;
            this.textBox_customBR.Enabled = true;
            this.textBox_customBG.Enabled = true;
            this.textBox_customBB.Enabled = true;
            this.textBox_customBA.Enabled = true;
            this.textBox_customleft.Enabled = true;
            this.textBox_customtop.Enabled = true;

        }

        private void button_realplay_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_PlayID)
            {
                m_PlayID = NETClient.RealPlay(m_LoginID, this.comboBox_channel.SelectedIndex, pictureBox_realplay.Handle);
                if (IntPtr.Zero == m_PlayID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_realplay.Text = "StopRealPlay(停止监视)";
            }
            else
            {
                NETClient.StopRealPlay(m_PlayID);
                m_PlayID = IntPtr.Zero;
                pictureBox_realplay.Refresh();
                this.button_realplay.Text = "RealPlay(监视)";
            }
        }


        private void button_channelget_Click(object sender, EventArgs e)
        {
            NET_OSD_CHANNEL_TITLE mainStreamTitleInfo = new NET_OSD_CHANNEL_TITLE();
            mainStreamTitleInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CHANNEL_TITLE));
            mainStreamTitleInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            object objMain = mainStreamTitleInfo;
            bool ret = NETClient.GetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CHANNELTITLE, this.comboBox_channel.SelectedIndex, ref objMain, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            mainStreamTitleInfo = (NET_OSD_CHANNEL_TITLE)objMain;
            this.textBox_channelleft.Text = mainStreamTitleInfo.stuRect.nLeft.ToString();
            this.textBox_channeltop.Text = mainStreamTitleInfo.stuRect.nTop.ToString();
            this.textBox_channelFR.Text = mainStreamTitleInfo.stuFrontColor.nRed.ToString();
            this.textBox_channelFG.Text = mainStreamTitleInfo.stuFrontColor.nGreen.ToString();
            this.textBox_channelFB.Text = mainStreamTitleInfo.stuFrontColor.nBlue.ToString();
            this.textBox_channelFA.Text = mainStreamTitleInfo.stuFrontColor.nAlpha.ToString();
            this.textBox_channelBR.Text = mainStreamTitleInfo.stuBackColor.nRed.ToString();
            this.textBox_channelBG.Text = mainStreamTitleInfo.stuBackColor.nGreen.ToString();
            this.textBox_channelBB.Text = mainStreamTitleInfo.stuBackColor.nBlue.ToString();
            this.textBox_channelBA.Text = mainStreamTitleInfo.stuBackColor.nAlpha.ToString();
        }

        private void button_channelset_Click(object sender, EventArgs e)
        {
            NET_OSD_CHANNEL_TITLE setTitleInfo = new NET_OSD_CHANNEL_TITLE();
            setTitleInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CHANNEL_TITLE));
            setTitleInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            setTitleInfo.bEncodeBlend = true;
            setTitleInfo.stuRect.nLeft = Convert.ToInt16(this.textBox_channelleft.Text.Trim());
            setTitleInfo.stuRect.nTop = Convert.ToInt16(this.textBox_channeltop.Text.Trim());
            setTitleInfo.stuBackColor.nAlpha = Convert.ToByte(this.textBox_channelBA.Text.Trim());
            setTitleInfo.stuBackColor.nBlue = Convert.ToByte(this.textBox_channelBB.Text.Trim());
            setTitleInfo.stuBackColor.nGreen = Convert.ToByte(this.textBox_channelBG.Text.Trim());
            setTitleInfo.stuBackColor.nRed = Convert.ToByte(this.textBox_channelBR.Text.Trim());
            setTitleInfo.stuFrontColor.nAlpha = Convert.ToByte(this.textBox_channelFA.Text.Trim());
            setTitleInfo.stuFrontColor.nBlue = Convert.ToByte(this.textBox_channelFB.Text.Trim());
            setTitleInfo.stuFrontColor.nGreen = Convert.ToByte(this.textBox_channelFG.Text.Trim());
            setTitleInfo.stuFrontColor.nRed = Convert.ToByte(this.textBox_channelFR.Text.Trim());
            object obj = setTitleInfo;
            bool ret = NETClient.SetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CHANNELTITLE, this.comboBox_channel.SelectedIndex, obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            MessageBox.Show("Set successfully(设置成功)");
        }

        private void button_timeget_Click(object sender, EventArgs e)
        {
            NET_OSD_TIME_TITLE timeInfo = new NET_OSD_TIME_TITLE();
            timeInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_TIME_TITLE));
            timeInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            object obj = timeInfo;
            bool ret = NETClient.GetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.TIMETITLE, this.comboBox_channel.SelectedIndex, ref obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            timeInfo = (NET_OSD_TIME_TITLE)obj;
            this.textBox_timeleft.Text = timeInfo.stuRect.nLeft.ToString();
            this.textBox_timetop.Text = timeInfo.stuRect.nTop.ToString();
            this.textBox_timeFR.Text = timeInfo.stuFrontColor.nRed.ToString();
            this.textBox_timeFG.Text = timeInfo.stuFrontColor.nGreen.ToString();
            this.textBox_timeFB.Text = timeInfo.stuFrontColor.nBlue.ToString();
            this.textBox_timeFA.Text = timeInfo.stuFrontColor.nAlpha.ToString();
            this.textBox_timeBR.Text = timeInfo.stuBackColor.nRed.ToString();
            this.textBox_timeBG.Text = timeInfo.stuBackColor.nGreen.ToString();
            this.textBox_timeBB.Text = timeInfo.stuBackColor.nBlue.ToString();
            this.textBox_timeBA.Text = timeInfo.stuBackColor.nAlpha.ToString();
        }

        private void button_timeset_Click(object sender, EventArgs e)
        {
            NET_OSD_TIME_TITLE timeInfo = new NET_OSD_TIME_TITLE();
            timeInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_TIME_TITLE));
            timeInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            timeInfo.bEncodeBlend = true;
            timeInfo.stuRect.nLeft = Convert.ToInt16(this.textBox_timeleft.Text.Trim());
            timeInfo.stuRect.nTop = Convert.ToInt16(this.textBox_timetop.Text.Trim());
            timeInfo.stuBackColor.nAlpha = Convert.ToByte(this.textBox_timeBA.Text.Trim());
            timeInfo.stuBackColor.nBlue = Convert.ToByte(this.textBox_timeBB.Text.Trim());
            timeInfo.stuBackColor.nGreen = Convert.ToByte(this.textBox_timeBG.Text.Trim());
            timeInfo.stuBackColor.nRed = Convert.ToByte(this.textBox_timeBR.Text.Trim());
            timeInfo.stuFrontColor.nAlpha = Convert.ToByte(this.textBox_timeFA.Text.Trim());
            timeInfo.stuFrontColor.nBlue = Convert.ToByte(this.textBox_timeFB.Text.Trim());
            timeInfo.stuFrontColor.nGreen = Convert.ToByte(this.textBox_timeFG.Text.Trim());
            timeInfo.stuFrontColor.nRed = Convert.ToByte(this.textBox_timeFR.Text.Trim());
            object obj = timeInfo;
            bool ret = NETClient.SetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.TIMETITLE, this.comboBox_channel.SelectedIndex, obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            MessageBox.Show("Set successfully(设置成功)");
        }

        private void button_customget_Click(object sender, EventArgs e)
        {
            NET_OSD_CUSTOM_TITLE customInfo = new NET_OSD_CUSTOM_TITLE();
            customInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CUSTOM_TITLE));
            customInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            object obj = customInfo;
            bool ret = NETClient.GetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CUSTOMTITLE, this.comboBox_channel.SelectedIndex, ref obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            customInfo = (NET_OSD_CUSTOM_TITLE)obj;
            this.textBox_customleft.Text = customInfo.stuCustomTitle[1].stuRect.nLeft.ToString();
            this.textBox_customtop.Text = customInfo.stuCustomTitle[1].stuRect.nTop.ToString();
            this.textBox_customFR.Text = customInfo.stuCustomTitle[1].stuFrontColor.nRed.ToString();
            this.textBox_customFG.Text = customInfo.stuCustomTitle[1].stuFrontColor.nGreen.ToString();
            this.textBox_customFB.Text = customInfo.stuCustomTitle[1].stuFrontColor.nBlue.ToString();
            this.textBox_customFA.Text = customInfo.stuCustomTitle[1].stuFrontColor.nAlpha.ToString();
            this.textBox_customBR.Text = customInfo.stuCustomTitle[1].stuBackColor.nRed.ToString();
            this.textBox_customBG.Text = customInfo.stuCustomTitle[1].stuBackColor.nGreen.ToString();
            this.textBox_customBB.Text = customInfo.stuCustomTitle[1].stuBackColor.nBlue.ToString();
            this.textBox_customBA.Text = customInfo.stuCustomTitle[1].stuBackColor.nAlpha.ToString();
            this.textBox_customtitle.Text = customInfo.stuCustomTitle[1].szText.Replace("|","\r\n");

            NET_OSD_CUSTOM_TITLE_TEXT_ALIGN customAlign = new NET_OSD_CUSTOM_TITLE_TEXT_ALIGN();
            customAlign.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CUSTOM_TITLE_TEXT_ALIGN));
            object objAlign = customAlign;
            ret = NETClient.GetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CUSTOMTITLETEXTALIGN, this.comboBox_channel.SelectedIndex, ref objAlign, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            customAlign = (NET_OSD_CUSTOM_TITLE_TEXT_ALIGN)objAlign;
            this.comboBox_customalign.SelectedIndex = (int)customAlign.emTextAlign[1];
        }

        private void button_customset_Click(object sender, EventArgs e)
        {
            NET_OSD_CUSTOM_TITLE customInfo = new NET_OSD_CUSTOM_TITLE();
            customInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CUSTOM_TITLE));
            customInfo.emOsdBlendType = EM_OSD_BLEND_TYPE.MAIN;
            customInfo.nCustomTitleNum = 4;
            customInfo.stuCustomTitle = new NET_CUSTOM_TITLE_INFO[8];
            customInfo.stuCustomTitle[1].bEncodeBlend = true;
            customInfo.stuCustomTitle[1].stuRect.nLeft = Convert.ToInt16(this.textBox_customleft.Text.Trim());
            customInfo.stuCustomTitle[1].stuRect.nTop = Convert.ToInt16(this.textBox_customtop.Text.Trim());
            customInfo.stuCustomTitle[1].stuBackColor.nAlpha = Convert.ToByte(this.textBox_customBA.Text.Trim());
            customInfo.stuCustomTitle[1].stuBackColor.nBlue = Convert.ToByte(this.textBox_customBB.Text.Trim());
            customInfo.stuCustomTitle[1].stuBackColor.nGreen = Convert.ToByte(this.textBox_customBG.Text.Trim());
            customInfo.stuCustomTitle[1].stuBackColor.nRed = Convert.ToByte(this.textBox_customBR.Text.Trim());
            customInfo.stuCustomTitle[1].stuFrontColor.nAlpha = Convert.ToByte(this.textBox_customFA.Text.Trim());
            customInfo.stuCustomTitle[1].stuFrontColor.nBlue = Convert.ToByte(this.textBox_customFB.Text.Trim());
            customInfo.stuCustomTitle[1].stuFrontColor.nGreen = Convert.ToByte(this.textBox_customFG.Text.Trim());
            customInfo.stuCustomTitle[1].stuFrontColor.nRed = Convert.ToByte(this.textBox_customFR.Text.Trim());
            customInfo.stuCustomTitle[1].szText = this.textBox_customtitle.Text.Replace("\r\n","|");
            object obj = customInfo;
            bool ret = NETClient.SetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CUSTOMTITLE, this.comboBox_channel.SelectedIndex, obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            NET_OSD_CUSTOM_TITLE_TEXT_ALIGN customAlign = new NET_OSD_CUSTOM_TITLE_TEXT_ALIGN();
            customAlign.dwSize = (uint)Marshal.SizeOf(typeof(NET_OSD_CUSTOM_TITLE_TEXT_ALIGN));
            customAlign.nCustomTitleNum = 4;
            customAlign.emTextAlign = new EM_TITLE_TEXT_ALIGNTYPE[8];
            customAlign.emTextAlign[1] = (EM_TITLE_TEXT_ALIGNTYPE)this.comboBox_customalign.SelectedIndex;
            obj = customAlign;
            ret = NETClient.SetOSDConfig(m_LoginID, EM_CFG_OSD_TYPE.CUSTOMTITLETEXTALIGN, this.comboBox_channel.SelectedIndex, obj, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            MessageBox.Show("Set successfully(设置成功)");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }
    }
}

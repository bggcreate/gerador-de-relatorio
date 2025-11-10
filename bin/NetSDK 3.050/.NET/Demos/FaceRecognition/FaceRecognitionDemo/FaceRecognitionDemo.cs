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
using System.IO;
using System.Globalization;
using System.Threading;

namespace WinForm_IPC_FaceRecognition_Demo
{
   

    public partial class FaceRecognitionDemo : Form
    {
        private static fAnalyzerDataCallBack m_AnalyzerDataCallBack;
        private static fDisConnectCallBack m_DisConnectCallBack;

        private NET_DEVICEINFO_Ex m_DevInfo = new NET_DEVICEINFO_Ex();
        private IntPtr m_LoginID = IntPtr.Zero;
        private IntPtr m_PlayID = IntPtr.Zero;
        private IntPtr m_AnalyzerID = IntPtr.Zero;
        private int m_GroupID = 0;
        private TextInfo m_TextInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

        public event Action DeviceDisconnected;

        private void OnDeviceDisconnected()
        {
            if (null != DeviceDisconnected)
            {
                DeviceDisconnected();
            }
        }

        public FaceRecognitionDemo()
        {
            InitializeComponent();
            this.Load += new EventHandler(FaceRecognitionDemo_Load);
        }

        void FaceRecognitionDemo_Load(object sender, EventArgs e)
        {
            try
            {
                m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
                m_AnalyzerDataCallBack = new fAnalyzerDataCallBack(AnalyzerDataCallBack);
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                InitUI();
                this.Text = "FaceRecognitionDemo(人脸识别Demo)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            MessageBox.Show(Marshal.PtrToStringAnsi(pchDVRIP) + "---Offline(离线)" );
            NETClient.Logout(m_LoginID);
            m_LoginID = IntPtr.Zero;
            if (m_PlayID != IntPtr.Zero)
            {
                NETClient.RenderPrivateData(m_PlayID, false);
                NETClient.StopRealPlay(m_PlayID);
                m_PlayID = IntPtr.Zero;
            }
            if (m_AnalyzerID != IntPtr.Zero)
            {
                NETClient.StopLoadPic(m_AnalyzerID);
                m_AnalyzerID = IntPtr.Zero;
            }
            OnDeviceDisconnected();
            this.BeginInvoke(new Action(InitUI));
        }

        private void InitUI()
        {
            this.groupBox_globalimage.Text = "GlobalScene_Image(全景图)";
            this.comboBox_channel.Text = "";
            this.comboBox_channel.Items.Clear();
            this.comboBox_channel.Enabled = false;
            this.button_login.Enabled = true;
            this.button_attach.Enabled = false;
            this.button_operateDB.Enabled = false;
            this.button_login.Text = "Login(登录)";
            this.button_attach.Text = "Attach(订阅)";
            this.button_realplay.Enabled = false;
            this.button_realplay.Text = "RealPlay(监视)";
            this.label_name.Text = "";
            this.label_id.Text = "";
            this.label_candidate_sex.Text = "";
            this.label_birthday.Text = "";
            this.label_similarity.Text = "";
            this.label_groupid.Text = "";
            this.label_groupname.Text = "";

            this.label_time.Text = "";
            this.label_face_sex.Text = "";
            this.label_age.Text = "";
            this.label_race.Text = "";
            this.label_eye.Text = "";
            this.label_mouth.Text = "";
            this.label_mask.Text = "";
            this.label_beard.Text = "";
            if (this.pictureBox_realplay.Image != null)
            {
                this.pictureBox_realplay.Image.Dispose();
                this.pictureBox_realplay.Image = null;
            }
            this.pictureBox_realplay.Refresh();
            if (this.pictureBox_candidateimage.Image != null)
            {
                this.pictureBox_candidateimage.Image.Dispose();
                this.pictureBox_candidateimage.Image = null;
            }
            this.pictureBox_candidateimage.Refresh();
            if (this.pictureBox_faceimage.Image != null)
            {
                this.pictureBox_faceimage.Image.Dispose();
                this.pictureBox_faceimage.Image = null;
            }
            this.pictureBox_faceimage.Refresh();
            if (this.pictureBox_image.Image != null)
            {
                this.pictureBox_image.Image.Dispose();
                this.pictureBox_image.Image = null;
            }
            this.pictureBox_image.Refresh();
            this.Text = "FaceRecognitionDemo(人脸识别Demo)---Offline(离线)";
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入的端口错误)!");
                    return;
                }
                m_LoginID = NETClient.Login(textBox_ip.Text.Trim(), port, textBox_name.Text.Trim(), textBox_pwd.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DevInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                for (int i = 0; i < m_DevInfo.nChanNum; i++)
                {
                    comboBox_channel.Items.Add(i + 1);
                }
                comboBox_channel.SelectedIndex = 0;
                LoginUI();
            }
            else
            {
                if (IntPtr.Zero != m_LoginID)
                {
                    bool ret = NETClient.Logout(m_LoginID);
                    if (!ret)
                    {
                        MessageBox.Show(NETClient.GetLastError());
                        return;
                    }
                    m_PlayID = IntPtr.Zero;
                    m_LoginID = IntPtr.Zero;
                    m_AnalyzerID = IntPtr.Zero;
					m_GroupID = 0;
                    pictureBox_realplay.Refresh();
                    pictureBox_image.Image = null;
                    pictureBox_image.Refresh();
                    pictureBox_faceimage.Image = null;
                    pictureBox_faceimage.Refresh();
                    pictureBox_candidateimage.Image = null;
                    pictureBox_candidateimage.Refresh();
                    InitUI();
                    this.Text = "FaceRecognitionDemo(人脸识别Demo)";
                }
            }
        }

        private void LoginUI()
        {
            this.button_attach.Enabled = true;
            this.button_operateDB.Enabled = true;
            this.button_realplay.Enabled = true;
            this.comboBox_channel.Enabled = true;
            this.button_login.Text = "Logout(登出)";
            this.Text = "FaceRecognitionDemo(人脸识别Demo)---Online(在线)";
        }

        private void button_attach_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_AnalyzerID)
            {
                m_AnalyzerID = NETClient.RealLoadPicture(m_LoginID, comboBox_channel.SelectedIndex, (uint)EM_EVENT_IVS_TYPE.ALL, true, m_AnalyzerDataCallBack, IntPtr.Zero, IntPtr.Zero);
                if (IntPtr.Zero == m_AnalyzerID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_attach.Text = "Detach(取消订阅)";
            }
            else
            {
                bool ret = NETClient.StopLoadPic(m_AnalyzerID);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
				m_GroupID = 0;
                m_AnalyzerID = IntPtr.Zero;
                DetachUI();
            }
        }

        private void DetachUI()
        {
            pictureBox_image.Image = null;
            pictureBox_image.Refresh();
            pictureBox_faceimage.Image = null;
            pictureBox_faceimage.Refresh();
            pictureBox_candidateimage.Image = null;
            pictureBox_candidateimage.Refresh();
            this.button_attach.Text = "Attach(订阅)";
            this.groupBox_globalimage.Text = "GlobalScene_Image(全景图)";
            this.label_name.Text = "";
            this.label_id.Text = "";
            this.label_candidate_sex.Text = "";
            this.label_birthday.Text = "";
            this.label_similarity.Text = "";
            this.label_groupid.Text = "";
            this.label_groupname.Text = "";

            this.label_time.Text = "";
            this.label_face_sex.Text = "";
            this.label_age.Text = "";
            this.label_race.Text = "";
            this.label_eye.Text = "";
            this.label_mouth.Text = "";
            this.label_mask.Text = "";
            this.label_beard.Text = "";
        }

        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            if (m_AnalyzerID == lAnalyzerHandle)
            {
                switch (dwEventType)
                {
                    case (uint)EM_EVENT_IVS_TYPE.FACERECOGNITION:
                        {
                            this.BeginInvoke(new Action(() => {
                                this.groupBox_globalimage.Text = "GlobalScene_Image(全景图) - FaceRecognition(人脸识别)";
                            }));
                            NET_DEV_EVENT_FACERECOGNITION_INFO info = (NET_DEV_EVENT_FACERECOGNITION_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_FACERECOGNITION_INFO));
                            if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                            {
                                if (info.bGlobalScenePic)
                                {
                                    if (info.stuGlobalScenePicInfo.dwFileLenth > 0)
                                    {
                                        byte[] globalScenePicInfo = new byte[info.stuGlobalScenePicInfo.dwFileLenth];
                                        Marshal.Copy(IntPtr.Add(pBuffer, (int)info.stuGlobalScenePicInfo.dwOffSet), globalScenePicInfo, 0, (int)info.stuGlobalScenePicInfo.dwFileLenth);
                                        using (MemoryStream stream = new MemoryStream(globalScenePicInfo))
                                        {
                                            try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                                            {
                                                Image globalSceneImage = Image.FromStream(stream);
                                                pictureBox_image.Image = globalSceneImage;
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //clear
                                    this.BeginInvoke(new Action(() => {
                                        pictureBox_image.Image = null;
                                        pictureBox_image.Refresh();
                                    }));
                                }
                                if (info.stuObject.stPicInfo.dwFileLenth > 0)
                                {
                                    byte[] personFaceInfo = new byte[info.stuObject.stPicInfo.dwFileLenth];
                                    Marshal.Copy(IntPtr.Add(pBuffer, (int)info.stuObject.stPicInfo.dwOffSet), personFaceInfo, 0, (int)info.stuObject.stPicInfo.dwFileLenth);
                                    using (MemoryStream stream = new MemoryStream(personFaceInfo))
                                    {
                                        try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                                        {   
                                            Image faceImage = Image.FromStream(stream);
                                            pictureBox_faceimage.Image = faceImage;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex);
                                        }
                                    }
                                    this.BeginInvoke(new Action<NET_DEV_EVENT_FACERECOGNITION_INFO>(UpdateFaceProperty), info);
                                }
                                if (info.nCandidateNum > 0)
                                {
                                    var candidatesInfo = info.stuCandidates.ToList().OrderByDescending(p => p.bySimilarity).ToArray(); 
                                    NET_CANDIDATE_INFO maxSimilarityPersonInfo = candidatesInfo[0];
                                    if (maxSimilarityPersonInfo.stPersonInfo.szFacePicInfo[0].dwFileLenth > 0)
                                    {
                                        byte[] candidateInfo = new byte[maxSimilarityPersonInfo.stPersonInfo.szFacePicInfo[0].dwFileLenth];
                                        Marshal.Copy(IntPtr.Add(pBuffer, (int)maxSimilarityPersonInfo.stPersonInfo.szFacePicInfo[0].dwOffSet), candidateInfo, 0, (int)maxSimilarityPersonInfo.stPersonInfo.szFacePicInfo[0].dwFileLenth);
                                        using (MemoryStream stream = new MemoryStream(candidateInfo))
                                        {
                                            try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                                            {   
                                                Image candidateImage = Image.FromStream(stream);
                                                pictureBox_candidateimage.Image = candidateImage;
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                        }
                                        string groupID = Marshal.PtrToStringAnsi(maxSimilarityPersonInfo.stPersonInfo.pszGroupID);
                                        string groupName = Marshal.PtrToStringAnsi(maxSimilarityPersonInfo.stPersonInfo.pszGroupName);
                                        this.BeginInvoke(new Action<NET_CANDIDATE_INFO, string, string>(UpdateCandidateProperty), maxSimilarityPersonInfo, groupID, groupName);
                                    }
                                }
                                else
                                {
                                    this.BeginInvoke(new Action(UpdateCandidateImage));
                                }
                            }
                        }
                        break;
                    case (uint)EM_EVENT_IVS_TYPE.FACEDETECT:
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                this.groupBox_globalimage.Text = "GlobalScene_Image(全景图) - FaceDetect(人脸检测)";
                                pictureBox_candidateimage.Image = null;
                                pictureBox_candidateimage.Refresh();
                                this.label_name.Text = "";
                                this.label_id.Text = "";
                                this.label_candidate_sex.Text = "";
                                this.label_birthday.Text = "";
                                this.label_similarity.Text = "";
                                this.label_groupid.Text = "";
                                this.label_groupname.Text = "";
                            }));
                            NET_DEV_EVENT_FACEDETECT_INFO info = (NET_DEV_EVENT_FACEDETECT_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_FACEDETECT_INFO));
                            if (m_GroupID != info.stuObject.nRelativeID)
                            {
                                m_GroupID = info.stuObject.nObjectID;
                                byte[] globalScenePicInfo = new byte[dwBufSize];
                                Marshal.Copy(pBuffer, globalScenePicInfo, 0, (int)dwBufSize);
                                using (MemoryStream stream = new MemoryStream(globalScenePicInfo))
                                {
                                    try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                                    {
                                        Image globalSceneImage = Image.FromStream(stream);
                                        pictureBox_image.Image = globalSceneImage;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }
                            }
                            else
                            {
                                byte[] personFaceInfo = new byte[dwBufSize];
                                Marshal.Copy(pBuffer, personFaceInfo, 0, (int)dwBufSize);
                                using (MemoryStream stream = new MemoryStream(personFaceInfo))
                                {
                                    try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                                    {
                                        Image faceImage = Image.FromStream(stream);
                                        pictureBox_faceimage.Image = faceImage;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }
                                this.BeginInvoke(new Action(() => 
                                {
                                    this.label_time.Text = info.UTC.ToShortString();
                                    this.label_face_sex.Text = PraseSex(info.emSex);
                                    if (info.nAge <= 0)
                                    {
                                        this.label_age.Text = "Unknown(未知)";
                                    }
                                    else
                                    {
                                        this.label_age.Text = info.nAge.ToString();
                                    }
                                    this.label_race.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_RACE_TYPE), info.emRace).ToLower() + PraseRace(info.emRace));
                                    this.label_eye.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_EYE_STATE_TYPE), info.emEye).ToLower() + PraseEye(info.emEye));
                                    this.label_mouth.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_MOUTH_STATE_TYPE), info.emMouth).ToLower() + PraseMouth(info.emMouth));
                                    this.label_mask.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_MASK_STATE_TYPE), info.emMask).ToLower() + PraseMask(info.emMask));
                                    this.label_beard.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_BEARD_STATE_TYPE), info.emBeard).ToLower() + PraseBeard(info.emBeard));
                                }));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return 0;
        }

        private void UpdateFaceProperty(NET_DEV_EVENT_FACERECOGNITION_INFO info)
        {
            this.label_time.Text = info.UTC.ToShortString();
            this.label_face_sex.Text = PraseSex(info.stuFaceData.emSex);
            if (info.stuFaceData.nAge <= 0)
            {
                this.label_age.Text = "Unknown(未知)";
            }
            else
            {
                this.label_age.Text = info.stuFaceData.nAge.ToString();
            }
            this.label_race.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_RACE_TYPE), info.stuFaceData.emRace).ToLower() + PraseRace(info.stuFaceData.emRace));
            this.label_eye.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_EYE_STATE_TYPE), info.stuFaceData.emEye).ToLower() + PraseEye(info.stuFaceData.emEye));
            this.label_mouth.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_MOUTH_STATE_TYPE), info.stuFaceData.emMouth).ToLower() + PraseMouth(info.stuFaceData.emMouth));
            this.label_mask.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_MASK_STATE_TYPE), info.stuFaceData.emMask).ToLower() + PraseMask(info.stuFaceData.emMask));
            this.label_beard.Text = m_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_BEARD_STATE_TYPE), info.stuFaceData.emBeard).ToLower() + PraseBeard(info.stuFaceData.emBeard));
        }

        private string PraseSex(EM_DEV_EVENT_FACEDETECT_SEX_TYPE emSex)
        {
            string res = "";
            switch (emSex)
            {
                case EM_DEV_EVENT_FACEDETECT_SEX_TYPE.UNKNOWN:
                    res = "Unknown(未知)";
                    break;
                case EM_DEV_EVENT_FACEDETECT_SEX_TYPE.MAN:
                    res = "Male(男)";
                    break;
                case EM_DEV_EVENT_FACEDETECT_SEX_TYPE.WOMAN:
                    res = "Female(女)";
                    break;
                default:
                    res = "Unknown(未知)";
                    break;
            }
            return res;
        }

        private string PraseRace(EM_RACE_TYPE emRace)
        {
            string res = "";
            switch (emRace)
            {
                case EM_RACE_TYPE.UNKNOWN:
                    res = "(未知)";
                    break;
                case EM_RACE_TYPE.NODISTI:
                    res = "(未识别)";
                    break;
                case EM_RACE_TYPE.YELLOW:
                    res = "(黄种人)";
                    break;
                case EM_RACE_TYPE.BLACK:
                    res = "(黑人)";
                    break;
                case EM_RACE_TYPE.WHITE:
                    res = "(白人)";
                    break;
                default:
                    res = "(未知)";
                    break;
            }
            return res;
        }

        private string PraseEye(EM_EYE_STATE_TYPE emEye)
        {
            string res = "";
            switch (emEye)
            {
                case EM_EYE_STATE_TYPE.UNKNOWN:
                    res = "(未知)";
                    break;
                case EM_EYE_STATE_TYPE.NODISTI:
                    res = "(未识别)";
                    break;
                case EM_EYE_STATE_TYPE.OPEN:
                    res = "(睁眼)";
                    break;
                case EM_EYE_STATE_TYPE.CLOSE:
                    res = "(闭眼)";
                    break;
                default:
                    res = "(未知)";
                    break;
            }
            return res;
        }

        private string PraseMouth(EM_MOUTH_STATE_TYPE emMouth)
        {
            string res = "";
            switch (emMouth)
            {
                case EM_MOUTH_STATE_TYPE.UNKNOWN:
                    res = "(未知)";
                    break;
                case EM_MOUTH_STATE_TYPE.NODISTI:
                    res = "(未识别)";
                    break;
                case EM_MOUTH_STATE_TYPE.OPEN:
                    res = "(张嘴)";
                    break;
                case EM_MOUTH_STATE_TYPE.CLOSE:
                    res = "(闭嘴)";
                    break;
                default:
                    res = "(未知)";
                    break;
            }
            return res;
        }

        private string PraseMask(EM_MASK_STATE_TYPE emMask)
        {
            string res = "";
            switch (emMask)
            {
                case EM_MASK_STATE_TYPE.UNKNOWN:
                    res = "(未知)";
                    break;
                case EM_MASK_STATE_TYPE.NODISTI:
                    res = "(未识别)";
                    break;
                case EM_MASK_STATE_TYPE.NOMASK:
                    res = "(没戴口罩)";
                    break;
                case EM_MASK_STATE_TYPE.WEAR:
                    res = "(戴口罩)";
                    break;
                default:
                    res = "(未知)";
                    break;
            }
            return res;
        }

        private string PraseBeard(EM_BEARD_STATE_TYPE emBeard)
        {
            string res = "";
            switch (emBeard)
            {
                case EM_BEARD_STATE_TYPE.UNKNOWN:
                    res = "(未知)";
                    break;
                case EM_BEARD_STATE_TYPE.NODISTI:
                    res = "(未识别)";
                    break;
                case EM_BEARD_STATE_TYPE.NOBEARD:
                    res = "(没胡子)";
                    break;
                case EM_BEARD_STATE_TYPE.HAVEBEARD:
                    res = "(有胡子)";
                    break;
                default:
                    res = "(未知)";
                    break;
            }
            return res;
        }

        private void UpdateCandidateImage()
        {
            pictureBox_candidateimage.Image = null;
            pictureBox_candidateimage.Refresh();
            this.label_name.Text = "";
            this.label_id.Text = "";
            this.label_candidate_sex.Text = "";
            this.label_birthday.Text = "";
            this.label_similarity.Text = "Stranger(陌生人)";
            this.label_groupid.Text = "";
            this.label_groupname.Text = "";
        }

        private void UpdateCandidateProperty(NET_CANDIDATE_INFO info, string groupID, string groupName)
        {
            this.label_name.Text = info.stPersonInfo.szPersonNameEx;
            if (1 == info.stPersonInfo.bySex)
            {
                this.label_candidate_sex.Text = "Male(男)";
            }
            else if (2 == info.stPersonInfo.bySex)
            {
                this.label_candidate_sex.Text = "Female(女)";
            }
            else
            {
                this.label_candidate_sex.Text = "Uknown(未知)";
            }
            this.label_birthday.Text = string.Format("{0}-{1}-{2}", info.stPersonInfo.wYear.ToString("D4"), info.stPersonInfo.byMonth.ToString("D2"), info.stPersonInfo.byDay.ToString("D2"));
            this.label_similarity.Text = info.bySimilarity.ToString();
            this.label_id.Text = info.stPersonInfo.szID;
            this.label_groupname.Text = groupName;
            this.label_groupid.Text = groupID;
        }

        private void button_operate_Click(object sender, EventArgs e)
        {
            OperateDBWindow operateWindow = new OperateDBWindow(this, m_DevInfo.nChanNum, m_LoginID);
            operateWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }

        private void button_realplay_Click(object sender, EventArgs e)
        {
            if (m_PlayID == IntPtr.Zero)
            {
                m_PlayID = NETClient.RealPlay(m_LoginID, comboBox_channel.SelectedIndex, pictureBox_realplay.Handle);
                if (IntPtr.Zero == m_PlayID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
				bool res = NETClient.RenderPrivateData(m_PlayID, true);
                if (!res)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_realplay.Text = "StopRealPlay(停止监视)";
            }
            else
            {
				bool ret = NETClient.RenderPrivateData(m_PlayID, false);
				if (!ret)
				{
					MessageBox.Show(NETClient.GetLastError());
					return;
				}
                ret = NETClient.StopRealPlay(m_PlayID);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                m_PlayID = IntPtr.Zero;
                this.button_realplay.Text = "RealPlay(监视)";
                this.pictureBox_realplay.Refresh();
            }
        }
    }
}

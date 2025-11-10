// MasterSlaverSensorDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MasterSlaverSensor.h"
#include "MasterSlaverSensorDlg.h"
#include "dhnetsdk.h"
#include "dhconfigsdk.h"
#include "dhplay.h"
#include "Utils.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMasterSlaver_watcherDlg dialog
#define PLAYPORT 450
#define WM_MASTERSLAVER_POINT (WM_USER + 112)
#define WM_EVENT_RECONNECT (WM_USER + 113)
#define WM_EVENT_DISCONNECT (WM_USER + 114)

struct MasterSlaverPoint
{
	RECT WinRect;
	CPoint pointStart;
	CPoint pointEnd;
	int nID;
};

CMasterSlaver_watcherDlg::CMasterSlaver_watcherDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMasterSlaver_watcherDlg::IDD, pParent)
	, m_Zs1(0)
	, m_Zs2(0)
	, m_Zs4(0)
	, m_Zs3(0)
{
	//{{AFX_DATA_INIT(CMasterSlaver_watcherDlg)
	m_right = 0;
	m_left = 0;
	m_top = 0;
	m_buttom = 0;
	m_Ym1 = 0;
	m_Xm1 = 0;
	m_Xs1 = 0;
	m_Ys1 = 0;
	m_Xm2 = 0;
	m_Ym2 = 0;
	m_Xs2 = 0;
	m_Ys2 = 0;
	m_Xm3 = 0;
	m_Ym3 = 0;
	m_Xs3 = 0;
	m_Ys3 = 0;
	m_Xm4 = 0;
	m_Ym4 = 0;
	m_Xs4 = 0;
	m_Ys4 = 0;
	m_method = 0;
	m_nRectDirection = 0;

	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_lLoginID = 0;
	m_lPlayID = 0;
	m_CurScreen =0;
	m_ChnCount = 0;//通道个数
	m_nGroupCount = 0;
	m_Zoomchn = 0;
	for(int i=0;i<16;i++)
	{
		m_DispHanle[i]=0;
		m_DispChn[i] = 0;
	}
	for(int j = 0; j<3; j++)
	{
		m_Group[j] = 0;
	}
	
	memset(m_szSlaveSerial, 0, MASTERSLAVE_DEV_SERIALNO_LEN);
	memset(m_szMasterSerial, 0, MASTERSLAVE_DEV_SERIALNO_LEN);
	m_nCalibrateDone = 0;
	m_lPlayID_REMOTE = 0;

	memset(&m_stuPointPairEx, 0, 8*sizeof(NET_POINT_PAIR_EX));
	m_nGroupNum = 4;
	m_nSensorNum = 0;
	m_nCalibrateNum = 0;
	m_lLoginID_REMOTE = 0;
	m_lLoginID = 0;
	memset(m_nSelceCout, 0, sizeof(m_nSelceCout));

	memset(&m_stuOutCalibrateConfig, 0, sizeof(m_stuOutCalibrateConfig));
	m_stuOutCalibrateConfig.dwSize = sizeof(m_stuOutCalibrateConfig);
	m_stuOutCalibrateConfig.nMaxCalibElement = 4;
	m_stuOutCalibrateConfig.pstCaliMatrixElementEx = new NET_CALIBRATE_MATRIX_ELEMENT_EX;
	memset(m_stuOutCalibrateConfig.pstCaliMatrixElementEx, 0, sizeof(NET_CALIBRATE_MATRIX_ELEMENT_EX));

}

CMasterSlaver_watcherDlg::~CMasterSlaver_watcherDlg()
{
	// TODO: Add your message handler code here
	if (m_lLoginID)
	{
		CLIENT_Logout(m_lLoginID);
	}

	if(m_lPlayID)
	{
		CLIENT_StopRealPlayEx(m_lPlayID);
	}

	if (m_lLoginID_REMOTE)
	{
		CLIENT_Logout(m_lLoginID_REMOTE);
	}

	if(m_lPlayID_REMOTE)
	{
		CLIENT_StopRealPlayEx(m_lPlayID_REMOTE);
	}
	UnInitNetSDK();
}

void CMasterSlaver_watcherDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMasterSlaver_watcherDlg)
	DDX_Control(pDX, IDC_COMBO_PTZDATA, m_comboPTZData);
	DDX_Control(pDX, IDC_COMBO_CALIBRATE_SELECT, m_cmbSelectNumber);
	DDX_Control(pDX, IDC_COMBO_CALIBRATE_SELECT2, m_cmbSelectGroup);
	DDX_Control(pDX, IDC_COMBO_SCREEN, m_comboDispNum);
	DDX_Control(pDX, IDC_COMBO_CHN, m_ctlChn);
	DDX_Control(pDX, IDC_DVRIPADDRESS, m_DvrIPAddr);
	DDX_Control(pDX, IDC_PTZIPADDRESS, m_PTZIPAddr);
	DDX_Text(pDX, IDC_EDIT_PWD, m_csPassWord);
	DDX_Text(pDX, IDC_EDIT_USER, m_csUser);
	DDX_Text(pDX, IDC_EDIT_REMOTE_PASSWORD, m_csRemotePassword);
	DDX_Text(pDX, IDC_EDIT_REMOTE_USER, m_csRemoteUse);
	DDX_Control(pDX, IDC_PTZ_RIGHTDOWN, m_ptz_rightdown);
	DDX_Control(pDX, IDC_PTZ_RIGHTUP, m_ptz_rightup);
	DDX_Control(pDX, IDC_PTZ_LEFTDOWN, m_ptz_leftdown);
	DDX_Control(pDX, IDC_PTZ_LEFTUP, m_ptz_leftup);
	DDX_Control(pDX, IDC_PTZ_RIGHT, m_ptz_right);
	DDX_Control(pDX, IDC_PTZ_LEFT, m_ptz_left);
	DDX_Control(pDX, IDC_PTZ_DOWN, m_ptz_down);
	DDX_Control(pDX, IDC_PTZ_UP, m_ptz_up);
	DDX_Control(pDX, IDC_ZOOM_TELE, m_zoom_tele);
	DDX_Control(pDX, IDC_ZOOM_WIDE, m_zoom_wide);
	DDX_Text(pDX, IDC_EDIT4, m_Ym1);
	DDX_Text(pDX, IDC_EDIT3, m_Xm1);
	DDX_Text(pDX, IDC_EDIT5, m_Xs1);
	DDX_Text(pDX, IDC_EDIT6, m_Ys1);
	DDX_Text(pDX, IDC_EDIT7, m_Xm2);
	DDX_Text(pDX, IDC_EDIT8, m_Ym2);
	DDX_Text(pDX, IDC_EDIT9, m_Xs2);
	DDX_Text(pDX, IDC_EDIT10, m_Ys2);
	DDX_Text(pDX, IDC_EDIT11, m_Xm3);
	DDX_Text(pDX, IDC_EDIT12, m_Ym3);
	DDX_Text(pDX, IDC_EDIT13, m_Xs3);
	DDX_Text(pDX, IDC_EDIT14, m_Ys3);
	DDX_Text(pDX, IDC_EDIT15, m_Xm4);
	DDX_Text(pDX, IDC_EDIT16, m_Ym4);
	DDX_Text(pDX, IDC_EDIT17, m_Xs4);
	DDX_Text(pDX, IDC_EDIT18, m_Ys4);
	DDX_Text(pDX, IDC_EDIT35, m_Zs1);
	DDX_Text(pDX, IDC_EDIT36, m_Zs2);
	DDX_Text(pDX, IDC_EDIT38, m_Zs4);
	DDX_Text(pDX, IDC_EDIT37, m_Zs3);
}

BEGIN_MESSAGE_MAP(CMasterSlaver_watcherDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_LOGIN, &CMasterSlaver_watcherDlg::OnButtonLogin)
	ON_BN_CLICKED(IDC_BUTTON_PLAY, &CMasterSlaver_watcherDlg::OnButtonPlay)
	ON_BN_CLICKED(IDC_BUTTON_LOGOUT, &CMasterSlaver_watcherDlg::OnButtonLogout)
	ON_BN_CLICKED(IDC_BUTTON_STOPPLAY, &CMasterSlaver_watcherDlg::OnButtonStopplay)
	ON_BN_CLICKED(IDC_BUTTON_ADD_REMOTE, &CMasterSlaver_watcherDlg::OnButtonAddRemote)
	ON_BN_CLICKED(IDC_BUTTON_BOND, &CMasterSlaver_watcherDlg::OnButtonBond)
	ON_BN_CLICKED(IDC_BUTTON_LOGINREMOTE, &CMasterSlaver_watcherDlg::OnButtonLoginremote)
	ON_BN_CLICKED(IDC_BUTTON_REMOTELOGOUT, &CMasterSlaver_watcherDlg::OnButtonRemotelogout)
	ON_BN_CLICKED(IDC_BUTTON_REMOTEPLAY, &CMasterSlaver_watcherDlg::OnButtonRemoteplay)
	ON_BN_CLICKED(IDC_BUTTON_REMOTESTOP, &CMasterSlaver_watcherDlg::OnButtonRemotestop)
	ON_BN_CLICKED(IDC_BUTTON_CALIBRATE_EACHSENSOR, &CMasterSlaver_watcherDlg::OnButtonCalibrateEachsensor)
	ON_BN_CLICKED(IDC_BUTTON_DONE, &CMasterSlaver_watcherDlg::OnButtonDone)
	ON_BN_CLICKED(IDC_BUTTON_new, &CMasterSlaver_watcherDlg::OnBUTTONnew)
	ON_BN_CLICKED(IDC_GET_SLAVEPOSITON1, &CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon1)
	ON_BN_CLICKED(IDC_GET_SLAVEPOSITON2, &CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon2)
	ON_BN_CLICKED(IDC_GET_SLAVEPOSITON3, &CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon3)
	ON_BN_CLICKED(IDC_GET_SLAVEPOSITON4, &CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon4)
	ON_CBN_SELCHANGE(IDC_COMBO_CALIBRATE_SELECT2, &CMasterSlaver_watcherDlg::OnSelchangeChannel)
	ON_BN_CLICKED(IDC_BUTTON_STARTCALIBRATE, &CMasterSlaver_watcherDlg::OnBnClickedButtonStartcalibrate)
	ON_MESSAGE(WM_MASTERSLAVER_POINT, &CMasterSlaver_watcherDlg::OnMasterSlaverPoint)
	ON_MESSAGE(WM_EVENT_RECONNECT, &CMasterSlaver_watcherDlg::OnDeviceReconnect)
	ON_MESSAGE(WM_EVENT_DISCONNECT, &CMasterSlaver_watcherDlg::OnDeviceDisConnect)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMasterSlaver_watcherDlg message handlers
void CALLBACK DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	if(0 != dwUser)
	{
		CMasterSlaver_watcherDlg *pDlgInstance = (CMasterSlaver_watcherDlg *)dwUser;
		HWND hwnd = pDlgInstance->GetSafeHwnd();
		::PostMessage(hwnd, WM_EVENT_DISCONNECT, 0, 0);	
	}

}

void CALLBACK HaveReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	if(0 != dwUser)
	{
		CMasterSlaver_watcherDlg *pDlgInstance = (CMasterSlaver_watcherDlg *)dwUser;
		HWND hwnd = pDlgInstance->GetSafeHwnd();
		::PostMessage(hwnd, WM_EVENT_RECONNECT, 0, 0);
	}
}

BOOL CMasterSlaver_watcherDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
	m_ptzScreen.Create(NULL,NULL,WS_CHILD|WS_VISIBLE,CRect(0,0,0,0),this,1981);
    m_ptzScreen.ShowWindow(SW_SHOW);
    m_ptzScreen.SetCallBack(RectEventFunc,(DWORD)this, IDC_SCREEN_WINDOW);

	m_ptzScreen.SetShowPlayWin(SPLIT1,0);

	m_ptzScreen1.Create(NULL,NULL,WS_CHILD|WS_VISIBLE,CRect(0,0,0,0),this,1981);
    m_ptzScreen1.ShowWindow(SW_SHOW);
    m_ptzScreen1.SetCallBack(RectEventFunc,(DWORD)this,IDC_DOME_WINDOW);
 	
 	m_ptzScreen1.SetShowPlayWin(SPLIT1,0);
	UpdataScreenPos();
	InitComboBox();
	InitPTZControl();

	g_SetWndStaticText(this);

	SetDlgItemText(IDC_DVRIPADDRESS, "10.11.16.88");
	SetDlgItemText(IDC_EDIT_PORT, "37777");
	SetDlgItemText(IDC_EDIT_USER, "admin");
	SetDlgItemText(IDC_EDIT_PWD, "123admin123");

	SetDlgItemText(IDC_PTZIPADDRESS, "10.11.16.90");
	SetDlgItemText(IDC_EDIT_REMOTE_PORT, "37777");
	SetDlgItemText(IDC_EDIT_REMOTE_USER, "admin");
	SetDlgItemText(IDC_EDIT_REMOTE_PASSWORD, "1234admin");

	GetDlgItem(IDC_BUTTON_ADD_REMOTE)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_ADDOTHER)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_BOND)->EnableWindow(FALSE);
	GetDlgItem(IDC_GET_SLAVEPOSITON1)->EnableWindow(FALSE);
	GetDlgItem(IDC_GET_SLAVEPOSITON2)->EnableWindow(FALSE);
	GetDlgItem(IDC_GET_SLAVEPOSITON3)->EnableWindow(FALSE);
	GetDlgItem(IDC_GET_SLAVEPOSITON4)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_CALIBRATE_EACHSENSOR)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_DONE)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_new )->EnableWindow(FALSE);

	CLIENT_Init(DisConnectFunc, DWORD(this));	//SDK初始化
	CLIENT_SetAutoReconnect(HaveReConnectFunc, (LDWORD)this);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

LRESULT CMasterSlaver_watcherDlg::OnDeviceReconnect(WPARAM wParam, LPARAM lParam)
{	
	SetWindowText(ConvertString(_T("MasterSlaverSensor")));
	return 0;
}

LRESULT CMasterSlaver_watcherDlg::OnDeviceDisConnect(WPARAM wParam, LPARAM lParam)
{
	SetWindowText(ConvertString(_T("Network disconnected!")));
	return 0;
}

void CMasterSlaver_watcherDlg::OnDestroy() 
{
	CDialog::OnDestroy();

	// TODO: Add your message handler code here
	OnButtonLogout();
	OnButtonRemotelogout();
	UnInitNetSDK();
}

void CMasterSlaver_watcherDlg::UnInitNetSDK()
{
	CLIENT_Cleanup();
}

void CMasterSlaver_watcherDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMasterSlaver_watcherDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMasterSlaver_watcherDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CMasterSlaver_watcherDlg::UpdataScreenPos()
{
	GetDlgItem(IDC_SCREEN_WINDOW)->GetWindowRect(&m_clientRect);
	ScreenToClient(&m_clientRect);
	m_screenRect = m_clientRect;
	m_ptzScreen.MoveWindow(m_screenRect);

	GetDlgItem(IDC_DOME_WINDOW)->GetWindowRect(&m_clientRect);
	ScreenToClient(&m_clientRect);
	m_screenRect = m_clientRect;
 	m_ptzScreen1.MoveWindow(m_screenRect);
}

void CMasterSlaver_watcherDlg::SetCalibrateValue(int nGroup, int nID)
{
	int nWidth = 0;
	int nLeft = 0;
		
	if(0 == nGroup) // 配置第一组的配置
	{
		if(IDC_SCREEN_WINDOW == nID)
		{
			m_Xm1 = m_left;
			m_Ym1 = m_top;
		}	

		if(IDC_DOME_WINDOW == nID)
		{
			m_Xs1 = m_right;
			m_Ys1 = m_buttom;
		}
	}
	if(1 == nGroup) // 配置第二组的配置
	{
		
		if(IDC_SCREEN_WINDOW == nID)
		{
			m_Xm2 = m_left;
			m_Ym2 = m_top;
		}	

		if(IDC_DOME_WINDOW == nID)
		{
			m_Xs2 = m_right;
			m_Ys2 = m_buttom;
		}	
	}
	if(2 == nGroup) 
	{
		
		if(IDC_SCREEN_WINDOW == nID)
		{
			m_Xm3 = m_left;
			m_Ym3 = m_top;
		}

		if(IDC_DOME_WINDOW == nID)
		{
			m_Xs3 = m_right;
			m_Ys3 = m_buttom;
		}
	}
	if(3 == nGroup) 
	{
		
		if(IDC_SCREEN_WINDOW == nID)
		{
			m_Xm4 = m_left;
			m_Ym4 = m_top;
		}

		if(IDC_DOME_WINDOW == nID)
		{
			m_Xs4 = m_right;
			m_Ys4 = m_buttom;
		}
	}
	UpdateData(FALSE);
	
}

void CALLBACK RectEventFunc(int nWndID,RECT WinRect,CPoint &pointStart,CPoint &pointEnd,DWORD dwUser,int nID)
{
	if(dwUser == 0)
	{
		return;
	}

	CMasterSlaver_watcherDlg *dlg =(CMasterSlaver_watcherDlg *)dwUser;
	HWND hwnd = dlg->GetSafeHwnd();
	dlg->RectEvent(nWndID,WinRect,pointStart,pointEnd,nID,hwnd);
}

void CMasterSlaver_watcherDlg::RectEvent(int nWndID,RECT WinRect,CPoint &pointStart,CPoint &pointEnd,int nID,HWND hwnd)
{
	if (m_lLoginID == NULL || m_lLoginID_REMOTE == NULL)
	{
		return;
	}

	MasterSlaverPoint *pInfo = new MasterSlaverPoint;
	pInfo->nID = nID;
	pInfo->pointEnd = pointEnd;
	pInfo->pointStart = pointStart;
	pInfo->WinRect = WinRect;
	
	::PostMessage(hwnd,WM_MASTERSLAVER_POINT,WPARAM(pInfo),0);
}

LRESULT CMasterSlaver_watcherDlg::OnMasterSlaverPoint(WPARAM wParam, LPARAM lParam)
{
	if (wParam == NULL)
	{
		return 0;
	}

	MasterSlaverPoint *pInfo = (MasterSlaverPoint*)wParam;

	int width  = pInfo->WinRect.right - pInfo->WinRect.left;
	int height = pInfo->WinRect.bottom - pInfo->WinRect.top;

	//点击时，start与end值相等，联动调用EM_MSGROUP_OPERATE_LOCATE
	if(pInfo->pointStart.x == pInfo->pointEnd.x)
	{
		m_method = 0; // 点击联动标志
	}
	else
	{
		m_method = 1; // 点击联动标志(复选)
		if (pInfo->pointStart.y - pInfo->pointEnd.y > 0)
		{
			m_nRectDirection = 0;
		}
		else
		{
			m_nRectDirection = 1;
		}
	}

	// 框选时下发四点坐标，点击时下发left和top坐标
	m_left = pInfo->pointStart.x * 8192/width;
	m_top = pInfo->pointStart.y * 8192/height;

	m_right = pInfo->pointEnd.x * 8192/width;
	m_buttom = pInfo->pointEnd.y * 8192/height;
	
	if(0 == m_nCalibrateDone)// 未标定完成
	{
		int nIndex = 0;
		nIndex = m_cmbSelectNumber.GetCurSel();
		int nGroupSelect = m_cmbSelectNumber.GetItemData(nIndex);
		SetCalibrateValue(nGroupSelect, pInfo->nID);

	}
	if(1 == m_nCalibrateDone)// 标定完成
	{
		if(IDC_SCREEN_WINDOW == pInfo->nID)
		{
			NET_IN_MSGROUP_OPEN_INFO stIn = {sizeof(stIn), 0}; // 参数2位绑定时指定的组号
			NET_OUT_MSGROUP_OPEN_INFO stOut = {sizeof(stOut)};
			int nToken = 0;
			if(!CLIENT_OperateMasterSlaveGroup(m_lLoginID, EM_MSGROUP_OPERATE_OPEN, &stIn, &stOut, 3000))
			{
				MessageBox(ConvertString("open group failed"),ConvertString("prompt"));
			}
			else
			{
				nToken = stOut.dwToken;
			}

			if(m_method == 0) // 执行点击联动
			{
				NET_IN_MSGROUP_LOCATE_INFO stIn = {sizeof(stIn)};
				NET_OUT_MSGROUP_LOCATE_INTO stOut = {sizeof(stOut)};
				stIn.dwToken= nToken;
				stIn.stuPoint.nx = m_left;
				stIn.stuPoint.ny = m_top;

				if(!CLIENT_OperateMasterSlaveGroup(m_lLoginID,EM_MSGROUP_OPERATE_LOCATE,&stIn,&stOut,3000))
				{
					MessageBox(ConvertString("let move failed"),ConvertString("prompt"));
				}
			}
			else if (m_method == 1)
			{
				NET_IN_MSGROUP_RECTLOCATE_INFO stIn = {sizeof(stIn)};
				NET_OUT_MSGROUP_RECTLOCATE_INFO stOut = {sizeof(stOut)};
				stIn.dwToken = nToken;
				stIn.stuRect.nRight = m_right;
				stIn.stuRect.nLeft = m_left;
				stIn.stuRect.nTop = m_top;
				stIn.stuRect.nBottom = m_buttom;
				stIn.nRectDirection = m_nRectDirection;

				if(!CLIENT_OperateMasterSlaveGroup(m_lLoginID,EM_MSGROUP_OPERATE_RECTLOCATE,&stIn,&stOut,3000))
				{
					MessageBox(ConvertString("let move failed"),ConvertString("prompt"));
				}
			}
		}
	}
	delete pInfo;
	pInfo = NULL;
	return 0;
}

void CMasterSlaver_watcherDlg::OnButtonLogin() 
{
	// TODO: Add your control notification handler code here
	// 登陆枪机
	UpdateData(TRUE);
	NET_DEVICEINFO_Ex netDevInfo = {0};
	CString strIp;
	CString strUser;
	CString strPwd;
	int nError = 0;
	int nPort = 0;

	GetDlgItemText(IDC_DVRIPADDRESS, strIp);
	GetDlgItemText(IDC_EDIT_USER, strUser);
	GetDlgItemText(IDC_EDIT_PWD, strPwd);
	nPort = GetDlgItemInt(IDC_EDIT_PORT);

	char *pcIP = (LPSTR)(LPCSTR)strIp;
	char *pcUser = (LPSTR)(LPCSTR)strUser;
	char *pcPwssd = (LPSTR)(LPCSTR)strPwd;

	m_lLoginID = CLIENT_LoginEx2(pcIP,nPort,pcUser,pcPwssd,EM_LOGIN_SPEC_CAP_TCP,NULL,&netDevInfo,&nError);
	if(m_lLoginID > 0)
	{
		((CButton *)GetDlgItem(IDC_BUTTON_LOGIN))->EnableWindow(FALSE);
		m_ctlChn.EnableWindow(TRUE);
		m_ctlChn.ResetContent();
		CString strChn;
		m_ChnCount = netDevInfo.nChanNum;
		for(int i = 0; i < m_ChnCount; i++)
		{
			strChn.Format("%d", i);
			m_ctlChn.SetItemData(m_ctlChn.AddString(strChn), i); 
		}
		
		m_ctlChn.SetCurSel(0);	
		
		// 获取枪机序列号
		DHDEV_SYSTEM_ATTR_CFG pDevConfigInfo = {0};
		DWORD dwRetLen = 0;
		BOOL bRet1 = CLIENT_GetDevConfig(m_lLoginID, DH_DEV_DEVICECFG, 0, &pDevConfigInfo, sizeof(DHDEV_SYSTEM_ATTR_CFG), &dwRetLen, 3000);
		if (bRet1 && dwRetLen == sizeof(DHDEV_SYSTEM_ATTR_CFG))
		{		
			memcpy(m_szMasterSerial, pDevConfigInfo.szDevSerialNo, DH_DEV_SERIALNO_LEN);
		}
		else
		{
			MessageBox(ConvertString("get failed"), ConvertString("Prompt"));
		}

		if (m_lLoginID && m_lLoginID_REMOTE)
		{
			GetDlgItem(IDC_BUTTON_ADD_REMOTE)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_ADDOTHER)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_BOND)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON1)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON2)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON3)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON4)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_new )->EnableWindow(TRUE);
			DlgEnableWindow(TRUE);
		}
	}
	else
	{
		ShowLoginErrorReason(nError);
		
	}
}

void CMasterSlaver_watcherDlg::ShowLoginErrorReason(int nError)
{
	if(1 == nError)	    	MessageBox(ConvertString("Invalid password!"), ConvertString("Prompt"));
	else if(2 == nError)	MessageBox(ConvertString("Invalid account!"), ConvertString("Prompt"));
	else if(3 == nError)	MessageBox(ConvertString("Timeout!"), ConvertString("Prompt"));
	else if(4 == nError)	MessageBox(ConvertString("The user has logged in!"), ConvertString("Prompt"));
	else if(5 == nError)	MessageBox(ConvertString("The user has been locked!"), ConvertString("Prompt"));
	else if(6 == nError)	MessageBox(ConvertString("The user has listed into illegal!"), ConvertString("Prompt"));
	else if(7 == nError)	MessageBox(ConvertString("The system is busy!"), ConvertString("Prompt"));
	else if(9 == nError)	MessageBox(ConvertString("You Can't find the network server!"), ConvertString("Prompt"));
	else	MessageBox(ConvertString("Login failed!"), ConvertString("Prompt"));
}

void CMasterSlaver_watcherDlg::InitComboBox()
{
	//Video dropdown menu initialization 
	int nIndex;
	int i = 0;
	CString strDispNum[16]={"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16"};
	m_comboDispNum.ResetContent();
	for(i=0;i<16;i++)
	{
		nIndex = m_comboDispNum.AddString(strDispNum[i]);
		m_comboDispNum.SetItemData(nIndex,i+1);
	}
	m_comboDispNum.SetCurSel(0);

	m_cmbSelectNumber.ResetContent();
	m_cmbSelectGroup.ResetContent();

	//Control parameter dropdown menu initialization 
	CString strPTZData[8]={"1","2","3","4","5","6","7","8"};
	m_comboPTZData.ResetContent();
	for(i=0;i<8;i++)
	{
		nIndex = m_comboPTZData.AddString(strPTZData[i]);
		m_comboPTZData.SetItemData(nIndex,i+1);
	}
	m_comboPTZData.SetCurSel(3);
	
	for(int k = 0; k < 4; k++)
	{
		CString strChn;
		strChn.Format("%d", k+1);
		m_cmbSelectNumber.SetItemData(m_cmbSelectNumber.AddString(strChn), k); 
		
	}
	m_cmbSelectNumber.SetCurSel(0);

	for(int j = 0; j < 4; j++)
	{
		CString strChn1;
		strChn1.Format("%d", j+1);
		m_cmbSelectGroup.SetItemData(m_cmbSelectGroup.AddString(strChn1), j); 

	}
	m_cmbSelectGroup.SetCurSel(0);


}

void CMasterSlaver_watcherDlg::CloseDispVideo(int iDispNum)
{
	if(0 != m_DispHanle[iDispNum-1])
	{
		BOOL bRet = CLIENT_StopRealPlayEx(m_DispHanle[iDispNum-1]);

		BOOL bPlay = PLAY_Stop(PLAYPORT);
		if(bPlay)
		{
			//At last close PLAY_OpenStream
			BOOL bStream = PLAY_CloseStream(PLAYPORT);
		}
		else
		{
			MessageBox(ConvertString("STOP FAILED!"),ConvertString("prompt"));
		}
	}
}

void CMasterSlaver_watcherDlg::OnButtonPlay() 
{
	// TODO: Add your control notification handler code here
	// 播放
	DH_RealPlayType emRealPlayType =  DH_RType_Realplay;   // real-time monitoring    
	HWND hWnd = GetDlgItem(IDC_SCREEN_WINDOW)->m_hWnd;
	m_lPlayID = CLIENT_RealPlayEx(m_lLoginID, 0, hWnd, emRealPlayType);
	if(0 != m_lPlayID)
	{
		((CButton *)GetDlgItem(IDC_BUTTON_PLAY))->EnableWindow(FALSE);//设置button状态
	}
	else
	{
		MessageBox(ConvertString("Fail to play!"), ConvertString("Prompt"));
	}
}

void CMasterSlaver_watcherDlg::ClearEdit()
{
	memset(m_nSelceCout, 0, sizeof(m_nSelceCout));
	m_cmbSelectNumber.SetCurSel(0);
	m_cmbSelectGroup.SetCurSel(0);
	GetDlgItem(IDC_EDIT3)->SetWindowText("0");
	GetDlgItem(IDC_EDIT4)->SetWindowText("0");
	GetDlgItem(IDC_EDIT5)->SetWindowText("0");
	GetDlgItem(IDC_EDIT6)->SetWindowText("0");
	GetDlgItem(IDC_EDIT7)->SetWindowText("0");
	GetDlgItem(IDC_EDIT8)->SetWindowText("0");
	GetDlgItem(IDC_EDIT9)->SetWindowText("0");
	GetDlgItem(IDC_EDIT10)->SetWindowText("0");
	GetDlgItem(IDC_EDIT11)->SetWindowText("0");
	GetDlgItem(IDC_EDIT12)->SetWindowText("0");
	GetDlgItem(IDC_EDIT13)->SetWindowText("0");
	GetDlgItem(IDC_EDIT14)->SetWindowText("0");
	GetDlgItem(IDC_EDIT15)->SetWindowText("0");
	GetDlgItem(IDC_EDIT16)->SetWindowText("0");
	GetDlgItem(IDC_EDIT17)->SetWindowText("0");
	GetDlgItem(IDC_EDIT18)->SetWindowText("0");
	GetDlgItem(IDC_EDIT35)->SetWindowText("0");
	GetDlgItem(IDC_EDIT36)->SetWindowText("0");
	GetDlgItem(IDC_EDIT37)->SetWindowText("0");
	GetDlgItem(IDC_EDIT38)->SetWindowText("0");
	UpdateData(TRUE);	
}

void CMasterSlaver_watcherDlg::OnButtonLogout() 
{
	// TODO: Add your control notification handler code here
	
	if(0 != m_lLoginID)
	{
		m_nCalibrateDone = 0;//重新标定
		//Clear 9-window real-time monitor handle 
		for(int i=0;i<16;i++)
		{
			CloseDispVideo(i + 1);
			Invalidate();
			m_DispHanle[i]=0;	
			m_DispChn[i]=0;
			m_ChnCount=0;
			m_nGroupCount=0;
		}
		
		BOOL bSuccess = CLIENT_Logout(m_lLoginID);
		if(bSuccess)
		{
			m_nCalibrateNum = 0;
			m_lLoginID = 0;
			ClearEdit();
			m_ctlChn.ResetContent();
			memset(&m_stuPointPairEx, 0, 8*sizeof(NET_POINT_PAIR_EX));
			((CButton *)GetDlgItem(IDC_BUTTON_LOGIN))->EnableWindow(TRUE);
			((CButton *)GetDlgItem(IDC_BUTTON_PLAY))->EnableWindow(TRUE);
			((CButton *)GetDlgItem(IDC_BUTTON_BOND))->EnableWindow(TRUE);
			((CButton *)GetDlgItem(IDC_BUTTON_ADD_REMOTE))->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_ADD_REMOTE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_ADDOTHER)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_BOND)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON1)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON2)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON3)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON4)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_CALIBRATE_EACHSENSOR)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_DONE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_new )->EnableWindow(FALSE);
		}
		else
		{
			MessageBox(ConvertString("Fail to Logout!"), ConvertString("Prompt"));
		}
	}
	
}

void CMasterSlaver_watcherDlg::OnButtonStopplay() 
{
	// TODO: Add your control notification handler code here
	if(m_lPlayID)
	{
		BOOL bRet = CLIENT_StopRealPlayEx(m_lPlayID);
		if(! bRet)
		{
			MessageBox(ConvertString("Stop RealPlay failed!"), ConvertString("Prompt"));
		}
		else
		{
			m_lPlayID = 0;
			Invalidate();
			((CButton *)GetDlgItem(IDC_BUTTON_PLAY))->EnableWindow(TRUE);//设置button状态
		}

	}
	else
	{
		MessageBox(ConvertString("please Play first"), ConvertString("Prompt"));
	}
	
}

void CMasterSlaver_watcherDlg::OnButtonAddRemote() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);
	char * szOut = new char[1024*32];
	memset(szOut, 0, 1024*32);
	AV_CFG_RemoteDevice stOut = {sizeof(stOut)};
	BOOL bRet = 0;
	CString strIp;

	GetDlgItemText(IDC_PTZIPADDRESS, strIp);
	char *pcIP = (LPSTR)(LPCSTR)strIp;

	//获取绑定关系 使用接口为CLIENT_GetNewDevConfig 和 CLIENT_ParseData
	bRet = CLIENT_GetNewDevConfig(m_lLoginID, CFG_CMD_REMOTEDEVICE, -1, szOut, 1024*32, NULL, 3000);
	if(bRet)
	{
		bRet = CLIENT_ParseData(CFG_CMD_REMOTEDEVICE, szOut, &stOut, sizeof(stOut), NULL);
		if(bRet)
		{
			stOut.bEnable = true;
			char szID[AV_CFG_Device_ID_Len] = "0";
			strncpy(stOut.szID, szID, AV_CFG_Device_ID_Len - 1);
	
			int nIPLen = strlen(pcIP);
			strncpy(stOut.szAddress, pcIP,sizeof(stOut.szAddress) - 1);
			strncpy(stOut.szIP, pcIP, sizeof(stOut.szIP) - 1);
			stOut.szAddress[nIPLen] = '\0';
			
			stOut.nPort = 5000; // 固定

			int nUserLen = strlen(m_csRemoteUse.GetBuffer(0));
			memcpy(stOut.szUser, m_csRemoteUse, nUserLen);
			stOut.szUser[nUserLen] = '\0';

			int nPassWordLen = strlen(m_csRemotePassword.GetBuffer(0));
			memcpy(stOut.szPassword, m_csRemotePassword, nPassWordLen);
			stOut.szPassword[nPassWordLen] = '\0';

			char szProtol[8] = "Private";
			memcpy(stOut.szProtocol, szProtol, 8);

			memcpy(stOut.szSerial, m_szSlaveSerial, AV_CFG_Serial_Len);

			stOut.nLoginType = 0;
	
			//设置绑定关系 使用接口为CLIENT_PacketData 和 CLIENT_SetNewDevConfig
			bRet = CLIENT_PacketData(CFG_CMD_REMOTEDEVICE, &stOut, sizeof(stOut), szOut, 1024*32);
			if(bRet)
			{
				bRet = CLIENT_SetNewDevConfig(m_lLoginID, CFG_CMD_REMOTEDEVICE, -1, szOut, 1024*32, NULL, NULL, 3000);
				if(bRet)
				{
					((CButton *)GetDlgItem(IDC_BUTTON_ADD_REMOTE))->EnableWindow(FALSE);
				}
				else
				{
					MessageBox(ConvertString("set failed"), ConvertString("prompt"));
				}
			}
			else
			{
				MessageBox(ConvertString("packet failed"), ConvertString("prompt"));
			}
		}
		else
		{
			MessageBox(ConvertString("parse failed"), ConvertString("prompt"));
		}
		
	}
	else
	{
		MessageBox(ConvertString("get failed"), ConvertString("prompt"));
	}

	delete[] szOut;
}

void CMasterSlaver_watcherDlg::OnButtonBond() 
{
	// TODO: Add your control notification handler code here
	char * szOut = new char[1024*32];
	memset(szOut, 0, 1024*32);
	CFG_MASTERSLAVE_GROUP_INFO stOut = {0};
	//获取绑定关系 使用接口为CLIENT_GetNewDevConfig 和 CLIENT_ParseData
	BOOL bRet1 = CLIENT_GetNewDevConfig(m_lLoginID, CFG_CMD_MASTERSLAVE_GROUP, -1, szOut, 1024*32, NULL, 3000);
	if(bRet1)
	{
		bRet1 = CLIENT_ParseData(CFG_CMD_MASTERSLAVE_GROUP, szOut, &stOut, sizeof(stOut), NULL);
		if(bRet1)
		{
			strncpy(stOut.szName, "B3Plaza", MASTERSLAVE_NAME_LEN - 1);
			stOut.nGroupID = 0;
			stOut.emMasterType = EM_MASTER_CAMERA;
			stOut.nMasterNum = 1;
			stOut.nSlaverNum = 1;
			stOut.stuMasterList[0].nChannel = 0;
			strncpy(stOut.stuMasterList[0].szDevSerial, m_szMasterSerial, MASTERSLAVE_DEV_SERIALNO_LEN - 1);
			stOut.stuSlaverList[0].nChannel = 0;
			strncpy(stOut.stuSlaverList[0].szDevSerial, m_szSlaveSerial, MASTERSLAVE_DEV_SERIALNO_LEN - 1);

			//设置绑定关系 使用接口为CLIENT_PacketData 和 CLIENT_SetNewDevConfig
			bRet1 = CLIENT_PacketData(CFG_CMD_MASTERSLAVE_GROUP, &stOut, sizeof(stOut), szOut, 1024*32);
			if(bRet1)
			{
				bRet1 = CLIENT_SetNewDevConfig(m_lLoginID, CFG_CMD_MASTERSLAVE_GROUP, -1, szOut, 1024*32, NULL, NULL, 3000);
				if(bRet1)
				{
					((CButton *)GetDlgItem(IDC_BUTTON_BOND))->EnableWindow(FALSE);
				}
				else
				{
					MessageBox(ConvertString("set failed"), ConvertString("prompt"));
				}
			}
			else
			{
				MessageBox(ConvertString("packet failed"), ConvertString("prompt"));
			}
		}
		else
		{
			MessageBox(ConvertString("parse failed"), ConvertString("prompt"));
		}
		
	}
	else
	{
		MessageBox(ConvertString("get failed"), ConvertString("prompt"));
	}	
	delete[] szOut;
}

void CMasterSlaver_watcherDlg::OnButtonLoginremote() 
{
	// TODO: Add your control notification handler code here
	// 登陆球机
	UpdateData(TRUE);
	NET_DEVICEINFO_Ex netDevInfo = {0};

	CString strIp;
	CString strUser;
	CString strPwd;
	int		nPort;

	GetDlgItemText(IDC_PTZIPADDRESS, strIp);
	GetDlgItemText(IDC_EDIT_REMOTE_USER, strUser);
	GetDlgItemText(IDC_EDIT_REMOTE_PASSWORD, strPwd);
	nPort = GetDlgItemInt(IDC_EDIT_REMOTE_PORT);

	char *pcIP = (LPSTR)(LPCSTR)strIp;
	char *pcUser = (LPSTR)(LPCSTR)strUser;
	char *pcPwssd = (LPSTR)(LPCSTR)strPwd;
	int nError = 0;

	m_lLoginID_REMOTE = CLIENT_LoginEx2(pcIP,nPort,pcUser,pcPwssd,EM_LOGIN_SPEC_CAP_TCP,NULL,&netDevInfo,&nError);
	if(m_lLoginID_REMOTE > 0)
	{
		((CButton *)GetDlgItem(IDC_BUTTON_LOGINREMOTE))->EnableWindow(FALSE);
		// 获取球机序列号
		DHDEV_SYSTEM_ATTR_CFG pDevConfigInfo = {0};
		DWORD dwRetLen = 0;
		BOOL bRet1 = CLIENT_GetDevConfig(m_lLoginID_REMOTE, DH_DEV_DEVICECFG, 0, &pDevConfigInfo, sizeof(DHDEV_SYSTEM_ATTR_CFG), &dwRetLen, 3000);
		if (bRet1 && dwRetLen == sizeof(DHDEV_SYSTEM_ATTR_CFG))
		{		
			memcpy(m_szSlaveSerial, pDevConfigInfo.szDevSerialNo, DH_DEV_SERIALNO_LEN);
		}
		else
		{
			MessageBox(ConvertString("get failed"), ConvertString("Prompt"));
		}

		if (m_lLoginID && m_lLoginID_REMOTE)
		{
			GetDlgItem(IDC_BUTTON_ADD_REMOTE)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_ADDOTHER)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_BOND)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON1)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON2)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON3)->EnableWindow(TRUE);
			GetDlgItem(IDC_GET_SLAVEPOSITON4)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_new )->EnableWindow(TRUE);
			DlgEnableWindow(TRUE);
		}
	}
	else
	{
		ShowLoginErrorReason(nError);
	}	

}

void CMasterSlaver_watcherDlg::OnButtonRemotelogout() 
{
	// TODO: Add your control notification handler code here
	if(0 != m_lLoginID_REMOTE)
	{
		m_nCalibrateDone = 0;//重新标定
		//stop real-play if real-play is on
		if(m_lPlayID_REMOTE)
		{
			BOOL bRet = CLIENT_StopRealPlayEx(m_lPlayID_REMOTE);
			if(! bRet)
			{
				MessageBox(ConvertString("Stop RealPlay failed!"), ConvertString("Prompt"));
			}
			else
			{
				m_lPlayID_REMOTE = 0;
				Invalidate();
			}
			
		}
		
		BOOL bSuccess = CLIENT_Logout(m_lLoginID_REMOTE);
		if(bSuccess)
		{
			m_nCalibrateNum = 0;
			ClearEdit();
			m_lLoginID_REMOTE = 0;
			memset(&m_stuPointPairEx, 0, 8*sizeof(NET_POINT_PAIR_EX));
			((CButton *)GetDlgItem(IDC_BUTTON_REMOTEPLAY))->EnableWindow(TRUE);
			((CButton *)GetDlgItem(IDC_BUTTON_LOGINREMOTE))->EnableWindow(TRUE);
			GetDlgItem(IDC_BUTTON_ADD_REMOTE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_ADDOTHER)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_BOND)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON1)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON2)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON3)->EnableWindow(FALSE);
			GetDlgItem(IDC_GET_SLAVEPOSITON4)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_CALIBRATE_EACHSENSOR)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_DONE)->EnableWindow(FALSE);
			GetDlgItem(IDC_BUTTON_new )->EnableWindow(FALSE);
			UpdateData(FALSE);
		}
		else
		{
			MessageBox(ConvertString("Fail to Logout!"), ConvertString("Prompt"));
		}
	}
	
}

void CMasterSlaver_watcherDlg::OnButtonRemoteplay() 
{
	// TODO: Add your control notification handler code here
	// 播放
	DH_RealPlayType emRealPlayType =  DH_RType_Realplay;   // real-time monitoring    
	HWND hWnd = GetDlgItem(IDC_DOME_WINDOW)->m_hWnd;
	m_lPlayID_REMOTE = CLIENT_RealPlayEx(m_lLoginID_REMOTE, 0, hWnd, emRealPlayType);
	if(0 != m_lPlayID_REMOTE)
	{
		((CButton *)GetDlgItem(IDC_BUTTON_REMOTEPLAY))->EnableWindow(FALSE);//设置button状态
	}
	else
	{
		MessageBox(ConvertString("Fail to play!"), ConvertString("Prompt"));
	}
	
}

void CMasterSlaver_watcherDlg::OnButtonRemotestop() 
{
	// TODO: Add your control notification handler code here
	if(m_lPlayID_REMOTE)
	{
		BOOL bRet = CLIENT_StopRealPlayEx(m_lPlayID_REMOTE);
		if(! bRet)
		{
			MessageBox(ConvertString("Stop RealPlay failed!"), ConvertString("Prompt"));
		}
		else
		{
			m_lPlayID_REMOTE = 0;
			Invalidate();
			((CButton *)GetDlgItem(IDC_BUTTON_REMOTEPLAY))->EnableWindow(TRUE);//设置button状态
		}
		
	}
	else
	{
		MessageBox(ConvertString("please Play first"), ConvertString("Prompt"));
	}
	
}

void CMasterSlaver_watcherDlg::SetCalibrateBySensor(int nSensor)
{
	UpdateData(TRUE);

	m_stuPointPairEx[nSensor].stuPoint[0].stuMasterPoint.nx = m_Xm1;
	m_stuPointPairEx[nSensor].stuPoint[0].stuMasterPoint.ny = m_Ym1;
	m_stuPointPairEx[nSensor].stuPoint[0].stuSlavePoint.nx = m_Xs1;
	m_stuPointPairEx[nSensor].stuPoint[0].stuSlavePoint.ny = m_Ys1;

	m_stuPointPairEx[nSensor].stuPoint[1].stuMasterPoint.nx = m_Xm2;
	m_stuPointPairEx[nSensor].stuPoint[1].stuMasterPoint.ny = m_Ym2;
	m_stuPointPairEx[nSensor].stuPoint[1].stuSlavePoint.nx = m_Xs2;
	m_stuPointPairEx[nSensor].stuPoint[1].stuSlavePoint.ny = m_Ys2;

	m_stuPointPairEx[nSensor].stuPoint[2].stuMasterPoint.nx = m_Xm3;
	m_stuPointPairEx[nSensor].stuPoint[2].stuMasterPoint.ny = m_Ym3;
	m_stuPointPairEx[nSensor].stuPoint[2].stuSlavePoint.nx = m_Xs3;
	m_stuPointPairEx[nSensor].stuPoint[2].stuSlavePoint.ny = m_Ys3;

	m_stuPointPairEx[nSensor].stuPoint[3].stuMasterPoint.nx = m_Xm4;
	m_stuPointPairEx[nSensor].stuPoint[3].stuMasterPoint.ny = m_Ym4;
	m_stuPointPairEx[nSensor].stuPoint[3].stuSlavePoint.nx = m_Xs4;
	m_stuPointPairEx[nSensor].stuPoint[3].stuSlavePoint.ny = m_Ys4;

	BOOL bCheck = CheckPoint();
	if (bCheck)
	{
		if (m_nSelceCout[nSensor] == 0)
		{
			m_nSelceCout[nSensor] = 1;
			m_nCalibrateNum++;
		}

		if (m_nCalibrateNum >= 2)
		{
			GetDlgItem(IDC_BUTTON_DONE)->EnableWindow(TRUE);
		}

		MessageBox(ConvertString("Save success"), ConvertString("Prompt"));
		GetDlgItem(IDC_BUTTON_CALIBRATE_EACHSENSOR)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(TRUE);
		DlgEnableWindow(TRUE);
	}
	else
	{
		MessageBox(ConvertString("Save failed"), ConvertString("Prompt"));
	}
}

BOOL CMasterSlaver_watcherDlg::CheckPoint()
{
	if (m_Xm1 && m_Ym1 && m_Xs1 && m_Ys1
		&& m_Xm2 && m_Ym2 && m_Xs2 && m_Ys2
		 && m_Xm3 && m_Ym3 && m_Xs3 && m_Ys3
		  && m_Xm4 && m_Ym4 && m_Xs4 && m_Ys4)
	{
		return TRUE;
	}
	return FALSE;
}

void CMasterSlaver_watcherDlg::OnButtonCalibrateEachsensor() 
{
	//// TODO: Add your control notification handler code here
	int nGroupID = m_cmbSelectGroup.GetCurSel();
	if (nGroupID != 0)
	{
		if ( m_stuPointPairEx[nGroupID - 1].stuPoint[0].stuMasterPoint.nx == 0)
		{
			MessageBox(ConvertString("Please select the group number by correct order"), ConvertString("Prompt"));
			return;
		}
	}
	char szLocationInfo[1024*32] ;
	memset(szLocationInfo, 0 , 1024*32);
	DH_PTZ_LOCATION_INFO * stLocationInfo = NULL;
	int nRetLen = 0;
	BOOL bRet = 0;
	bRet = CLIENT_QueryDevState(m_lLoginID_REMOTE, DH_DEVSTATE_PTZ_LOCATION,szLocationInfo,sizeof(DH_PTZ_LOCATION_INFO),&nRetLen,3000);
	if(TRUE == bRet)
	{
		stLocationInfo = (DH_PTZ_LOCATION_INFO*)szLocationInfo;	
	}
	else
	{
		MessageBox(ConvertString("Get device location failed"), ConvertString("Prompt"));
		return;
	}

	int nSel = m_cmbSelectGroup.GetCurSel();
	m_stuPointPairEx[nSel].stuSlaveCenter.nPositionX = stLocationInfo->nPTZPan;
	m_stuPointPairEx[nSel].stuSlaveCenter.nPositionY = stLocationInfo->nPTZTilt;
	m_stuPointPairEx[nSel].stuSlaveCenter.nZoom = stLocationInfo->nPTZZoom;

	m_stuPointPairEx[nSel].nEffectiveNumber = m_nGroupNum;
	m_stuPointPairEx[nSel].nCaliSceneID = 0;

	SetCalibrateBySensor(nSel);
}

void CMasterSlaver_watcherDlg::ShowCalibrateBySensor(int nSensor)
{
	m_Xm1 = m_stuPointPairEx[nSensor].stuPoint[0].stuMasterPoint.nx;
	m_Ym1 = m_stuPointPairEx[nSensor].stuPoint[0].stuMasterPoint.ny;
	m_Xs1 = m_stuPointPairEx[nSensor].stuPoint[0].stuSlavePoint.nx;
	m_Ys1 = m_stuPointPairEx[nSensor].stuPoint[0].stuSlavePoint.ny;

	m_Xm2 = m_stuPointPairEx[nSensor].stuPoint[1].stuMasterPoint.nx;
	m_Ym2 = m_stuPointPairEx[nSensor].stuPoint[1].stuMasterPoint.ny;
	m_Xs2 = m_stuPointPairEx[nSensor].stuPoint[1].stuSlavePoint.nx;
	m_Ys2 = m_stuPointPairEx[nSensor].stuPoint[1].stuSlavePoint.ny;

	m_Xm3 = m_stuPointPairEx[nSensor].stuPoint[2].stuMasterPoint.nx;
	m_Ym3 = m_stuPointPairEx[nSensor].stuPoint[2].stuMasterPoint.ny;
	m_Xs3 = m_stuPointPairEx[nSensor].stuPoint[2].stuSlavePoint.nx;
	m_Ys3 = m_stuPointPairEx[nSensor].stuPoint[2].stuSlavePoint.ny;

	m_Xm4 = m_stuPointPairEx[nSensor].stuPoint[3].stuMasterPoint.nx;
	m_Ym4 = m_stuPointPairEx[nSensor].stuPoint[3].stuMasterPoint.ny;
	m_Xs4 = m_stuPointPairEx[nSensor].stuPoint[3].stuSlavePoint.nx;
	m_Ys4 = m_stuPointPairEx[nSensor].stuPoint[3].stuSlavePoint.ny;

	UpdateData(FALSE);
}

void CMasterSlaver_watcherDlg::OnButtonDone() 
{
	if (m_nCalibrateNum < 2)
	{
		MessageBox(ConvertString("Calibrate at least two groups"), ConvertString("Prompt"));
		return;
	}

	m_stuOutCalibrateConfig.nMaxCalibElement = 1;
	//strncpy(m_stuOutCalibrateConfig.pstCaliMatrixElementEx[0].szMasterSN, m_szMasterSerial, MASTERSLAVE_DEV_SERIALNO_LEN - 1);
	strncpy(m_stuOutCalibrateConfig.pstCaliMatrixElementEx[0].szSlaveSN, m_szSlaveSerial, MASTERSLAVE_DEV_SERIALNO_LEN - 1);
	if(m_nSensorNum == 0)
	{
		m_stuOutCalibrateConfig.pstCaliMatrixElementEx[0].nEffectiveNumber = m_nCalibrateNum;
	}

	memcpy(&m_stuOutCalibrateConfig.pstCaliMatrixElementEx[0].stuPointPairEx, &m_stuPointPairEx, 8* sizeof(NET_POINT_PAIR_EX));
	BOOL bRet = CLIENT_SetConfig(m_lLoginID, NET_EM_CFG_STEREO_CALIBRATEMATRIX_MULTISENSOR, -1, &m_stuOutCalibrateConfig, sizeof(m_stuOutCalibrateConfig),5000);
	if(!bRet)
	{
		MessageBox(ConvertString("Calibrate failed"), ConvertString("Prompt"));
		return;
	}
	else
	{
		((CButton *)GetDlgItem(IDC_BUTTON_DONE))->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(FALSE);
	}

	m_nCalibrateDone = 1; // 标定完成，点击全景相机可看到联动现象
	((CButton *)GetDlgItem(IDC_BUTTON_new))->EnableWindow(TRUE);
}

void CMasterSlaver_watcherDlg::OnBUTTONnew() 
{
	// TODO: Add your control notification handler code here
	m_nCalibrateDone = 0; 
	((CButton *)GetDlgItem(IDC_BUTTON_DONE))->EnableWindow(TRUE);	
}

void CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon1()
{
	// TODO: 在此添加控件通知处理程序代码
	char szLocationInfo[1024*32] ;
	memset(szLocationInfo, 0 , 1024*32);
	DH_PTZ_LOCATION_INFO * stLocationInfo = NULL;
	int nRetLen = 0;
	BOOL bRet = 0;
	bRet = CLIENT_QueryDevState(m_lLoginID_REMOTE, DH_DEVSTATE_PTZ_LOCATION,szLocationInfo,sizeof(DH_PTZ_LOCATION_INFO),&nRetLen,3000);
	if(TRUE == bRet)
	{
		stLocationInfo = (DH_PTZ_LOCATION_INFO*)szLocationInfo;	
		m_Xs1 = stLocationInfo->nPTZPan;
		m_Ys1 = stLocationInfo->nPTZTilt;
		m_Zs1 = stLocationInfo->nPTZZoom;
		UpdateData(FALSE);
	}
	else
	{
		MessageBox(ConvertString("Get device location failed"), ConvertString("Prompt"));
	}
}

void CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon2()
{
	// TODO: 在此添加控件通知处理程序代码
	char szLocationInfo[1024*32] ;
	memset(szLocationInfo, 0 , 1024*32);
	DH_PTZ_LOCATION_INFO * stLocationInfo = NULL;
	int nRetLen = 0;
	BOOL bRet = 0;
	bRet = CLIENT_QueryDevState(m_lLoginID_REMOTE, DH_DEVSTATE_PTZ_LOCATION,szLocationInfo,sizeof(DH_PTZ_LOCATION_INFO),&nRetLen,3000);
	if(TRUE == bRet)
	{
		stLocationInfo = (DH_PTZ_LOCATION_INFO*)szLocationInfo;	
		m_Xs2 = stLocationInfo->nPTZPan;
		m_Ys2 = stLocationInfo->nPTZTilt;
		m_Zs2 = stLocationInfo->nPTZZoom;
		UpdateData(FALSE);
	}
	else
	{
		MessageBox(ConvertString("Get device location failed"), ConvertString("Prompt"));
	}
}

void CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon3()
{
	// TODO: 在此添加控件通知处理程序代码
	char szLocationInfo[1024*32] ;
	memset(szLocationInfo, 0 , 1024*32);
	DH_PTZ_LOCATION_INFO * stLocationInfo = NULL;
	int nRetLen = 0;
	BOOL bRet = 0;
	bRet = CLIENT_QueryDevState(m_lLoginID_REMOTE, DH_DEVSTATE_PTZ_LOCATION,szLocationInfo,sizeof(DH_PTZ_LOCATION_INFO),&nRetLen,3000);
	if(TRUE == bRet)
	{
		stLocationInfo = (DH_PTZ_LOCATION_INFO*)szLocationInfo;	
		m_Xs3 = stLocationInfo->nPTZPan;
		m_Ys3 = stLocationInfo->nPTZTilt;
		m_Zs3 = stLocationInfo->nPTZZoom;
		UpdateData(FALSE);
	}
	else
	{
		MessageBox(ConvertString("Get device location failed"), ConvertString("Prompt"));
	}
}

void CMasterSlaver_watcherDlg::OnBnClickedGetSlavepositon4()
{
	// TODO: 在此添加控件通知处理程序代码
	char szLocationInfo[1024*32] ;
	memset(szLocationInfo, 0 , 1024*32);
	DH_PTZ_LOCATION_INFO * stLocationInfo = NULL;
	int nRetLen = 0;
	BOOL bRet = 0;
	bRet = CLIENT_QueryDevState(m_lLoginID_REMOTE, DH_DEVSTATE_PTZ_LOCATION,szLocationInfo,sizeof(DH_PTZ_LOCATION_INFO),&nRetLen,3000);
	if(TRUE == bRet)
	{
		stLocationInfo = (DH_PTZ_LOCATION_INFO*)szLocationInfo;	
		m_Xs4 = stLocationInfo->nPTZPan;
		m_Ys4 = stLocationInfo->nPTZTilt;
		m_Zs4 = stLocationInfo->nPTZZoom;
		UpdateData(FALSE);
	}
	else
	{
		MessageBox(ConvertString("Get device location failed"), ConvertString("Prompt"));
	}
}

//PTZ control command definition setup 
void CMasterSlaver_watcherDlg::InitPTZControl()
{
	//Control parameter dropdown menu initialization 
	CString strPTZData[8]={"1","2","3","4","5","6","7","8"};
	m_comboPTZData.ResetContent();
	for(int i=0;i<8;i++)
	{
		int nIndex = m_comboPTZData.AddString(strPTZData[i]);
		m_comboPTZData.SetItemData(nIndex,i+1);
	}
	m_comboPTZData.SetCurSel(3);

	m_ptz_up.SetButtonCommand(DH_PTZ_UP_CONTROL);
	m_ptz_down.SetButtonCommand(DH_PTZ_DOWN_CONTROL);
	m_ptz_left.SetButtonCommand(DH_PTZ_LEFT_CONTROL);
	m_ptz_right.SetButtonCommand(DH_PTZ_RIGHT_CONTROL);
	m_zoom_wide.SetButtonCommand(DH_PTZ_ZOOM_DEC_CONTROL);
	m_zoom_tele.SetButtonCommand(DH_PTZ_ZOOM_ADD_CONTROL);
	m_ptz_rightup.SetButtonCommand(DH_EXTPTZ_RIGHTTOP);
	m_ptz_rightdown.SetButtonCommand(DH_EXTPTZ_RIGHTDOWN);
	m_ptz_leftup.SetButtonCommand(DH_EXTPTZ_LEFTTOP);
	m_ptz_leftdown.SetButtonCommand(DH_EXTPTZ_LEFTDOWN);
}

//PTZ control 
void CMasterSlaver_watcherDlg::PtzControl(int type, BOOL stop)
{
	if (0 == m_lLoginID_REMOTE)
	{
		return;
	}

	UpdateData(TRUE);
	BYTE param1=0,param2=0;
	BYTE bPTZData=(unsigned char)GetDlgItemInt(IDC_COMBO_PTZDATA);
	switch(type) {
		case DH_PTZ_UP_CONTROL:
			//Up
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_DOWN_CONTROL:
			//Down
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_LEFT_CONTROL:
			//Left 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_RIGHT_CONTROL:
			//Right 
			param1=0;
			param2=bPTZData;
			break;
		case DH_EXTPTZ_LEFTTOP:
			//Up left
			param1=bPTZData;
			param2=bPTZData;
			break;
		case DH_EXTPTZ_LEFTDOWN:
			//Up down 
			param1=bPTZData;
			param2=bPTZData;
			break;
		case DH_EXTPTZ_RIGHTTOP:
			//Up right 
			param1=bPTZData;
			param2=bPTZData;
			break;
		case DH_EXTPTZ_RIGHTDOWN:
			//Up down 
			param1=bPTZData;
			param2=bPTZData;
			break;
		case DH_PTZ_ZOOM_DEC_CONTROL:
			//Zoom out 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_ZOOM_ADD_CONTROL:
			//Zoom in 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_FOCUS_DEC_CONTROL:
			//Focus zoom in 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_FOCUS_ADD_CONTROL:
			//Focus zoom out 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_APERTURE_DEC_CONTROL:
			//Aperture zoom out 
			param1=0;
			param2=bPTZData;
			break;
		case DH_PTZ_APERTURE_ADD_CONTROL:
			//Aperture zoom in 
			param1=0;
			param2=bPTZData;
			break;
		default:
			break;
	}
	BOOL bRet=CLIENT_DHPTZControl(m_lLoginID_REMOTE,0,type,param1,param2,0,stop);
	if(!bRet)
	{
		MessageBox(ConvertString("PTZ control failed"), ConvertString("Prompt"));
	}
}

void CMasterSlaver_watcherDlg::OnSelchangeChannel() 
{
	m_cmbSelectNumber.SetCurSel(0);
	int nGroupID = m_cmbSelectGroup.GetCurSel();
	ShowCalibrateBySensor(nGroupID);
}
void CMasterSlaver_watcherDlg::OnBnClickedButtonStartcalibrate()
{
	// TODO: 在此添加控件通知处理程序代码
	GetDlgItem(IDC_BUTTON_STARTCALIBRATE)->EnableWindow(FALSE);
	DlgEnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_CALIBRATE_EACHSENSOR)->EnableWindow(TRUE);
}

void CMasterSlaver_watcherDlg::DlgEnableWindow(BOOL bFlag)
{
	GetDlgItem(IDC_PTZ_RIGHTDOWN)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_RIGHTUP)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_LEFTDOWN)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_LEFTUP)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_RIGHT)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_LEFT)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_DOWN)->EnableWindow(bFlag);
	GetDlgItem(IDC_PTZ_UP)->EnableWindow(bFlag);
	GetDlgItem(IDC_ZOOM_TELE)->EnableWindow(bFlag);
	GetDlgItem(IDC_ZOOM_WIDE)->EnableWindow(bFlag);
}

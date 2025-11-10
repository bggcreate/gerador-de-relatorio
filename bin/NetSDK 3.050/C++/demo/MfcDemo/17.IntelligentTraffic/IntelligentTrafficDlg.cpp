// IntelligentTrafficDlg.cpp : implementation file
//

#include "stdafx.h"
#include "IntelligentTraffic.h"
#include "IntelligentTrafficDlg.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// Device ip
#define DEVICE_IP           "172.23.19.62"     
// Connect port  
#define CONNECT_PORT        37777            
// Connect user name  
#define CONNECT_USERNAME    "admin"          
// Connect password
#define CONNECT_PASSWORD    "admin123"     


// Device disconnect Callback
void CALLBACK DisConnect(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
    if(0 != dwUser)
    {
        PostMessage(((CIntelligentTrafficDlg *)dwUser)->GetSafeHwnd(), WM_DEVICE_DISCONNECT, 0, 0);
    }
}

// Device reconnect Callback
void CALLBACK ReConnect(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
    if(0 != dwUser)
    {
        PostMessage(((CIntelligentTrafficDlg *)dwUser)->GetSafeHwnd(), WM_REDEVICE_RECONNECT, 0, 0);
    }
}

// Handle message : WM_DEVICE_DISCONNECT
LRESULT CIntelligentTrafficDlg::OnDeviceDisConnect(WPARAM wParam, LPARAM lParam)
{
    SetWindowText(ConvertString("Network disconnected!"));
    return 0;
}

// Handle message : WM_REDEVICE_RECONNECT
LRESULT CIntelligentTrafficDlg::OnDeviceReconnect(WPARAM wParam, LPARAM lParam)
{
    SetWindowText(ConvertString("IntelligentTraffic"));
    return 0;
}

// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CIntelligentTrafficDlg dialog


CIntelligentTrafficDlg::CIntelligentTrafficDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CIntelligentTrafficDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
    m_lLoginHandle = 0;

}

void CIntelligentTrafficDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_TAB1, m_ctrTab);
}

BEGIN_MESSAGE_MAP(CIntelligentTrafficDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
    ON_NOTIFY(TCN_SELCHANGE, IDC_TAB1, &CIntelligentTrafficDlg::OnTcnSelchangeTab)
    ON_BN_CLICKED(IDC_BTN_LOGIN_AND_LOGOUT, &CIntelligentTrafficDlg::OnBnClickedBtnLoginAndLogout)
    ON_MESSAGE(WM_DEVICE_DISCONNECT,&CIntelligentTrafficDlg::OnDeviceDisConnect)
    ON_MESSAGE(WM_REDEVICE_RECONNECT,&CIntelligentTrafficDlg::OnDeviceReconnect)
    ON_WM_DESTROY()
END_MESSAGE_MAP()


// CIntelligentTrafficDlg message handlers

BOOL CIntelligentTrafficDlg::PreTranslateMessage(MSG* pMsg)
{
    // Enter key
    if(pMsg->message == WM_KEYDOWN &&
        pMsg->wParam == VK_RETURN)
    {
        return TRUE;
    }

    // Escape key
    if(pMsg->message == WM_KEYDOWN &&
        pMsg->wParam == VK_ESCAPE)
    {
        return TRUE;
    }
    return CDialog::PreTranslateMessage(pMsg);
}

BOOL CIntelligentTrafficDlg::OnInitDialog()
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
    
    g_SetWndStaticText(this);
	InitDialogContorl();
    InitNetSdk();
   
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CIntelligentTrafficDlg::InitDialogContorl(void)
{
  
    SetDlgItemText(IDC_IPADDRESS1, DEVICE_IP);
    SetDlgItemInt(IDC_EDIT_PORT, CONNECT_PORT);
    SetDlgItemText(IDC_EDIT_USER, CONNECT_USERNAME);
    SetDlgItemText(IDC_EDIT_PWD, CONNECT_PASSWORD);

    m_dlgBWList.Create(IDD_BW_LITS_DIALOG, &m_ctrTab);
    m_dlgTrafficFlow.Create(IDD_TRAFIC_FLOW_DIALOG, &m_ctrTab);
    m_dlgIntelligentEvent.Create(IDD_INTELLIGENT_EVENT_DIALOG, &m_ctrTab);
    m_dlgQueryPictureAndRecord.Create(IDD_QUERY_TRAFFIC_PICTURE_DIALOG, &m_ctrTab);

    m_ctrTab.InsertItem(0, ConvertString("Intelligent Event"));
    m_ctrTab.InsertItem(1, ConvertString("BW List"));
    m_ctrTab.InsertItem(2, ConvertString("Traffic Flow"));
    m_ctrTab.InsertItem(3, ConvertString("Query Traffic Picture"));

    m_ctrTab.SetMinTabWidth(-1);
    m_ctrTab.SetPadding(CSize(25, 5));
    m_ctrTab.SetCurSel(0);
    DoTab(0);
}



void CIntelligentTrafficDlg::DoTab(int nTab)
{
    //Confirm nTab value is within the threshold.
    if (nTab < 0 || nTab > 4)
    {
        nTab = 0;
    }

    BOOL bTab[4];
    for (int i = 0; i < 4; ++i)
    {
        if (i == nTab)
        {
            bTab[i]=TRUE;
        }
        else
        {
            bTab[i]=FALSE;
        }
    }

    //Display or hide dialog
    SetDlgState(&m_dlgIntelligentEvent, bTab[0]);
    SetDlgState(&m_dlgBWList, bTab[1]);
    SetDlgState(&m_dlgTrafficFlow, bTab[2]);
    SetDlgState(&m_dlgQueryPictureAndRecord, bTab[3]);
}

void CIntelligentTrafficDlg::SetDlgState(CWnd *pWnd, BOOL bShow)
{
    if(bShow)
    {
        CRect rc;
        pWnd->GetWindowRect(rc);
        pWnd->MoveWindow(10, 40, rc.Width(), rc.Height());
        pWnd->ShowWindow(SW_SHOW);
    }
    else
    {
        pWnd->ShowWindow(SW_HIDE);
    }
}



void CIntelligentTrafficDlg::InitNetSdk(void)
{
    CLIENT_Init(DisConnect, (LDWORD)this);
    CLIENT_SetAutoReconnect(ReConnect, (LDWORD)this);
}



void CIntelligentTrafficDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CIntelligentTrafficDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

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

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CIntelligentTrafficDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CIntelligentTrafficDlg::OnTcnSelchangeTab(NMHDR *pNMHDR, LRESULT *pResult)
{
    int nSelect = m_ctrTab.GetCurSel();
    if(nSelect>=0)
    {
        DoTab(nSelect);
    }
    *pResult = 0;
}

void CIntelligentTrafficDlg::OnBnClickedBtnLoginAndLogout()
{
    CButton* pBtnLoginAndLogout =  (CButton*)GetDlgItem(IDC_BTN_LOGIN_AND_LOGOUT);

    if (0 != m_lLoginHandle )
    {
        CleanUpChildDlgOfTab();
        CLIENT_Logout(m_lLoginHandle);
        m_lLoginHandle = 0;
        pBtnLoginAndLogout->SetWindowText(ConvertString("Login"));
        SetWindowText(ConvertString("IntelligentTraffic"));
    }
    else
    {
        CString csIP = "";
        CString csUserName = "";
        CString csPwd = "";
        int nPort = 0; 
        GetDlgItemText(IDC_IPADDRESS1, csIP);
        GetDlgItemText(IDC_EDIT_USER, csUserName);
        GetDlgItemText(IDC_EDIT_PWD, csPwd);
        nPort = GetDlgItemInt(IDC_EDIT_PORT);

        NET_DEVICEINFO_Ex stDevInfo = {0};
        m_lLoginHandle = CLIENT_LoginEx2(csIP.GetBuffer(), nPort, csUserName.GetBuffer(), csPwd.GetBuffer(), EM_LOGIN_SPEC_CAP_TCP, NULL, &stDevInfo, NULL);   
        if (0 == m_lLoginHandle)
        {
            MessageBox(ConvertString("Login failed!"), ConvertString("Prompt")); 
            m_lLoginHandle = 0;
            return;
        }
        InitChildDlgOfTab(stDevInfo.nChanNum, m_lLoginHandle);
        pBtnLoginAndLogout->SetWindowText(ConvertString("Logout"));
    }
}

void CIntelligentTrafficDlg::OnDestroy()
{
    if (0 != m_lLoginHandle)
    {
        // Logout device 
        OnBnClickedBtnLoginAndLogout();
    }
    CLIENT_Cleanup();
    CDialog::OnDestroy();
}


void CIntelligentTrafficDlg::InitChildDlgOfTab(int nChannel, LLONG lLoginIHandle)
{
    m_dlgIntelligentEvent.Init(nChannel, lLoginIHandle);
    m_dlgBWList.Init(nChannel, lLoginIHandle);
    m_dlgTrafficFlow.Init(nChannel, lLoginIHandle);
    m_dlgQueryPictureAndRecord.Init(nChannel, lLoginIHandle);
}


void CIntelligentTrafficDlg::CleanUpChildDlgOfTab()
{
    m_dlgIntelligentEvent.CleanUp();
    m_dlgBWList.CleanUp();
    m_dlgTrafficFlow.CleanUp();
    m_dlgQueryPictureAndRecord.CleanUp();
}
// PlayDemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include <windows.h>
#include "PlayDemo.h"
#include "PlayDemoDlg.h"
#include "LocateDlg.h"
#include "Player.h"
#include "LanguageConvertor.h"
#include "GetDllVersion.h"
#include "CmdDialog.h"
#include "dhplayEx.h"
#include "CharactorTansfer.h"
#include <string>
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma comment(lib, "Version.lib")
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
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	BOOL m_bGotVer;
	CString m_strVer;
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
	m_bGotVer = FALSE;
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()
void CAboutDlg::OnOK() 
{
	// TODO: Add extra validation here
	CDialog::OnOK();
}

BOOL CAboutDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here

	if(!m_bGotVer)
	{
		TCHAR szDir[MAX_PATH];
		TCHAR szDrive[MAX_PATH];
		TCHAR szDllName[MAX_PATH];
		TCHAR szAppName[MAX_PATH];

		GetModuleFileName(GetModuleHandle(NULL), szAppName, MAX_PATH);
		_tsplitpath(szAppName, szDrive, szDir, NULL, NULL);
		_tmakepath(szDllName, szDrive, szDir, _T("dhplay"), _T(".dll"));
		m_bGotVer = GetDllVersion(szDllName, m_strVer);
	}

	if(m_bGotVer)
	{
		GetDlgItem(IDC_STATIC_VERSION)->SetWindowText(m_strVer);
	}

	LANG_SETWNDSTATICTEXT(this);
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


/////////////////////////////////////////////////////////////////////////////
// CPlayDemoDlg dialog

#define SUPPORT_FISYEYE_VR 0

CPlayDemoDlg::CPlayDemoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CPlayDemoDlg::IDD, pParent)
	, m_lastFisheyeMode(0)
	, m_DisplayAngle(0)

{
	//{{AFX_DATA_INIT(CPlayDemoDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_nThrowMode = ID_THROW_NO;
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CPlayDemoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPlayDemoDlg)
	DDX_Control(pDX, IDC_SLIDER_PROC, m_sdProc);
	DDX_Control(pDX, IDC_BUTTON_PAUSE, m_bnPause);
	DDX_Control(pDX, IDC_BUTTON_SETCOLOR, m_bnSetColor);
	DDX_Control(pDX, IDC_BUTTON_TOEND, m_bnToEnd);
	DDX_Control(pDX, IDC_BUTTON_TOBEGIN, m_bnToBegin);
	DDX_Control(pDX, IDC_BUTTON_STOP, m_bnStop);
	DDX_Control(pDX, IDC_BUTTON_SLOW, m_bnSlow);
	DDX_Control(pDX, IDC_BUTTON_PICTURE, m_bnPicCatch);
	DDX_Control(pDX, IDC_BUTTON_ONEBYONE, m_bnStepOne);
	DDX_Control(pDX, IDC_BUTTON_BACKONCE, m_bnBackOne);
	DDX_Control(pDX, IDC_BUTTON_FAST, m_bnFast);
	DDX_Control(pDX, IDC_BUTTON_PLAY, m_bnPlay);
	DDX_Control(pDX, IDC_SLIDER_WAVE, m_sdAudioWave);
	DDX_Control(pDX, IDC_SLIDER_AUDIO, m_sdAuidoVolume);
	DDX_Control(pDX, IDC_BUTTON_AUDIO, m_bnAudio);
	DDX_Control(pDX, IDC_BUTTON_BACKWORD, m_bnBackword);
	DDX_Control(pDX, IDC_BUTTON_FORWORD, m_bnForword);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CPlayDemoDlg, CDialog)
	//{{AFX_MSG_MAP(CPlayDemoDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_WM_QUERYDRAGICON()
	ON_COMMAND(IDM_FILE_OPEN, OnFileOpen)
	ON_COMMAND(IDM_FILE_CLOSE, OnFileClose)
	ON_COMMAND(IDM_SETTING_LOCATE, OnLocate)
	ON_COMMAND(IDM_SETTING_CUTFILE, OnCutFile)
	ON_COMMAND(IDM_SETTING_MEDIAINFO, OnMediaInfo)
	ON_WM_HSCROLL()
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_WM_DROPFILES()
	ON_COMMAND(IDM_HELP_ABOUT, OnHelpAbout)
	ON_COMMAND(IDM_SETTING_SNAPPIC, OnSettingSnappic)
	ON_COMMAND(IDM_SETTING_VERTICALSYNC, OnVerticalSync)
	//ON_COMMAND(IDM_SETTING_IVS, OnIVS)
	ON_BN_CLICKED(IDC_BUTTON_PICTURE, OnButtonPicture)
	ON_BN_CLICKED(IDC_BUTTON_AUDIO, OnButtonAudio)
	ON_BN_CLICKED(IDC_BUTTON_SETCOLOR, OnButtonSetcolor)
	ON_COMMAND_RANGE(IDC_BUTTON_PLAY, IDC_BUTTON_FAST, OnOperator)
	ON_BN_CLICKED(IDC_BUTTON_BACKWORD, OnButtonBackword)
	ON_BN_CLICKED(IDC_BUTTON_FORWORD, OnButtonForword)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_USER_MSD_INDEXCREATED, OnIndexCreated)
	ON_MESSAGE(WM_USER_MSD_FISHEYEDEVICE_DETECT, OnFisheyeDetect)
	ON_COMMAND(ID_FISHEYEVIEW_WALL_1PPLUS8, OnFisheyeviewWall1pplus8)
	ON_COMMAND(ID_FISHEYEVIEW_FLOOR_1PPLUS6, OnFisheyeviewFloor1pplus6)
	ON_COMMAND(ID_FISHEYEVIEW_FLOOR_1PLUS2, OnFisheyeviewFloor1plus2)
	ON_COMMAND(ID_FISHEYEVIEW_CEIL_1PPLUS1, OnFisheyeviewCeil1pplus1)
	ON_COMMAND(ID_FISHEYEVIEW_CEIL_1PLUS3, OnFisheyeviewCeil1plus3)
	ON_COMMAND(ID_FISHEYEVIEW_FLOOR_2P, OnFisheyeviewFloor2p)
	ON_COMMAND(ID_SETTING_START_FISHEYE, OnSettingStartFisheye)
	ON_COMMAND(ID_SETTING_DECODEENGINE, OnSettingDecodeEngine)
	ON_COMMAND_RANGE(ID_ROTATEANGLE_0, ID_ACCELERATOR_F8, OnRotateAngle)

	ON_COMMAND_RANGE(ID_THROW_NO, ID_THROW_ADAPTION, OnSetThrowMode)

	ON_COMMAND(ID_ACCELERATOR_F9, OnDevCase)
	ON_COMMAND(ID_SETTING_THROWFRAME, OnSettingThrowframe)
	ON_COMMAND(ID_FISHEYEVIEW_SEMI, OnFisheyeviewSemi)
	ON_COMMAND(ID_FISHEYEVIEW_CYLINDER, OnFisheyeviewCylinder)
	ON_COMMAND(ID_FISHEYEVIEW_LITTLE, OnFisheyeviewLittle)
	ON_COMMAND(ID_FISHEYEVIEW_360_SHPERE, OnFisheyeview360Sphere)
	ON_COMMAND(ID_FISHEYEVIEW_360_CYLINDER, OnFisheyeview360Cylinder)
	ON_COMMAND(ID_SETTING_ANTI, OnAntiAliasing)
	ON_COMMAND(ID_SEMI_CEIL, &CPlayDemoDlg::OnSemiCeil)
	ON_COMMAND(ID_SEMI_FLOOR, &CPlayDemoDlg::OnSemiFloor)
	ON_COMMAND(ID_SEMI_WALL, &CPlayDemoDlg::OnSemiWall)
	ON_COMMAND(ID_CYLINDER_CEIL, &CPlayDemoDlg::OnCylinderCeil)
	ON_COMMAND(ID_CYLINDER_FLOOR, &CPlayDemoDlg::OnCylinderFloor)
	ON_COMMAND(ID_CYLINDER_WALL, &CPlayDemoDlg::OnCylinderWall)
	ON_COMMAND(ID_LITTLE_CEIL, &CPlayDemoDlg::OnLittleCeil)
	ON_COMMAND(ID_LITTLE_FLOOR, &CPlayDemoDlg::OnLittleFloor)
	ON_COMMAND(ID_LITTLE_WALL, &CPlayDemoDlg::OnLittleWall)
END_MESSAGE_MAP()

BEGIN_EASYSIZE_MAP(CPlayDemoDlg)
		 EASYSIZE(IDC_STATIC_PLAY,ES_BORDER,ES_BORDER,
		 ES_BORDER,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_PLAY,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_STOP,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_PAUSE,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_TOBEGIN,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_TOEND,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_BACKONCE,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_ONEBYONE,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_SLOW,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_FAST,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_BACKWORD,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_FORWORD,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_PICTURE,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_SETCOLOR,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_BUTTON_AUDIO,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_SLIDER_AUDIO,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_SLIDER_WAVE,ES_BORDER,IDC_STATIC_PLAY,
		 ES_KEEPSIZE,ES_BORDER,0)
		 EASYSIZE(IDC_SLIDER_PROC,ES_BORDER,IDC_STATIC_PLAY,
		 ES_BORDER,ES_BORDER,0)
		 EASYSIZE(IDC_STATIC_MSG,ES_BORDER,IDC_STATIC_PLAY,
		 ES_BORDER,ES_BORDER,0)
END_EASYSIZE_MAP

/////////////////////////////////////////////////////////////////////////////
// CPlayDemoDlg message handlers
BOOL CPlayDemoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here

	/*load button bmp*/
	m_bnPlay.LoadBitmap(IDB_BITMAP_PLAY);
	m_bnPause.LoadBitmap(IDB_BITMAP_PAUSE);
	m_bnStop.LoadBitmap(IDB_BITMAP_STOP);
	m_bnToEnd.LoadBitmap(IDB_BITMAP_TOEND);
	m_bnBackOne.LoadBitmap(IDB_BITMAP_STEPBACK);
	m_bnStepOne.LoadBitmap(IDB_BITMAP_STEP);
	m_bnSlow.LoadBitmap(IDB_BITMAP_SLOW);
	m_bnFast.LoadBitmap(IDB_BITMAP_FAST);
	m_bnToBegin.LoadBitmap(IDB_BITMAP_TOBEGIN);
	m_bnPicCatch.LoadBitmap(IDB_BITMAP_CAMERA);
	m_bnSetColor.LoadBitmap(IDB_BITMAP_SETCOLOR);
	m_bnAudio.LoadBitmap(IDB_BITMAP_SOUND);
	m_bnBackword.LoadBitmap(IDB_BITMAP_STEPBACK);
	m_bnForword.LoadBitmap(IDB_BITMAP_STEP);
	
	m_sdAudioWave.SetRange(-100, 100, true);
	m_sdAudioWave.SetPos(0);
	m_sdAuidoVolume.SetRange(0, 0xffff);
	
	
	m_dlgStateText.Init(GetDlgItem(IDC_STATIC_MSG));

	
	m_dlgDisplay.Create(IDD_DIALOG_DISPLAY, GetDlgItem(IDC_STATIC_PLAY));
	m_dlgDisplay.ShowWindow(SW_SHOW);

	
	m_sdProc.SetRange(0, 100);

	ChangeUIState(Close);

	m_hotKey = ::LoadAccelerators(AfxGetInstanceHandle(),MAKEINTRESOURCE(IDR_ACCELERATOR1));

	LANG_INIT();
	LANG_SETWNDSTATICTEXT(this);
	LANG_SETMENUSTATICTEXT(GetMenu());

	INIT_EASYSIZE;

	if (!m_ContentTip.Create(this, TTS_ALWAYSTIP))
	{
		return FALSE;
	}
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_PLAY), _T("play"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_PAUSE), _T("pause"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_STOP), _T("stop"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_FAST), _T("fast"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_SLOW), _T("slow"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_BACKWORD), _T("back"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_FORWORD), _T("forward"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_BACKONCE), _T("one back"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_ONEBYONE), _T("one forward"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_TOEND), _T("end"));
	m_ContentTip.AddTool(GetDlgItem(IDC_BUTTON_TOBEGIN), _T("begin"));
	m_ContentTip.Activate(TRUE);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CPlayDemoDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CPlayDemoDlg::OnPaint() 
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
HCURSOR CPlayDemoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CPlayDemoDlg::OnOperator(UINT uID)
{
	UINT uIDBase = IDC_BUTTON_PLAY;
	/*Button and status enumeration is in sequence relationship which need to be set manually. */
	PLAY_STATE nState = PLAY_STATE(uID - uIDBase);
	
	/*change player status*/
	CPlayer::Instance()->ChangeState(nState);
	
	if ( nState == Stop )
	{
		CMenu* lpMenu = GetMenu();
		//lpMenu->CheckMenuItem(IDM_SETTING_IVS, MF_UNCHECKED);
	}

	/*Operation to switch status*/
	if( CPlayer::Instance()->Do() <= 0)
	{
		ChangeSingleUIState(nState, FALSE);
	}

	/*Refresh status bar*/
	TCHAR* szDesc = CPlayer::Instance()->Description();
	if(szDesc && _tcscmp(szDesc, _T("Play")) == 0) 
	{
		/*Operation of Slow and Fast are special. To switch between slow/fast play, you may need to change play state*/
		nState = Play;
	}
	m_dlgStateText.SetState(LANG_CS(szDesc));
	
	/*change ui status*/
	ChangeUIState(nState);
	
}

LRESULT CPlayDemoDlg::OnIndexCreated(WPARAM wParam, LPARAM lParam)
{
	CPlayer::Instance()->stateTable[0][5] = wParam;
	if(wParam)
	{
		ChangeUIState(m_lastState);
		ChangeMenuState(FILEINDEXCREATED);
	}
	return 0;
}

LRESULT CPlayDemoDlg::OnFisheyeDetect(WPARAM wParam, LPARAM lParam)
{
	MENU_STATE feMenuState = wParam?ENABLEFISHEYE:DISABLEFISHEYE;
	ChangeMenuState(feMenuState);
	return 0;
}

void CPlayDemoDlg::OnLocate()
{
	CLocateDlg dlg;
	dlg.DoModal();
}

void CPlayDemoDlg::OnCutFile()
{
	CCutFileDlg dlg(m_dlgOpenFile.m_strFile);
	dlg.DoModal();
}

void CPlayDemoDlg::OnMediaInfo()
{
	CMediaInfoDlg dlg;
	dlg.DoModal();
}

void CPlayDemoDlg::OpenFile()
{
	/**/
	CPlayer::Instance()->stateTable[0][5] = 0;
	
	/*Open player*/
	if( CPlayer::Instance()->Open(CPlayer::TYPE(m_dlgOpenFile.m_nType), 
		m_dlgOpenFile.m_strFile.GetBuffer(0), 
		m_dlgDisplay.m_hWnd, this->GetSafeHwnd()) <= 0 )
	{
		CPlayer::Instance()->Close();
		AfxMessageBox(LANG_CS("Open file failed!"));
		return;
	}

	/*Change UI status*/
	ChangeUIState(Open);
	
	/*Initialize status bar, file play be length or total number of frames*/
	m_dlgStateText.SetPlayedTime(0, CPlayer::Instance()->GetTotalTime());
	m_dlgStateText.SetPlayedFrame(0, CPlayer::Instance()->GetTotalFrame());


	MediaInit();

	OnSetThrowMode(m_nThrowMode);

	/*Turn on progress bar timer*/
	SetTimer(1, 1000, NULL);
}

void CPlayDemoDlg::OnFileOpen() 
{
	// TODO: Add your command handler code here
	if(m_dlgOpenFile.DoModal() != IDOK || !m_dlgOpenFile.m_strFile.GetLength())
		return;

	OpenFile();
}

void CPlayDemoDlg::OnFileClose() 
{
	// TODO: Add your command handler code here

	/*Reverse operation to Onopenfile*/
	KillTimer(1);

	m_sdProc.SetPos(0);
	m_dlgStateText.Clear();
	m_dlgStateText.Show();

	CloseAudio();
	CPlayer::Instance()->Close();
	
	//lpMenu->CheckMenuItem(IDM_SETTING_IVS, MF_UNCHECKED);

	//UINT nFlag = lpMenu->GetMenuState(IDM_SETTING_IVS, MF_CHECKED) ;
	//UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	//GetMenu()->CheckMenuItem(IDM_SETTING_IVS,  nFlags);

	CMenu* lpMenu = GetMenu();
	//lpMenu->CheckMenuItem(IDM_SETTING_IVS, MF_BYCOMMAND|MF_UNCHECKED);

	ChangeUIState(Close);
	
	/*Refresh display window, may still see the last frame*/
	m_dlgDisplay.CloseMultiWnd();
}

int CPlayDemoDlg::ChangeMenuState(MENU_STATE nState)
{
	CMenu* lpMenu = GetMenu();
	if(nState == FILEOPEN)
	{
		lpMenu->EnableMenuItem(IDM_FILE_OPEN, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_FILE_CLOSE, MF_ENABLED);
		lpMenu->EnableMenuItem(IDM_SETTING_OVERLAY, MF_ENABLED);//only enabled when file is opened and not played

#if SUPPORT_FISYEYE_VR
		lpMenu->EnableMenuItem(ID_SETTING_START_FISHEYE, MF_ENABLED);
		CMenu* pSubMenu = lpMenu->GetSubMenu(1);
		pSubMenu->EnableMenuItem(7, MF_BYPOSITION|MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_SHPERE, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_CYLINDER, MF_ENABLED);
#endif
		lpMenu->EnableMenuItem(IDM_SETTING_VERTICALSYNC, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SETTING_THROWFRAME, MF_ENABLED);
		//lpMenu->EnableMenuItem(IDM_SETTING_IVS, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SETTING_ANTI, MF_ENABLED);
		GetDlgItem(IDC_SLIDER_PROC)->EnableWindow(TRUE);

	}
	else if(nState == FILECLOSE)
	{
		lpMenu->EnableMenuItem(IDM_FILE_OPEN, MF_ENABLED);
		lpMenu->EnableMenuItem(IDM_FILE_CLOSE, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_LOCATE, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_CUTFILE, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_MEDIAINFO, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_OVERLAY, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_VERTICALSYNC, MF_GRAYED);
		//lpMenu->EnableMenuItem(IDM_SETTING_IVS, MF_GRAYED);
		lpMenu->CheckMenuItem(IDM_SETTING_OVERLAY, MF_UNCHECKED);
		lpMenu->CheckMenuItem(IDM_SETTING_VERTICALSYNC, MF_UNCHECKED);
		//lpMenu->CheckMenuItem(IDM_SETTING_IVS, MF_UNCHECKED);
		lpMenu->EnableMenuItem(ID_SETTING_THROWFRAME, MF_GRAYED);
		lpMenu->CheckMenuItem(ID_SETTING_THROWFRAME, MF_UNCHECKED);
		lpMenu->EnableMenuItem(ID_SETTING_ANTI, MF_GRAYED);
		lpMenu->CheckMenuItem(ID_SETTING_ANTI, MF_UNCHECKED);

		CMenu* pSubMenu = lpMenu->GetSubMenu(1);
		pSubMenu->EnableMenuItem(7, MF_BYPOSITION|MF_GRAYED);
		lpMenu->CheckMenuItem(ID_SETTING_START_FISHEYE,  MF_UNCHECKED);
		lpMenu->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);

		lpMenu->EnableMenuItem(ID_SETTING_START_FISHEYE, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PPLUS1, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PLUS3, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_2P, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PLUS2, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PPLUS6, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_WALL_1PPLUS8, MF_GRAYED);
		
		CMenu* pFisheyeModeMenu = pSubMenu->GetSubMenu(7);
		pFisheyeModeMenu->EnableMenuItem(6, MF_BYPOSITION|MF_DISABLED);
		pFisheyeModeMenu->EnableMenuItem(7, MF_BYPOSITION|MF_DISABLED);
		pFisheyeModeMenu->EnableMenuItem(8, MF_BYPOSITION|MF_DISABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_SHPERE, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_CYLINDER, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SETTING_DECODEENGINE, MF_ENABLED);
		
		GetDlgItem(IDC_SLIDER_PROC)->EnableWindow(FALSE);
	}
	/*When index has been created, set single frame back to TRUE, eliminating quick play and slow play.*/
	else if(nState == FILEINDEXCREATED)
	{
		lpMenu->EnableMenuItem(IDM_SETTING_LOCATE, MF_ENABLED);
		lpMenu->EnableMenuItem(IDM_SETTING_CUTFILE, MF_ENABLED);
	}
	else if(nState == STOPPLAY)
	{
		lpMenu->EnableMenuItem(IDM_SETTING_LOCATE, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_OVERLAY, MF_ENABLED);
		lpMenu->EnableMenuItem(IDM_SETTING_VERTICALSYNC, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SETTING_THROWFRAME, MF_GRAYED);
		//lpMenu->EnableMenuItem(IDM_SETTING_IVS, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SETTING_ANTI, MF_GRAYED);

		lpMenu->CheckMenuItem(ID_SETTING_START_FISHEYE, MF_UNCHECKED);
		lpMenu->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	}
	else if(nState == STARTPLAY)
	{
		if(CPlayer::Instance()->IsIndexCreated())
		{
			lpMenu->EnableMenuItem(IDM_SETTING_LOCATE, MF_ENABLED);
			lpMenu->EnableMenuItem(IDM_SETTING_CUTFILE, MF_ENABLED);
		}
		lpMenu->EnableMenuItem(IDM_SETTING_OVERLAY, MF_GRAYED);
		lpMenu->EnableMenuItem(IDM_SETTING_MEDIAINFO, MF_ENABLED);
		lpMenu->EnableMenuItem(IDM_SETTING_VERTICALSYNC, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SETTING_THROWFRAME, MF_ENABLED);
		//lpMenu->EnableMenuItem(IDM_SETTING_IVS, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SETTING_ANTI, MF_ENABLED);
	}
	else if (nState == ENABLEFISHEYE)
	{
		CMenu* pSubMenu = lpMenu->GetSubMenu(1);
		pSubMenu->EnableMenuItem(7, MF_BYPOSITION|MF_ENABLED);

		lpMenu->EnableMenuItem(ID_SETTING_START_FISHEYE, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PPLUS1, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PLUS3, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_2P, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PLUS2, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PPLUS6, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_WALL_1PPLUS8, MF_ENABLED);
#if SUPPORT_FISYEYE_VR
		CMenu* pFisheyeModeMenu = pSubMenu->GetSubMenu(7);
		pFisheyeModeMenu->EnableMenuItem(6, MF_BYPOSITION|MF_ENABLED);
		pFisheyeModeMenu->EnableMenuItem(7, MF_BYPOSITION|MF_ENABLED);
		pFisheyeModeMenu->EnableMenuItem(8, MF_BYPOSITION|MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SEMI_CEIL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SEMI_FLOOR, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_SEMI_WALL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_CYLINDER_CEIL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_CYLINDER_FLOOR, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_CYLINDER_WALL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_LITTLE_CEIL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_LITTLE_FLOOR, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_LITTLE_WALL, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_SHPERE, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_CYLINDER, MF_ENABLED);
#endif

	}
	else if (nState == DISABLEFISHEYE)
	{
#if SUPPORT_FISYEYE_VR
		lpMenu->EnableMenuItem(ID_SETTING_START_FISHEYE, MF_ENABLED);
		CMenu* pSubMenu = lpMenu->GetSubMenu(1);
		pSubMenu->EnableMenuItem(7, MF_BYPOSITION|MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_SHPERE, MF_ENABLED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_CYLINDER, MF_ENABLED);
#else
		lpMenu->EnableMenuItem(ID_SETTING_START_FISHEYE, MF_GRAYED);
		CMenu* pSubMenu = lpMenu->GetSubMenu(1);
		pSubMenu->EnableMenuItem(7, MF_BYPOSITION|MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_SHPERE, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_360_CYLINDER, MF_GRAYED);
#endif
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PPLUS1, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_CEIL_1PLUS3, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_2P, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PLUS2, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_FLOOR_1PPLUS6, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_FISHEYEVIEW_WALL_1PPLUS8, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SEMI_CEIL, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SEMI_FLOOR, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_SEMI_WALL, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_CYLINDER_CEIL, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_CYLINDER_FLOOR, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_CYLINDER_WALL, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_LITTLE_CEIL, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_LITTLE_FLOOR, MF_GRAYED);
		lpMenu->EnableMenuItem(ID_LITTLE_WALL, MF_GRAYED);
	}

	return 1;
}

void CPlayDemoDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	// TODO: Add your message handler code here and/or call default

	switch(GetWindowLong(pScrollBar->m_hWnd, GWL_ID))
	{
	case IDC_SLIDER_PROC:
		CPlayer::Instance()->Seek(m_sdProc.GetPos());
		break;
	case IDC_SLIDER_AUDIO:
		CPlayer::Instance()->SetAudioVolume(m_sdAuidoVolume.GetPos());
		break;
	case IDC_SLIDER_WAVE:
		CPlayer::Instance()->SetAudioWave(m_sdAudioWave.GetPos());
		break;
	default:
		break;
	}


	CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CPlayDemoDlg::OnTimer(UINT_PTR nIDEvent) 
{
	// TODO: Add your message handler code here and/or call default
	if(nIDEvent==1)
	{
		int nProc = CPlayer::Instance()->GetProcess();
		m_sdProc.SetPos(nProc);

		double dbBitrate;
		CPlayer::Instance()->GetBitrate(&dbBitrate);
		m_dlgStateText.SetBitrate(dbBitrate);

		int nRate;
		CPlayer::Instance()->GetRate(&nRate);
		m_dlgStateText.SetRate(nRate);
		
		long nWidht  = 0;
		long nHeight = 0;
		CPlayer::Instance()->GetPicSize(&nWidht, &nHeight);
		if (0 != nWidht && 0 != nHeight)
		{
			m_dlgStateText.SetPictureSize(nWidht, nHeight);
		}

		m_dlgStateText.SetPlayedTime(CPlayer::Instance()->GetPlayedTime(), CPlayer::Instance()->GetTotalTime());
		m_dlgStateText.SetPlayedFrame(CPlayer::Instance()->GetPlayedFrame(), CPlayer::Instance()->GetTotalFrame());

		/*Refresh status bar*/
		m_dlgStateText.Show();
	}

	CDialog::OnTimer(nIDEvent);
}

void CPlayDemoDlg::OnClose() 
{
	/* To close, send a close info. */
	if(GetMenu()->GetMenuState(IDM_FILE_CLOSE, MF_BYCOMMAND) != MF_GRAYED)
	{
		SendMessage(WM_COMMAND, IDM_FILE_CLOSE);
	}

	CDialog::OnClose();
}

void CPlayDemoDlg::OnHelpAbout() 
{
	// TODO: Add your command handler code here

	/*CAboutDlg can be CPlayDemoDlg member, but MFC template did not define  CAboutDlg*/
	CAboutDlg().DoModal();
}

void CPlayDemoDlg::OnSettingSnappic() 
{
	// TODO: Add your command handler code here
    int nLastType = m_dlgPicSetting.m_nType;
    CString lastFilePath = m_dlgPicSetting.m_strPath;
	if (IDOK != m_dlgPicSetting.DoModal())
    {
        m_dlgPicSetting.m_nType = nLastType;
        m_dlgPicSetting.m_strPath = lastFilePath;
    }
}

void CPlayDemoDlg::OnButtonPicture() 
{
	// TODO: Add your control notification handler code here
	SYSTEMTIME localTime;
	GetLocalTime(&localTime);
	
	const TCHAR* tszExt[] = {_T("bmp"), _T("jpg")};
	CString strFileName;
	strFileName.Format(_T("%s\\%d_%d_%d_%d_%d_%d.%s"), 
		m_dlgPicSetting.m_strPath, 
		localTime.wYear,
		localTime.wMonth,
		localTime.wDay,
		localTime.wHour,
		localTime.wMinute,
		localTime.wSecond,
		tszExt[m_dlgPicSetting.m_nType]);
	if(strFileName == "")
		AfxMessageBox(LANG_CS("picture path error!"));
	TCHAR szShortPath[1024] = {0};
    std::string strShortPath;
	DWORD nLength = GetShortPathNameW(m_dlgPicSetting.m_strPath.GetBuffer(0), szShortPath, sizeof(szShortPath)-1);
    if (nLength == 0)
    {
        strShortPath = UnicodeToAnsi(m_dlgPicSetting.m_strPath.GetBuffer(0));
    }
    else
    {
        strShortPath = UnicodeToAnsi(szShortPath);
    }

	char szShortFileName[2048] = {0};
	const char* szExt[] = {"bmp", "jpg"};
	_snprintf(szShortFileName, sizeof(szShortFileName)-1, "%s\\%d_%d_%d_%d_%d_%d.%s", 
		strShortPath.c_str(), 
		localTime.wYear,
		localTime.wMonth,
		localTime.wDay,
		localTime.wHour,
		localTime.wMinute,
		localTime.wSecond,
		szExt[m_dlgPicSetting.m_nType]);
	if(!CPlayer::Instance()->SnapPicture(szShortFileName, m_dlgPicSetting.m_nType))
		AfxMessageBox(LANG_CS("Snap failed!"));
}

void CPlayDemoDlg::OnVerticalSync()
{
	UINT nCheckFlag = GetMenu()->GetMenuState(IDM_SETTING_VERTICALSYNC, MF_CHECKED) & MF_CHECKED;
	bool bSetCheck = nCheckFlag>0?FALSE:TRUE;
	UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(IDM_SETTING_VERTICALSYNC,  nFlags);

	CPlayer::Instance()->SetVerticalSync(bSetCheck);
}

void CPlayDemoDlg::OnIVS()
{
	/*UINT nCheckFlag = GetMenu()->GetMenuState(IDM_SETTING_IVS, MF_CHECKED) & MF_CHECKED;
	BOOL bSetCheck = nCheckFlag>0?FALSE:TRUE;
	UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(IDM_SETTING_IVS,  nFlags);

	CPlayer::Instance()->RenderPrivateData(bSetCheck);*/
}

BOOL CPlayDemoDlg::PreTranslateMessage(MSG* pMsg) 
{
	float SPEED      = 0.5f;

	// TODO: Add your specialized code here and/or call the base class
	if(pMsg->wParam == VK_ESCAPE && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
		return 1;
	else if (pMsg->wParam == VK_RETURN && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
		return 1;
	else if(::TranslateAccelerator(GetSafeHwnd(), m_hotKey, pMsg))
		return 1;
	else if (pMsg->wParam == 87 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;
			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_BACK, &m_dTotalDistanceZ);

			m_dTotalDistanceZ -= (double)nDistance;
			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_BACK, m_dTotalDistanceZ);
		}
	}
	//s
	else if (pMsg->wParam == 83 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;

			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_BACK, &m_dTotalDistanceZ);
			m_dTotalDistanceZ += (double)nDistance;

			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_BACK, m_dTotalDistanceZ);
		}
	}
	//A
	else if (pMsg->wParam == 65 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;

			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_RIGHT, &m_dTotalDistanceX);

			//物体往左相当于相机往右
			m_dTotalDistanceX += (double)nDistance;
			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_RIGHT, m_dTotalDistanceX);
		}
	}
	//D
	else if (pMsg->wParam == 68 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;

			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_RIGHT, &m_dTotalDistanceX);

			m_dTotalDistanceX -= (double)nDistance;
			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_RIGHT, m_dTotalDistanceX);
		}
	}
	else if (pMsg->wParam == VK_UP && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;

			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_TOP, &m_dTotalDistanceY);

			m_dTotalDistanceY -= (double)nDistance;

			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_TOP, m_dTotalDistanceY);
		}
	}
	else if (pMsg->wParam == VK_DOWN && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		if (m_nLastTime <= 0.005)
		{
			m_nNowTime = ((double)GetTickCount()) / 1000;
			m_nLastTime = m_nNowTime;
		}
		else
		{
			m_nNowTime =((double) GetTickCount()) / 1000;

			if (m_nNowTime < (m_nLastTime + 0.005))
			{
				return 1;
			}
			double nDistance = (m_nNowTime - m_nLastTime) *SPEED;
			m_nLastTime = m_nNowTime;
			PLAY_GetDoubleRegion(0,0, RENDER_STEREO_EYE_MOVE_TOP, &m_dTotalDistanceY);

			m_dTotalDistanceY += (double)nDistance;

			PLAY_SetStereoEyeMoveDistance(0, 0, STEREO_EYE_MOVE_TOP, m_dTotalDistanceY);
		}
	}
	//o
	else if (pMsg->wParam == 79 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		double dFovy = 0.0;
		PLAY_GetDoubleRegion(0,0, RENDER_STEREO_PERSPECTIVE_FOVY, &dFovy);

		dFovy -= (double)2;
		if(dFovy > 10)
		{
			PLAY_SetStereoPerspectiveFovy(0, 0, dFovy);
		}
	}
	//i
	else if (pMsg->wParam == 73 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		double dFovy = 0.0;
		PLAY_GetDoubleRegion(0,0, RENDER_STEREO_PERSPECTIVE_FOVY, &dFovy);

		dFovy += (double)2;
		// TODO: 在此添加控件通知处理程序代码
		if(dFovy < 180)
		{
			PLAY_SetStereoPerspectiveFovy(0, 0, dFovy);
		}
	}
	//p，普通模式观察
	else if (pMsg->wParam == 80 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		PLAY_SetStereoViewMode(0, 0, STEREO_COMMON_VIEW);
	}
	//n，内视点观察
	else if (pMsg->wParam == 78 && (pMsg->message == WM_KEYDOWN || pMsg->message == WM_SYSKEYDOWN))
	{
		PLAY_SetStereoViewMode(0, 0, STEREO_INNER_VIEW);
	}
	else if (pMsg->message == WM_KEYUP || pMsg->message == WM_SYSKEYUP)
	{
		m_nLastTime = 0;
		m_nNowTime = 0;
	}
	else
	{
		if (::IsWindow(m_ContentTip.GetSafeHwnd()))
		{
			m_ContentTip.RelayEvent(pMsg);
		}
		return CDialog::PreTranslateMessage(pMsg);
	}
}

void ChangePlayButtonUIState(CHoverButton* lpUI[STATE_SIZE] , int nState[9])
{
	for(int i = 0 ; i < STATE_SIZE; i++)
	{
		if(nState[i]!=2)
		{
			lpUI[i]->EnableWindowEx(nState[i]);
		}
	}
}

//Change UI State
int CPlayDemoDlg::ChangeUIState(PLAY_STATE nState)
{
	CHoverButton* lpUI[STATE_SIZE] = { &m_bnPlay, &m_bnPause, &m_bnStop, &m_bnToEnd, &m_bnToBegin,
		&m_bnBackOne, &m_bnStepOne, &m_bnSlow, &m_bnFast };
	
	if(nState == Open)
	{
		m_bnSetColor.EnableWindowEx(TRUE);
		m_bnAudio.EnableWindow(TRUE);
		m_sdAudioWave.EnableWindow(TRUE);
		m_sdAuidoVolume.EnableWindow(TRUE);
		ChangePlayButtonUIState(lpUI, CPlayer::Instance()->openTable);
		ChangeMenuState(FILEOPEN);
	}
	else if(nState == Close)
	{
		m_bnPicCatch.EnableWindowEx(FALSE);
		m_bnSetColor.EnableWindowEx(FALSE);
		m_bnAudio.EnableWindow(FALSE);
		m_sdAudioWave.EnableWindow(FALSE);
		m_sdAuidoVolume.EnableWindow(FALSE);
		ChangePlayButtonUIState(lpUI, CPlayer::Instance()->closeTable);
		ChangeMenuState(FILECLOSE);
		m_dlgDisplay.Invalidate(TRUE);
	}
	else if(nState>=Play && nState<=Fast)
	{
		m_bnPicCatch.EnableWindowEx(nState != Stop ? TRUE : FALSE);
		if(nState==Stop)
		{
			m_dlgDisplay.Invalidate(TRUE);
			ChangeMenuState(STOPPLAY);
		}

		if (nState == Play)
		{
			ChangeMenuState(STARTPLAY);
		}

		ChangePlayButtonUIState(lpUI, CPlayer::Instance()->stateTable[nState]);
	}
	
	m_lastState = nState;
	return 1;
}

int CPlayDemoDlg::ChangeSingleUIState(PLAY_STATE nState, BOOL bEnable)
{
	CHoverButton* lpUI[STATE_SIZE] = { &m_bnPlay, &m_bnPause, &m_bnStop,
		&m_bnToEnd, &m_bnToBegin, &m_bnBackOne, 
		&m_bnStepOne, &m_bnSlow, &m_bnFast };
	lpUI[nState]->EnableWindowEx(bEnable);
	return 1;
}

int CPlayDemoDlg::MediaInit()
{
	DecodeType type = (DecodeType)m_dlgDecodeEngine.GetDecodeType();

	if(type == DECODE_HW_FAST)
		PLAY_SetEngine(0,type,RENDER_NOTSET);
	else
		PLAY_SetEngine(0,type,RENDER_NOTSET);

	ChangeAngle(0);
	/*Audio is on by default.*/
	OpenAudio();

	return 0;
}

int CPlayDemoDlg::OpenAudio()
{
	CPlayer::Instance()->OpenSound(TRUE);
	m_sdAudioWave.EnableWindow(TRUE);
	m_sdAuidoVolume.EnableWindow(TRUE);
	m_sdAuidoVolume.SetPos(CPlayer::Instance()->GetAudioVolume());

	CPlayer::Instance()->SetAudioWave(m_sdAudioWave.GetPos());
	
	m_bnAudio.EnableWindow(TRUE);
	
	return 1;
}

int CPlayDemoDlg::CloseAudio()
{
	CPlayer::Instance()->OpenSound(FALSE);
	m_sdAudioWave.EnableWindow(FALSE);
	m_sdAuidoVolume.EnableWindow(FALSE);
	
	m_bnAudio.EnableWindow(FALSE);
	return 1;
}

void CPlayDemoDlg::OnButtonAudio() 
{
	// TODO: Add your control notification handler code here
	if(m_bnAudio.m_bButtonEnable)
	{
		/*Turn off audio if on*/
		CloseAudio();
	}
	else
	{
		/*Turn on audio if off*/
		OpenAudio();
	}
}

void CPlayDemoDlg::OnButtonSetcolor() 
{
	// TODO: Add your control notification handler code here
	m_dlgSetColor.DoModal();
}

void CPlayDemoDlg::OnButtonBackword()
{
	CPlayer::Instance()->SetPlayDirection(1);
}

void CPlayDemoDlg::OnButtonForword()
{
	CPlayer::Instance()->SetPlayDirection(0);
}

void CPlayDemoDlg::OnDropFiles(HDROP hDropInfo)
{
	TCHAR szFile[1024] = {0};
	if( DragQueryFile(hDropInfo, 0, szFile, 1024)<=0 )
	{
		return;
	}
	
	OnFileClose();
	m_dlgOpenFile.m_strFile = szFile;

	int pos = -1;
	TCHAR* pbuffer = m_dlgOpenFile.m_strFile.GetBuffer(m_dlgOpenFile.m_strFile.GetLength());
	if ( -1 != (pos = m_dlgOpenFile.m_strFile.ReverseFind('.')))
	{
		pbuffer += pos;
	}
	
	//asf,MP4 only support file mode
	if (0 == _tcscmp(pbuffer, _T(".asf")) || 0 == _tcscmp(pbuffer, _T(".mp4")))
	{
		m_dlgOpenFile.m_nType = 0;
	}
	
	OpenFile();
}

void CPlayDemoDlg::OnSettingStartFisheye()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	UINT nCheckFlag = GetMenu()->GetMenuState(ID_SETTING_START_FISHEYE, MF_CHECKED) & MF_CHECKED;
	BOOL bSetCheck = nCheckFlag>0?FALSE:TRUE;
	UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SETTING_START_FISHEYE,  nFlags);

	CPlayer::Instance()->ControlFishEye();
}

void CPlayDemoDlg::OnSettingDecodeEngine()
{
	m_dlgDecodeEngine.DoModal();
}

void CPlayDemoDlg::OnFisheyeviewCeil1pplus1()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_CEIL_1PPLUS1,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_CEIL_1PPLUS1;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, FISHEYECALIBRATE_MODE_PANORAMA_PLUS_ONE_EPTZ);
}

void CPlayDemoDlg::OnFisheyeviewCeil1plus3()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_CEIL_1PLUS3,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_CEIL_1PLUS3;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, FISHEYECALIBRATE_MODE_ORIGINAL_PLUS_THREE_EPTZ_REGION);
}

void CPlayDemoDlg::OnFisheyeviewFloor2p()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_FLOOR_2P,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_FLOOR_2P;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, FISHEYECALIBRATE_MODE_DOUBLE_PANORAMA);
}

void CPlayDemoDlg::OnFisheyeviewFloor1plus2()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_FLOOR_1PLUS2,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_FLOOR_1PLUS2;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, FISHEYECALIBRATE_MODE_ORIGINAL_PLUS_TWO_EPTZ_REGION);
}

void CPlayDemoDlg::OnFisheyeviewFloor1pplus6()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_FLOOR_1PPLUS6,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_FLOOR_1PPLUS6;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, FISHEYECALIBRATE_MODE_PANORAMA_PLUS_SIX_EPTZ_REGION);
}

void CPlayDemoDlg::OnFisheyeviewWall1pplus8()
{
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_WALL_1PPLUS8,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_WALL_1PPLUS8;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_WALL, FISHEYECALIBRATE_MODE_PANORAMA_PLUS_EIGHT_EPTZ_REGION);
}


void CPlayDemoDlg::ChangeAngle(int type)
{
	if(type == m_DisplayAngle)
		return;
	if(type == 4)
		m_DisplayAngle = 0;
	else if(type == -1)
		m_DisplayAngle = 3;
	else
		m_DisplayAngle = type;
	PLAY_SetRotateAngle(0,m_DisplayAngle);
	UINT nFlags;
	nFlags = MF_BYCOMMAND | (m_DisplayAngle == 0?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_ROTATEANGLE_0,  nFlags);
	nFlags = MF_BYCOMMAND | (m_DisplayAngle == 1?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_ROTATEANGLE_90,  nFlags);
	nFlags = MF_BYCOMMAND | (m_DisplayAngle == 2?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_ROTATEANGLE_180,  nFlags);
	nFlags = MF_BYCOMMAND | (m_DisplayAngle == 3?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_ROTATEANGLE_270,  nFlags);
}

void CPlayDemoDlg::OnSetThrowMode(UINT ID)
{
	int nThrowFlags = -1;
	switch(ID)
	{
	case ID_THROW_NO:
		nThrowFlags = 0;
		m_nThrowMode = ID_THROW_NO;
		CPlayer::Instance()->EnableLargePicAdjustment(0); //0为不抽帧
		break;
	case ID_THROW_DEFAULT:
		nThrowFlags = 1;
		m_nThrowMode = ID_THROW_DEFAULT;
		CPlayer::Instance()->EnableLargePicAdjustment(1); // 1为默认策略抽帧
		break;
	case ID_THROW_ADAPTION:
		nThrowFlags = 3;
		m_nThrowMode = ID_THROW_ADAPTION;
		CPlayer::Instance()->EnableLargePicAdjustment(3); //3为自适应抽帧
		break;
	default:
		break;
	}

	if (nThrowFlags != -1)
	{
		int nFlags = 0;
		nFlags = MF_BYCOMMAND | (nThrowFlags == 0?MF_CHECKED:MF_UNCHECKED);
		GetMenu()->CheckMenuItem(ID_THROW_NO,  nFlags);

		nFlags = MF_BYCOMMAND | (nThrowFlags == 1?MF_CHECKED:MF_UNCHECKED);
		GetMenu()->CheckMenuItem(ID_THROW_DEFAULT,  nFlags);

		nFlags = MF_BYCOMMAND | (nThrowFlags == 3?MF_CHECKED:MF_UNCHECKED);
		GetMenu()->CheckMenuItem(ID_THROW_ADAPTION,  nFlags);
	}

	return;
}

void CPlayDemoDlg::OnRotateAngle(UINT ID)
{
	switch(ID)
	{
		case ID_ROTATEANGLE_0:
		case ID_ROTATEANGLE_90:
		case ID_ROTATEANGLE_180:
		case ID_ROTATEANGLE_270:
			ChangeAngle(ID - ID_ROTATEANGLE_0);
			break;
		case ID_ROTATEANGLE_ROTATELEFT:
		case ID_ACCELERATOR_F7:
			ChangeAngle(m_DisplayAngle - 1);
			break;
		case ID_ROTATEANGLE_ROTATERIGHT:
		case ID_ACCELERATOR_F8:
			ChangeAngle(m_DisplayAngle + 1);
			break;
	}
}

void CPlayDemoDlg::OnSettingThrowframe()
{
	UINT nCheckFlag = GetMenu()->GetMenuState(ID_SETTING_THROWFRAME, MF_CHECKED) & MF_CHECKED;
	BOOL bSetCheck = nCheckFlag>0?FALSE:TRUE;
	UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SETTING_THROWFRAME,  nFlags);

	//0为不抽帧，1为720P以上抽帧
	CPlayer::Instance()->EnableLargePicAdjustment(bSetCheck);
}


void CPlayDemoDlg::OnDevCase()
{
#if (!defined _WIN64) && (defined _DEBUG)
	// 64 地址强转会出现内存问题。暂时不只是win64
	CCmdDialog().DoModal();
#endif

}
void CPlayDemoDlg::OnFisheyeviewSemi()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_SEMI,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_SEMI;

	//CPlayer::Instance()->SetFishEyeMode((FISHEYE_MOUNTMODE)m_lastSemiMount, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_SEMI_SPHERE));

}

void CPlayDemoDlg::OnFisheyeviewCylinder()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_CYLINDER,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_CYLINDER;

//	CPlayer::Instance()->SetFishEyeMode((FISHEYE_MOUNTMODE)m_lastSemiMount, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_CYLINDER));
}

void CPlayDemoDlg::OnFisheyeviewLittle()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_LITTLE,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_LITTLE;

	//CPlayer::Instance()->SetFishEyeMode((FISHEYE_MOUNTMODE)m_lastSemiMount, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_LITTLE_PLANET));
}

void CPlayDemoDlg::OnFisheyeview360Sphere()
{
	// TODO: 在此添加命令处理程序代码
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_360_SHPERE,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_360_SHPERE;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_DOUBLE_SPHERE));

}

void CPlayDemoDlg::OnFisheyeview360Cylinder()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_FISHEYEVIEW_360_CYLINDER,  MF_CHECKED);

	m_lastFisheyeMode = ID_FISHEYEVIEW_360_CYLINDER;

	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_DOUBLE_CYLINDER));
}

void CPlayDemoDlg::OnSize(UINT nType, int cx, int cy) 
{
	UPDATE_EASYSIZE;
	CDialog::OnSize(nType, cx, cy);
	
	CWnd* pBoder = GetDlgItem(IDC_STATIC_PLAY);
	if (pBoder && pBoder->GetSafeHwnd())
	{
		CRect rBoder;
		
		pBoder->GetWindowRect(&rBoder);
		pBoder->ScreenToClient(&rBoder);
		m_dlgDisplay.MoveWindow(&rBoder);
	}
} 

void CPlayDemoDlg::OnAntiAliasing()
{
	UINT nCheckFlag = GetMenu()->GetMenuState(ID_SETTING_ANTI, MF_CHECKED) & MF_CHECKED;
	bool bSetCheck = nCheckFlag>0?FALSE:TRUE;
	UINT nFlags = MF_BYCOMMAND | (bSetCheck?MF_CHECKED:MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SETTING_ANTI,  nFlags);

	CPlayer::Instance()->SetAntiAliasing(bSetCheck);
}

void CPlayDemoDlg::OnSemiCeil()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SEMI_CEIL,  MF_CHECKED);

	m_lastFisheyeMode = ID_SEMI_CEIL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_SEMI_SPHERE));

}

void CPlayDemoDlg::OnSemiFloor()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SEMI_FLOOR,  MF_CHECKED);

	m_lastFisheyeMode = ID_SEMI_FLOOR;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_SEMI_SPHERE));

}

void CPlayDemoDlg::OnSemiWall()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_SEMI_WALL,  MF_CHECKED);

	m_lastFisheyeMode = ID_SEMI_WALL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_WALL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_SEMI_SPHERE));
}

void CPlayDemoDlg::OnCylinderCeil()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_CYLINDER_CEIL,  MF_CHECKED);

	m_lastFisheyeMode = ID_CYLINDER_CEIL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_CYLINDER));
}

void CPlayDemoDlg::OnCylinderFloor()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_CYLINDER_FLOOR,  MF_CHECKED);

	m_lastFisheyeMode = ID_CYLINDER_FLOOR;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_CYLINDER));

}

void CPlayDemoDlg::OnCylinderWall()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_CYLINDER_WALL,  MF_CHECKED);

	m_lastFisheyeMode = ID_CYLINDER_WALL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_WALL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_CYLINDER));

}

void CPlayDemoDlg::OnLittleCeil()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_LITTLE_CEIL,  MF_CHECKED);

	m_lastFisheyeMode = ID_LITTLE_CEIL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_CEIL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_LITTLE_PLANET));

}

void CPlayDemoDlg::OnLittleFloor()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_LITTLE_FLOOR,  MF_CHECKED);

	m_lastFisheyeMode = ID_LITTLE_FLOOR;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_FLOOR, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_LITTLE_PLANET));

}

void CPlayDemoDlg::OnLittleWall()
{
	// TODO: 在此添加命令处理程序代码
	GetMenu()->CheckMenuItem(m_lastFisheyeMode, MF_UNCHECKED);
	GetMenu()->CheckMenuItem(ID_LITTLE_WALL,  MF_CHECKED);

	m_lastFisheyeMode = ID_LITTLE_WALL;
	CPlayer::Instance()->SetFishEyeMode(FISHEYEMOUNT_MODE_WALL, (FISHEYE_CALIBRATMODE)(FISHEYECALIBRATE_MODE_LITTLE_PLANET));

}

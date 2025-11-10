// WaterCheck_demoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "WaterCheck_demo.h"
#include "WaterCheck_demoDlg.h"
#include "dhplay.h"
#include "LanguageConvertor.h"
#include <shlwapi.h>
#include "CharactorTansfer.h"
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CWaterCheck_demoDlg dialog
#define BUF_SIZE 1024*1024

#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "dhplay.lib")

CWaterCheck_demoDlg::CWaterCheck_demoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CWaterCheck_demoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CWaterCheck_demoDlg)
	m_csfilePath = _T("");
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_hThread = NULL;
	m_hExit = CreateEvent(NULL, TRUE, FALSE, NULL);
	LANG_INIT();
}
CWaterCheck_demoDlg::~CWaterCheck_demoDlg()
{
	SetEvent(m_hExit);
	CloseHandle(m_hExit);
	if(m_hThread)
		CloseHandle(m_hThread);

}

BOOL CWaterCheck_demoDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if(pMsg->wParam == VK_ESCAPE && pMsg->message == WM_KEYDOWN)
		return 1;
	else if (pMsg->wParam == VK_RETURN && pMsg->message == WM_KEYDOWN)
		return 1;
	else
		return CDialog::PreTranslateMessage(pMsg);
}

void CWaterCheck_demoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWaterCheck_demoDlg)
	DDX_Control(pDX, IDC_LIST_WATERINFO, m_lscheckInfoList);
	DDX_Text(pDX, IDC_EDIT_FILEPATH, m_csfilePath);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CWaterCheck_demoDlg, CDialog)
	//{{AFX_MSG_MAP(CWaterCheck_demoDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_CHECK, OnButtonCheck)
	ON_BN_CLICKED(IDC_BUTTON_FILEPATH, OnButtonFilepath)
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_BN_CLICKED(IDC_BUTTON_STOP, OnButtonStop)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWaterCheck_demoDlg message handlers

BOOL CWaterCheck_demoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here	

	LANG_SETWNDSTATICTEXT(this);

	GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(FALSE);

	// Init Track List
	DWORD dwStyle=::GetWindowLong(m_lscheckInfoList.m_hWnd,GWL_STYLE);
	//Set to report form
	SetWindowLong(m_lscheckInfoList.m_hWnd,GWL_STYLE,dwStyle|LVS_REPORT);
	DWORD ExStyle =m_lscheckInfoList.GetExtendedStyle();
	//Set to full field selection and grid line
	m_lscheckInfoList.SetExtendedStyle(ExStyle|LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES);
	
	m_lscheckInfoList.InsertColumn(0, LANG_CS("Num"), LVCFMT_LEFT, 40);
	m_lscheckInfoList.InsertColumn(1, LANG_CS("Error Type"), LVCFMT_LEFT, 200);
	m_lscheckInfoList.InsertColumn(2, LANG_CS("Time Stamp"), LVCFMT_LEFT, 150);
	
	((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetRange(0, 1000);

	ChangeUIstate(INIT);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CWaterCheck_demoDlg::OnPaint() 
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
HCURSOR CWaterCheck_demoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CWaterCheck_demoDlg::OnButtonFilepath() 
{
	// TODO: Add your control notification handler code here
	CFileDialog fileChooser(TRUE, NULL, NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, _T("All files(*.*)|*.*||"));
	if (fileChooser.DoModal() == IDOK)
	{
		m_csfilePath = fileChooser.GetPathName();
	}
	SetDlgItemText(IDC_EDIT_FILEPATH, m_csfilePath);
	if(m_csfilePath != "")
	{
		ChangeUIstate(OPENFILE);
	}
}

DWORD WINAPI watermarkCheckPro(LPVOID lParam)
{
	CWaterCheck_demoDlg *dlg = (CWaterCheck_demoDlg *)lParam;
	dlg->watermarkCheck();
	return 0;
}

//convert time stamp info
bool GetTimeStamp(long lTimeStamp, CString &csTimeStamp)
{
	long long ltime = static_cast<long long>(lTimeStamp);
	tm *tempTime = localtime((time_t*)&ltime);
	if(tempTime == 0)
		return false;
	int nHour = tempTime->tm_hour;
	int nMinute = tempTime->tm_min;
	int nSec = tempTime->tm_sec;
	int nDay = tempTime->tm_mday;
	int nMonth = tempTime->tm_mon + 1;
	int nYear = tempTime->tm_year + 1900;
	csTimeStamp.Format(_T("%d-%02d-%02d-%02d-%02d-%02d"), nYear, nMonth, nDay, nHour, nMinute, nSec);
	return true;
}	

int CALLBACK watermarkCheckCBFunc(long nPort, char* buf, long lTimeStamp, long lInfoType, long len, long reallen, long lCheckResult, void* pUserData)
{
	CWaterCheck_demoDlg *dlg = (CWaterCheck_demoDlg *)pUserData;
	CString csErrorType;
	CString csItemNum;
	CString csTimeStamp;
	switch(lCheckResult)
	{
		//no error:
	case 1:
		{
			if (lInfoType == WATERMARK_DATA_TEXT)
			{
				buf[reallen] = '\0';
				std::string waterMark(buf);
				//UTF-8תunicode
				int len = MultiByteToWideChar(CP_UTF8, 0, waterMark.c_str(), -1, NULL, 0);
				wchar_t * strUnicode = new wchar_t[len];//len = 2
				wmemset(strUnicode, 0, len);
	            MultiByteToWideChar(CP_UTF8, 0, waterMark.c_str(), -1, strUnicode, len);
				dlg->GetDlgItem(IDC_EDIT_WATERMARKINFO)->SetWindowText(strUnicode);
			}
		}
		break;
		//watermark error
	case 2:
		{
			csErrorType = LANG_CS("Watermark verify error");
			GetTimeStamp(lTimeStamp, csTimeStamp);
			int errorNum = dlg->m_lscheckInfoList.GetItemCount();
			csItemNum.Format(_T("%d"), errorNum);
			dlg->m_lscheckInfoList.InsertItem(errorNum, csItemNum);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 1, csErrorType);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 2, csTimeStamp);	
		}
		break;
		//frame data error
	case 3:
		{
			csErrorType = LANG_CS("Frame data verify error");
			GetTimeStamp(lTimeStamp, csTimeStamp);
			int errorNum = dlg->m_lscheckInfoList.GetItemCount();
			csItemNum.Format(_T("%d"), errorNum);
			dlg->m_lscheckInfoList.InsertItem(errorNum, csItemNum);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 1, csErrorType);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 2, csTimeStamp);
		}
		break;
		//Frame number discontinuity
	case 4:
		{
			csErrorType = LANG_CS("Frame number discontinuity");
			GetTimeStamp(lTimeStamp, csTimeStamp);
			int errorNum = dlg->m_lscheckInfoList.GetItemCount();
			csItemNum.Format(_T("%d"), errorNum);
			dlg->m_lscheckInfoList.InsertItem(errorNum, csItemNum);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 1, csErrorType);
			dlg->m_lscheckInfoList.SetItemText(errorNum, 2, csTimeStamp);
		}
		break;
	default:
		break;
	}
	return 0;
}

//Adjust UI status
void CWaterCheck_demoDlg::ChangeUIstate(int nState)
{
	if(nState == INIT)
	{
		GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(0);
		GetDlgItem(IDC_EDIT_WATERMARKINFO)->SetWindowText(_T(""));
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(TRUE);
	}
	else if(nState == OPENFILE)
	{
		GetDlgItem(IDC_EDIT_WATERMARKINFO)->SetWindowText(_T(""));	
		GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		m_lscheckInfoList.DeleteAllItems();
	}
	else if(nState == STOP)
	{
		GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(0);
		GetDlgItem(IDC_EDIT_WATERMARKINFO)->SetWindowText(_T(""));
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(TRUE);
		m_lscheckInfoList.DeleteAllItems();
	}
	else if(nState == CHECK)
	{
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_WATERMARKINFO)->SetWindowText(_T(""));
		m_lscheckInfoList.DeleteAllItems();
	}
	else
	{
		GetDlgItem(IDC_BUTTON_CHECK)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(0);
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(TRUE);
	}
}

void CWaterCheck_demoDlg::watermarkCheck()
{
	PLAY_SetStreamOpenMode(0, STREAME_FILE);
	PLAY_OpenStream(0, NULL, 0, BUF_SIZE);	
	PLAY_Play(0, NULL);
	PLAY_SetWaterMarkCallBackEx(0, watermarkCheckCBFunc, this);

	DWORD dwFileSize = m_checkFile.GetLength();
	const int READ_SIZE = 128*1024;
	BYTE pBuf[READ_SIZE];
	DWORD dwReadPos = 0;

	BOOL bInput = TRUE;
	DWORD dwReadSize = 0;
	while(WaitForSingleObject(m_hExit, 5)!=WAIT_OBJECT_0)
	{
		if(bInput)
		{
			((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(((double)dwReadPos*1000)/(double)dwFileSize);
			dwReadSize = m_checkFile.Read(pBuf, READ_SIZE);
			if(dwReadSize<=0)
				break;
			dwReadPos += dwReadSize;
			
		}
		
		bInput = PLAY_InputData(0, pBuf, dwReadSize);
	}

	while ( (PLAY_GetBufferValue(0, BUF_VIDEO_RENDER) + PLAY_GetSourceBufferRemain(0)) > 0 )
	{	
		Sleep(5);
	}

	PLAY_Stop(0);
	PLAY_CloseStream(0);
	m_checkFile.Close();
	
	ChangeUIstate(COMPLETE);
}

void CWaterCheck_demoDlg::OnButtonCheck() 
{
	// TODO: Add your control notification handler code here
	if(!UpdateData(TRUE))
		return;

	if (!PathFileExists(m_csfilePath))
	{
		MessageBox(LANG_CS("Input file error!"));
		return;
	}
	
	if (!m_checkFile.Open(m_csfilePath, CFile::modeRead | CFile::shareDenyNone))
	{
		MessageBox(LANG_CS("open file failed!"));
		return;
	}

	DWORD dwID = 0;
	
	ResetEvent(m_hExit);
	
	m_hThread = CreateThread(NULL, 0, watermarkCheckPro, this, 0, &dwID);
	
	ChangeUIstate(CHECK);
}

void CWaterCheck_demoDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	OnButtonStop();
	CDialog::OnClose();
}

void CWaterCheck_demoDlg::OnButtonStop() 
{
	// TODO: Add your control notification handler code here
	SetEvent(m_hExit);
	CloseHandle(m_hThread);
	m_hThread = NULL;

}

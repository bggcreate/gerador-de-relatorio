// AviConvert_DemoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "AviConvert_Demo.h"
#include "AviConvert_DemoDlg.h"
#include "LanguageConvertor.h"
#include <shlwapi.h>
#include "CharactorTansfer.h"
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define SOURCEBUF_SIZE 500*1024
/////////////////////////////////////////////////////////////////////////////
// CAviConvert_DemoDlg dialog

CAviConvert_DemoDlg::CAviConvert_DemoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CAviConvert_DemoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CAviConvert_DemoDlg)
	m_nConvertType = AVI_CONVERT;
	m_csSourceFile = "";
	m_csTargetFile = "";
	m_nChangeCount = 0;
	m_bConverting = FALSE;
	m_hconvertThread = NULL;
	m_lheight = 0;
	m_lwidth = 0;
	m_lwidth = 0;
	m_lheight = 0;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	LANG_INIT();
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CAviConvert_DemoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAviConvert_DemoDlg)
	DDX_Radio(pDX, IDC_RADIO_AVI, m_nConvertType);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAviConvert_DemoDlg, CDialog)
	//{{AFX_MSG_MAP(CAviConvert_DemoDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_SRCPATH, OnButtonSrcpath)
	ON_BN_CLICKED(IDC_BUTTON_DESTPATH, OnButtonDestpath)
	ON_BN_CLICKED(IDC_BUTTON_CONVERT, OnButtonConvert)
	ON_BN_CLICKED(IDC_BUTTON_CANCEL, OnButtonCancel)
	ON_BN_CLICKED(IDC_RADIO_ASF, OnRadioAsf)
	ON_BN_CLICKED(IDC_RADIO_AVI, OnRadioAvi)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_RADIO_MP4, OnRadioMp4)
    ON_BN_CLICKED(IDC_RADIO_PS, OnBnClickedRadioPs)
    ON_BN_CLICKED(IDC_RADIO_TS, OnBnClickedRadioTs)
	ON_MESSAGE(UM_MESSAGEPRESENT, &CAviConvert_DemoDlg::OnPresent)
	ON_MESSAGE(UM_MESSAGECOMPLETE, &CAviConvert_DemoDlg::OnComplete)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CAviConvert_DemoDlg message handlers

BOOL CAviConvert_DemoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here
	((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetRange(0, 500);
	((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(0);
	GetDlgItem(IDC_BUTTON_CANCEL)->EnableWindow(FALSE);
	GetDlgItem(IDC_BUTTON_CONVERT)->EnableWindow(TRUE);
	
	LANG_SETWNDSTATICTEXT(this);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CAviConvert_DemoDlg::OnPaint() 
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
HCURSOR CAviConvert_DemoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CAviConvert_DemoDlg::OnButtonSrcpath() 
{
	// TODO: Add your control notification handler code here
	CFileDialog FileChooser(TRUE, NULL, NULL, 
		OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, _T("*.dav|*.dav|"));
	if (FileChooser.DoModal() != IDOK || !PathFileExists(FileChooser.GetPathName()))
	{
		return;
	}

	m_csSourceFile = FileChooser.GetPathName();
	GetDlgItem(IDC_EDIT_SOURCEPATH)->SetWindowText(m_csSourceFile);
	m_csTargetFile = m_csSourceFile;
	
	//Change file suffix
	CString szExt = "";
	if(m_nConvertType == AVI_CONVERT)
    {
		szExt += ".avi";
    }
	else if (m_nConvertType == ASF_CONVERT)
    {
		szExt += ".asf";
    }
    else if (m_nConvertType == MP4_CONVERT)
    {
        szExt += ".mp4";
    }
    else if (m_nConvertType == PS_CONVERT)
    {
        szExt += ".ps";
    }
    else if (m_nConvertType == TS_CONVERT)
    {
        szExt += ".ts";
    }

	m_csTargetFile.Replace(CString(".")+FileChooser.GetFileExt(), szExt);
	GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);	
}

void CAviConvert_DemoDlg::OnButtonDestpath() 
{
	// TODO: Add your control notification handler code here

	TCHAR szTitle[] = _T("folder");
	TCHAR szDisplayName[MAX_PATH] =_T("");
	TCHAR szPath[MAX_PATH] =_T("");
	BROWSEINFO bi;
	bi.hwndOwner = GetSafeHwnd();
	bi.pidlRoot = NULL;
	bi.lpszTitle = szTitle;
	bi.pszDisplayName = szDisplayName;
	bi.ulFlags = BIF_RETURNONLYFSDIRS|BIF_BROWSEINCLUDEFILES;
	bi.lpfn = NULL;
	bi.lParam = 0;
	LPITEMIDLIST pItemIDList = SHBrowseForFolder(&bi);
	
	if (pItemIDList)
	{
		SHGetPathFromIDList(pItemIDList, szPath);
		IMalloc *pMalloc;
		if (SHGetMalloc(&pMalloc) != NOERROR)
		{
			AfxMessageBox(LANG_CS("path choose failed!"));
			return;
		}
		int pos = 0;
		pos = m_csSourceFile.ReverseFind('\\');
		int filelen = m_csSourceFile.GetLength();
		CString m_fSrcFileName = m_csSourceFile.Mid(pos+1, filelen+1);
		pos = m_fSrcFileName.Find(_T(".dav"));
		if(pos != -1)
			m_fSrcFileName = m_fSrcFileName.Mid(0, pos);
		m_csTargetFile = szPath + CString("\\") + m_fSrcFileName + ((m_nConvertType == AVI_CONVERT)?".avi":".asf");
		SetDlgItemText(IDC_EDIT_DESTPATH, m_csTargetFile);
		pMalloc->Free(pItemIDList);
		if(pMalloc)
			pMalloc->Release();
	}
}

void CAviConvert_DemoDlg::AviConvert()
{
	if(!StartConvert())
		return;

	unsigned long long fileLen = m_fSrcFile.GetLength();
	unsigned long long readPos = 0;
	const int readlen = 8*1024;
	BYTE readBuf[readlen];
	
	//input data
	while(m_bConverting)
	{
		TRACE(_T("while\n"));
		DWORD nRead = m_fSrcFile.Read(readBuf, readlen);
		if (nRead <= 0)
			break;
		while(!PLAY_InputData(0, readBuf, readlen))
			Sleep(10);
		
		readPos += nRead;
		double proPercent = (double)readPos/(double)fileLen;
		DWORD progressPos = proPercent*(500.0);
		if(m_bConverting)
		{
			TRACE(_T("SetPos\n"));
			PostMessageA(m_hWnd, UM_MESSAGEPRESENT, 0, progressPos);//发送完成消息	
		}
	}
	
	while ((PLAY_GetBufferValue(0, BUF_VIDEO_RENDER) + PLAY_GetSourceBufferRemain(0)) > 0)
	{
		Sleep(5);
	}
	StopConvert();
    m_fSrcFile.Close();
	PostMessageA(m_hWnd, UM_MESSAGECOMPLETE, 0, 0);//发送结束消息
}

int CAviConvert_DemoDlg::ConvertType(int nDataType)
{
    int nConvertType = 0;
    switch (nDataType)
    {
    case AVI_CONVERT:
        nConvertType = DATA_RECORD_AVI;
        break;
    case ASF_CONVERT:
        nConvertType = DATA_RECORD_ASF;
        break;
    case MP4_CONVERT:
        nConvertType = DATA_RECORD_MP4;
        break;
    case PS_CONVERT:
        nConvertType = DATA_RECORD_PS;
        break;
    case TS_CONVERT:
        nConvertType = DATA_RECORD_TS;
        break;
    }
    return nConvertType;
}

bool CAviConvert_DemoDlg::StartConvert()
{
	if(!m_fSrcFile.Open(m_csSourceFile, CFile::modeRead | CFile::shareDenyNone))
	{
		MessageBox(LANG_CS("Open source file Failed!"));
		m_bConverting = FALSE;
		return false;
	}
	
	PLAY_SetStreamOpenMode(0, STREAME_FILE);
	PLAY_OpenStream(0, NULL, 0, SOURCEBUF_SIZE);
	PLAY_Play(0, NULL);
	
	BOOL bRet = FALSE;
	std::string strFileNameA = UnicodeToGbk(m_csTargetFile.GetBuffer(0));
	bRet = PLAY_StartDataRecord(0, (char *)strFileNameA.c_str(), ConvertType(m_nConvertType));
	if(!bRet)
	{
		MessageBox(LANG_CS("Convert Failed!"));
		m_bConverting = FALSE;
		PLAY_Stop(0);
		PLAY_CloseStream(0);
		m_fSrcFile.Close();
		return false;
	}
	return true;
}

void CAviConvert_DemoDlg::StopConvert()
{
	PLAY_StopDataRecord(0);
	PLAY_Stop(0);
	PLAY_CloseStream(0);
}

DWORD WINAPI aviConvert(LPVOID lParam)
{
	CAviConvert_DemoDlg *dlg = (CAviConvert_DemoDlg *)lParam;
	dlg->AviConvert();
	return 0;
}

void CAviConvert_DemoDlg::OnButtonConvert() 
{
	// TODO: Add your control notification handler code here
	GetDlgItem(IDC_EDIT_SOURCEPATH)->GetWindowText(m_csSourceFile);
	GetDlgItem(IDC_EDIT_DESTPATH)->GetWindowText(m_csTargetFile);
	UpdateData(TRUE);

	if(!PathFileExists(m_csSourceFile))
	{
		MessageBox(LANG_CS("source file error!"));
		return;
	}
	if(m_csTargetFile == "" || m_csSourceFile == m_csTargetFile)
	{
		MessageBox(LANG_CS("target file error!"));
		return;
	}
	
	DWORD threadId = 0;
	if (m_hconvertThread != NULL)
	{
		WaitForSingleObject(m_hconvertThread, INFINITE);
		CloseHandle(m_hconvertThread);
		m_hconvertThread = NULL;
	}
	m_hconvertThread = CreateThread(NULL, NULL, aviConvert, this, FALSE, &threadId);
	m_bConverting = TRUE;
}

void CAviConvert_DemoDlg::ChangeUIState(int nState)
{
	if(nState == CONVERT)
	{
		GetDlgItem(IDC_RADIO_ASF)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_AVI)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_MP4)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_PS)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_TS)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_CANCEL)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_CONVERT)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_DESTPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_SOURCEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_SRCPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_DESTPATH)->EnableWindow(FALSE);
	}
	else if(nState == CANCEL)
	{
		GetDlgItem(IDC_BUTTON_CONVERT)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_CANCEL)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_AVI)->EnableWindow(TRUE);
		GetDlgItem(IDC_RADIO_ASF)->EnableWindow(TRUE);
        GetDlgItem(IDC_RADIO_MP4)->EnableWindow(TRUE);
        GetDlgItem(IDC_RADIO_PS)->EnableWindow(TRUE);
        GetDlgItem(IDC_RADIO_TS)->EnableWindow(TRUE);

		GetDlgItem(IDC_EDIT_DESTPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_SOURCEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_SRCPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_DESTPATH)->EnableWindow(TRUE);
		((CProgressCtrl *)GetDlgItem(IDC_PROGRESS))->SetPos(0);
	}
}
void CAviConvert_DemoDlg::OnButtonCancel() 
{
	// TODO: Add your control notification handler code here
	m_bConverting = FALSE;
	m_nChangeCount = 0;
	ChangeUIState(CANCEL);
}

void CAviConvert_DemoDlg::OnRadioAsf() 
{
	// TODO: Add your control notification handler code here
	if(m_nConvertType == ASF_CONVERT)
		return;

	m_nConvertType = ASF_CONVERT;
	if(m_csTargetFile != "")
	{
		//Change file suffix
		CString tempFileName = m_csTargetFile;
		int pos = m_csTargetFile.ReverseFind('.');
		if(pos > 0)
			tempFileName = m_csTargetFile.Mid(0, pos);
		m_csTargetFile = tempFileName + ".asf";
		GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);
	}
}

void CAviConvert_DemoDlg::OnRadioAvi() 
{
	// TODO: Add your control notification handler code here
	if (m_nConvertType == AVI_CONVERT)
		return;

	m_nConvertType = AVI_CONVERT;	
	if(m_csTargetFile != "")
	{
		//Change file suffix
		int i = m_csTargetFile.GetLength();
		int	pos = m_csTargetFile.ReverseFind('.');
		CString tempFileName = m_csTargetFile;
		if (pos > 0)
			tempFileName = m_csTargetFile.Mid(0, pos);
		m_csTargetFile = tempFileName + ".avi";
		GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);
	}
}

void CAviConvert_DemoDlg::OnRadioMp4() 
{
	// TODO: Add your control notification handler code here
	if (m_nConvertType == MP4_CONVERT)
		return;
	
	m_nConvertType = MP4_CONVERT;	
	if(m_csTargetFile != "")
	{
		//Change file suffix
		int i = m_csTargetFile.GetLength();
		int	pos = m_csTargetFile.ReverseFind('.');
		CString tempFileName = m_csTargetFile;
		if (pos > 0)
			tempFileName = m_csTargetFile.Mid(0, pos);
		m_csTargetFile = tempFileName + ".mp4";
		GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);
	}
}

void CAviConvert_DemoDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	m_bConverting = FALSE;
	if (m_hconvertThread != NULL)
	{
		StopConvert();
		CloseHandle(m_hconvertThread);
		m_hconvertThread = NULL;
	}
	CDialog::OnClose();
}


BOOL CAviConvert_DemoDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if(((pMsg->wParam== VK_ESCAPE) || pMsg->wParam== VK_RETURN) && pMsg->message == WM_KEYDOWN)
		return 0;
	return CDialog::PreTranslateMessage(pMsg);
}

void CAviConvert_DemoDlg::OnBnClickedRadioPs()
{
    if (m_nConvertType == PS_CONVERT)
        return;

    m_nConvertType = PS_CONVERT;	
    if(m_csTargetFile != "")
    {
        //Change file suffix
        int i = m_csTargetFile.GetLength();
        int	pos = m_csTargetFile.ReverseFind('.');
        CString tempFileName = m_csTargetFile;
        if (pos > 0)
            tempFileName = m_csTargetFile.Mid(0, pos);
        m_csTargetFile = tempFileName + ".ps";
        GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);
    }
}

void CAviConvert_DemoDlg::OnBnClickedRadioTs()
{
    if (m_nConvertType == TS_CONVERT)
        return;

    m_nConvertType = TS_CONVERT;	
    if(m_csTargetFile != "")
    {
        //Change file suffix
        int i = m_csTargetFile.GetLength();
        int	pos = m_csTargetFile.ReverseFind('.');
        CString tempFileName = m_csTargetFile;
        if (pos > 0)
            tempFileName = m_csTargetFile.Mid(0, pos);
        m_csTargetFile = tempFileName + ".ts";
        GetDlgItem(IDC_EDIT_DESTPATH)->SetWindowText(m_csTargetFile);
    }
}

LRESULT CAviConvert_DemoDlg::OnPresent(WPARAM mParam, LPARAM lParam)
{
	CProgressCtrl* pProgress = (CProgressCtrl *)GetDlgItem(IDC_PROGRESS);
	ChangeUIState(CONVERT);
	DWORD progressPos = (DWORD)(lParam);
	pProgress->SetPos(progressPos);
	return 0;
}

LRESULT CAviConvert_DemoDlg::OnComplete(WPARAM mParam, LPARAM lParam)
{
	CProgressCtrl* pProgress = (CProgressCtrl *)GetDlgItem(IDC_PROGRESS);

	MessageBox(LANG_CS("Convert complete!"));
	pProgress->SetPos(0);
	ChangeUIState(CANCEL);
	return 0;
}

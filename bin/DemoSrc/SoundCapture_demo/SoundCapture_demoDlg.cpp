// SoundCapture_demoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "SoundCapture_demo.h"
#include "SoundCapture_demoDlg.h"
#include "LanguageConvertor.h"
#include "CharactorTansfer.h"
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma comment(lib, "dhplay.lib")

/////////////////////////////////////////////////////////////////////////////
// CSoundCapture_demoDlg dialog

CSoundCapture_demoDlg::CSoundCapture_demoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CSoundCapture_demoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSoundCapture_demoDlg)
	m_csFileName = _T("");
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_fPCM = NULL;
	LANG_INIT();
}

void CSoundCapture_demoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSoundCapture_demoDlg)
	DDX_Control(pDX, IDC_COMBO_SAMPLEPERSECOND, m_cSamplePerSecondBox);
	DDX_Control(pDX, IDC_COMBO_BITPERSAMPLE, m_cBitPerSampleBox);
	DDX_Text(pDX, IDC_EDIT_FILEPATH, m_csFileName);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CSoundCapture_demoDlg, CDialog)
	//{{AFX_MSG_MAP(CSoundCapture_demoDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_RECORD, OnButtonRecord)
	ON_BN_CLICKED(IDC_BUTTON_STOP, OnButtonStop)
	ON_BN_CLICKED(IDC_BUTTON_FILEPATH, OnButtonFilepath)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSoundCapture_demoDlg message handlers
void CSoundCapture_demoDlg::InitCombox()
{	
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("11025 Hz")), 11025);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("16000 Hz")), 16000);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("22050 Hz")), 22050);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("32000 Hz")), 32000);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("44100 Hz")), 44100);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("8000  Hz")), 8000);
	m_cSamplePerSecondBox.SetItemData(m_cSamplePerSecondBox.AddString(_T("96000 Hz")), 96000);
	m_cSamplePerSecondBox.SetCurSel(5);


	m_cBitPerSampleBox.SetItemData(m_cBitPerSampleBox.AddString(_T("16 bit")), 16);
	m_cBitPerSampleBox.SetItemData(m_cBitPerSampleBox.AddString(_T("8 bit")), 8);
	m_cBitPerSampleBox.SetCurSel(0);
}

BOOL CSoundCapture_demoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here

	LANG_SETWNDSTATICTEXT(this);
	InitCombox();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CSoundCapture_demoDlg::OnPaint() 
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
HCURSOR CSoundCapture_demoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void WINAPI AudioRecord(LPBYTE pDataBuffer, DWORD DataLength, void* pUserData)
{
	FILE *fPCM = (FILE *)pUserData;
	if(fPCM != NULL)
		fwrite(pDataBuffer, 1, DataLength, fPCM);
}

void CSoundCapture_demoDlg::OnButtonRecord() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);
	m_fPCM = _tfopen(m_csFileName.GetBuffer(0), _T("wb+"));
	if( m_fPCM == NULL)
	{
		AfxMessageBox(LANG_CS("File path error!"));
		return;
	}
	
	m_nBitPerSample = m_cBitPerSampleBox.GetItemData(m_cBitPerSampleBox.GetCurSel());
	m_nSamplePerSecond = m_cSamplePerSecondBox.GetItemData(m_cSamplePerSecondBox.GetCurSel());
	
	long lSampleLen = 40*m_nBitPerSample*m_nSamplePerSecond/16000;
	if(lSampleLen <= 320)
		lSampleLen = 320;
	//else if (lSampleLen >= 1024)
	//	lSampleLen = 1024;

	PLAY_OpenAudioRecord(AudioRecord, m_nBitPerSample, m_nSamplePerSecond, lSampleLen, NULL, m_fPCM);
	GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(TRUE);
	GetDlgItem(IDC_BUTTON_RECORD)->EnableWindow(FALSE);
}

void CSoundCapture_demoDlg::OnButtonStop() 
{
	// TODO: Add your control notification handler code here
	PLAY_CloseAudioRecord();
	if(m_fPCM != NULL)
		fclose(m_fPCM);
	GetDlgItem(IDC_BUTTON_RECORD)->EnableWindow(TRUE);
	GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
}

void CSoundCapture_demoDlg::OnButtonFilepath() 
{
	// TODO: Add your control notification handler code here
	CFileDialog FileChooser(TRUE, NULL, NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, _T("All files(*.*)|*.*||"));

	if(FileChooser.DoModal() == IDOK)
	{
		m_csFileName = FileChooser.GetPathName();
		GetDlgItem(IDC_EDIT_FILEPATH)->SetWindowText(m_csFileName);
	}
}

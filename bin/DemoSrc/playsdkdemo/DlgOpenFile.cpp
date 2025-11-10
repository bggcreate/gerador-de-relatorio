// DlgOpenFile.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DlgOpenFile.h"
#include "LanguageConvertor.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgOpenFile dialog


CDlgOpenFile::CDlgOpenFile(CWnd* pParent /*=NULL*/)
	: CDialog(CDlgOpenFile::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgOpenFile)
	m_nType = 0;
	m_strFile = _T("");
	//}}AFX_DATA_INIT
}


void CDlgOpenFile::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgOpenFile)
	DDX_Text(pDX, IDC_EDIT_FILE, m_strFile);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgOpenFile, CDialog)
	//{{AFX_MSG_MAP(CDlgOpenFile)
	ON_BN_CLICKED(IDC_BUTTON_FILE, OnButtonFile)
	ON_BN_CLICKED(IDC_RADIO_FILE, OnRadioFile)
	ON_BN_CLICKED(IDC_RADIO_FILESTREAM, OnRadioFilestream)
	ON_BN_CLICKED(IDC_BUTTON_OK, OnButtonOk)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgOpenFile message handlers

void CDlgOpenFile::OnButtonFile() 
{
	// TODO: Add your control notification handler code here
	// TODO: Add your control notification handler code here
	CFileDialog dlgFile(TRUE, 
		NULL,
		NULL, 
		OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
		_T("All files(*.*)|*.*|Dav files (*.dav)|*.dav|"));
	//choose file
	if (dlgFile.DoModal()==IDOK)
	{
		m_strFile = dlgFile.GetPathName();
		UpdateData(FALSE);

		((CButton*)GetDlgItem(IDC_RADIO_FILE))->EnableWindow(TRUE);
		((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->EnableWindow(TRUE);

		int pos = -1;
		TCHAR* pbuffer = m_strFile.GetBuffer(m_strFile.GetLength());
		if ( -1 != (pos = m_strFile.ReverseFind('.')))
		{
			pbuffer += pos;
		}
		
		//asf,MP4 only support file mode
		if (0 == _tcscmp(pbuffer, _T(".asf")) || 0 == _tcscmp(pbuffer, _T(".mp4")))
		{
			m_nType = 0;
			((CButton*)GetDlgItem(IDC_RADIO_FILE))->SetCheck(TRUE);
			((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->SetCheck(FALSE);
			((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->EnableWindow(FALSE);
		}
	}
}

void CDlgOpenFile::OnRadioFile() 
{
	// TODO: Add your control notification handler code here
	m_nType = 0;
}

void CDlgOpenFile::OnRadioFilestream() 
{
	// TODO: Add your control notification handler code here
	m_nType = 1;
}

BOOL CDlgOpenFile::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	((CButton*)GetDlgItem(IDC_RADIO_FILE))->EnableWindow(TRUE);
	((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->EnableWindow(TRUE);
	
	((CButton*)GetDlgItem(IDC_RADIO_FILE))->SetCheck(!m_nType);
	((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->SetCheck(m_nType);

	if (!m_strFile.IsEmpty())
	{
		//ps,ts only support stream mode
		int pos = -1;
		TCHAR* pbuffer = m_strFile.GetBuffer(m_strFile.GetLength());
		if ( -1 != (pos = m_strFile.ReverseFind('.')))
		{
			pbuffer += pos;
		}
		
		if (0 == _tcscmp(pbuffer, _T(".asf")) || 0 == _tcscmp(pbuffer, _T(".mp4")))
		{
			m_nType = 0;
			((CButton*)GetDlgItem(IDC_RADIO_FILE))->SetCheck(TRUE);
			((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->SetCheck(FALSE);
			((CButton*)GetDlgItem(IDC_RADIO_FILESTREAM))->EnableWindow(FALSE);
		}
	}

	LANG_SETWNDSTATICTEXT(this);
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgOpenFile::OnButtonOk() 
{
	// TODO: Add your control notification handler code here
	CDialog::OnOK();
}

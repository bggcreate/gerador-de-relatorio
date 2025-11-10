// DlgPicSetting.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DlgPicSetting.h"
#include "LanguageConvertor.h"
#include <shlwapi.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgPicSetting dialog

#pragma comment(lib, "shlwapi.lib")

CDlgPicSetting::CDlgPicSetting(CWnd* pParent /*=NULL*/)
	: CDialog(CDlgPicSetting::IDD, pParent)
{
	/* get the whole process name */
	m_strPath.Format(_T("%s%s"), GetMoudlePath(), _T("picture"));
	if(!PathFileExists(m_strPath))
		CreateDirectory(m_strPath, NULL);
	m_nType = 0;
}


void CDlgPicSetting::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgPicSetting)
	DDX_Text(pDX, IDC_EDIT_PICPATH, m_strPath);
	if(!PathFileExists(m_strPath))
		CreateDirectory(m_strPath, NULL);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgPicSetting, CDialog)
	//{{AFX_MSG_MAP(CDlgPicSetting)
	ON_BN_CLICKED(IDC_BUTTON_PATH, OnButtonPath)
	ON_BN_CLICKED(IDC_RADIO_BMP, OnRadioBmp)
	ON_BN_CLICKED(IDC_RADIO_JPG, OnRadioJpg)
	ON_WM_CLOSE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgPicSetting message handlers

void CDlgPicSetting::OnOK() 
{
	// TODO: Add extra validation here
	CDialog::OnOK();
}


CString OpenFolder() 
{
	CString strPath;
	LPMALLOC   pMalloc;  
	if(SHGetMalloc(&pMalloc) == NOERROR)   
	{   
		BROWSEINFO bi;   
		TCHAR pszBuffer[MAX_PATH];   
		LPITEMIDLIST pidl;   
		bi.hwndOwner = NULL;   
		bi.pidlRoot = NULL;   
		bi.pszDisplayName = pszBuffer;   
		bi.lpszTitle = _T("");  
		bi.ulFlags = BIF_RETURNFSANCESTORS   |   BIF_RETURNONLYFSDIRS;   
		bi.lpfn = NULL;   
		bi.lParam = 0;   
		bi.iImage = 0;   
		if((pidl=SHBrowseForFolder(&bi)) != NULL)   
		{   
			if(SHGetPathFromIDList(pidl, pszBuffer))   
			{
				strPath = pszBuffer;  
			}
			pMalloc->Free(pidl);   
		}   
		pMalloc->Release();   
	} 
	
	return strPath;
}


void CDlgPicSetting::OnButtonPath() 
{
	// TODO: Add your control notification handler code here
	CString strPath = OpenFolder();
	if(PathFileExists(strPath))
		m_strPath = strPath;
	
	UpdateData(FALSE);
}

void CDlgPicSetting::OnRadioBmp() 
{
	// TODO: Add your control notification handler code here
	m_nType = 0;
}

void CDlgPicSetting::OnRadioJpg() 
{
	// TODO: Add your control notification handler code here
	m_nType = 1;
}

BOOL CDlgPicSetting::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	((CButton*)GetDlgItem(IDC_RADIO_BMP))->SetCheck(!m_nType);
	((CButton*)GetDlgItem(IDC_RADIO_JPG))->SetCheck(m_nType);

	UpdateData(FALSE);

	LANG_SETWNDSTATICTEXT(this);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgPicSetting::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	CDialog::OnClose();
}

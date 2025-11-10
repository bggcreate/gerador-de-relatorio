// LocateDlg.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "LocateDlg.h"
#include "Player.h"
#include "LanguageConvertor.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CLocateDlg dialog


CLocateDlg::CLocateDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CLocateDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CLocateDlg)
	m_locateType = TYPEBYFRAME;
	m_locatevalue = 0;
	//}}AFX_DATA_INIT
}


void CLocateDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CLocateDlg)
	DDX_Text(pDX, IDC_EDIT_RANGEVALUE, m_locatevalue);
	DDX_Radio(pDX, IDC_RADIO_FRAME, m_locateType);
	//}}AFX_DATA_MAP
	DDV_MinMaxInt(pDX, m_locatevalue, 0, INT_MAX);
}


BEGIN_MESSAGE_MAP(CLocateDlg, CDialog)
	//{{AFX_MSG_MAP(CLocateDlg)
	ON_BN_CLICKED(IDC_BUTTON_LOCATE, OnButtonLocate)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLocateDlg message handlers

void CLocateDlg::OnButtonLocate() 
{
	// TODO: Add your control notification handler code here
	if (UpdateData(TRUE) == 0)
		return ;

	m_locateType = ((CButton*)GetDlgItem(IDC_RADIO_FRAME))->GetCheck()?TYPEBYFRAME:TYPEBYTIME;
	if (m_locateType == TYPEBYFRAME)
	{
		if ( m_locatevalue < 0 || m_locatevalue > CPlayer::Instance()->GetTotalFrame() - 1)
		{
			AfxMessageBox(LANG_CS("Input number error!")) ;
			return ;
		}
		CPlayer::Instance()->SetCurrentFrameNum(m_locatevalue);
	}
	else 
	{
		if (m_locatevalue < 0 || m_locatevalue > CPlayer::Instance()->GetTotalTime())
		{
			AfxMessageBox(LANG_CS("Input number error!")) ;
			return ;
		}
		CPlayer::Instance()->SetPlayedTiemEx(m_locatevalue*1000);
	}
}

BOOL CLocateDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	LANG_SETWNDSTATICTEXT(this);
	// TODO: Add extra initialization here
	CString str ;
	str.Format(_T("%s         0--%d\n%s(Sec)     0--%d"),
		LANG_CS("Frame Range"), CPlayer::Instance()->GetTotalFrame() - 1,
		LANG_CS("Time Range"), CPlayer::Instance()->GetTotalTime());
	m_locatevalue = 0;
	GetDlgItem(IDC_STATIC_RANGE)->SetWindowText(str);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CLocateDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if (pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_RETURN )
	{
		OnButtonLocate();
		return 1;
	}
	else if(pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_ESCAPE)
		return 1;
	return CDialog::PreTranslateMessage(pMsg);
}

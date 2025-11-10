// DlgSetColor.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DlgSetColor.h"
#include "Player.h"
#include "LanguageConvertor.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgSetColor dialog


CDlgSetColor::CDlgSetColor(CWnd* pParent /*=NULL*/)
	: CDialog(CDlgSetColor::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgSetColor)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDlgSetColor::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgSetColor)
	DDX_Control(pDX, IDC_SLIDER_SATURATION, m_procSaturation);
	DDX_Control(pDX, IDC_SLIDER_HUE, m_procHue);
	DDX_Control(pDX, IDC_SLIDER_CONTRAST, m_procContrast);
	DDX_Control(pDX, IDC_SLIDER_BRIGHTNESS, m_procBrightness);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgSetColor, CDialog)
	//{{AFX_MSG_MAP(CDlgSetColor)
	ON_WM_HSCROLL()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgSetColor message handlers

BOOL CDlgSetColor::OnInitDialog() 
{
	CDialog::OnInitDialog();
	LANG_SETWNDSTATICTEXT(this);
	// TODO: Add extra initialization here
	m_procSaturation.SetRange(0, 128);
	m_procHue.SetRange(0, 128);
	m_procContrast.SetRange(0, 128);
	m_procBrightness.SetRange(0, 128);

	int nBrightness = 0;
	int nContrast = 0;
	int nSaturation = 0;
	int nHue = 0;
	CPlayer::Instance()->GetColor(&nSaturation, &nBrightness, &nContrast, &nHue);
			
	m_procSaturation.SetPos(nSaturation);
	m_procHue.SetPos(nHue);
	m_procContrast.SetPos(nContrast);
	m_procBrightness.SetPos(nBrightness);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgSetColor::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	// TODO: Add your message handler code here and/or call default
	switch(GetWindowLong(pScrollBar->m_hWnd, GWL_ID))
	{
	case IDC_SLIDER_SATURATION:
	case IDC_SLIDER_CONTRAST:
	case IDC_SLIDER_BRIGHTNESS:
	case IDC_SLIDER_HUE:
		{
			int nBrightness = m_procBrightness.GetPos();
			int nContrast = m_procContrast.GetPos();
			int nSaturation = m_procSaturation.GetPos();
			int nHue = m_procHue.GetPos();	
			
			CPlayer::Instance()->SetColor(nSaturation, nBrightness, nContrast, nHue);
		}
		break ;
	}
	
	CDialog::OnHScroll(nSBCode, nPos, pScrollBar);	
}

BOOL CDlgSetColor::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if (pMsg->wParam == VK_RETURN && pMsg->message == WM_KEYDOWN)
		return 1;
	else if(pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_ESCAPE)
		return 1;
	return CDialog::PreTranslateMessage(pMsg);
}

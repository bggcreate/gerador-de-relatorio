// DlgMultPlay.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DlgMultPlay.h"
#include "Player.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgMultPlay dialog


CDlgMultPlay::CDlgMultPlay(CWnd* pParent /*=NULL*/)
	: CDialog(CDlgMultPlay::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgMultPlay)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDlgMultPlay::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgMultPlay)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgMultPlay, CDialog)
	//{{AFX_MSG_MAP(CDlgMultPlay)
	ON_WM_CLOSE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgMultPlay message handlers

void CDlgMultPlay::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	CPlayer::Instance()->SetDisplayRegion(1, NULL, m_hWnd, FALSE);
	CDialog::OnClose();
}

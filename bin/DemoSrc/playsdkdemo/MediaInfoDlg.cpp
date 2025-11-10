// MediaInfoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "MediaInfoDlg.h"
#include "dhplay.h"
#include "LanguageConvertor.h"
#include "Player.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMediaInfoDlg dialog


CMediaInfoDlg::CMediaInfoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMediaInfoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMediaInfoDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CMediaInfoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMediaInfoDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CMediaInfoDlg, CDialog)
	//{{AFX_MSG_MAP(CMediaInfoDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMediaInfoDlg message handlers

BOOL CMediaInfoDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	int len = 0;
	MEDIA_INFO tMediaInfo;
	memset(&tMediaInfo, 0, sizeof(MEDIA_INFO));
	CPlayer::Instance()->GetQueryInfo(PLAY_CMD_GetMediaInfo, (char *)&tMediaInfo, sizeof(MEDIA_INFO), &len);

	CString csMediaInfo = "";

	if (tMediaInfo.lFrameRate != 0)
	{
		CString csTemp;
		csTemp.Format(_T("%s\t%d\n"),LANG_CS("Video Frame Rate"), tMediaInfo.lFrameRate);
		csMediaInfo += csTemp;

		csTemp.Format(_T("%s\t%d * %d\n"),LANG_CS("Video resolution"), tMediaInfo.lWidth, tMediaInfo.lHeight);
		csMediaInfo += csTemp;
		csMediaInfo += "---------------------------------------------\n";
	}

	if (tMediaInfo.lChannel != 0)
	{
		CString csTemp;
		csTemp.Format(_T("%s\t%d\n"),LANG_CS("Audio Channel"), tMediaInfo.lChannel);
		csMediaInfo += csTemp;

		csTemp.Format(_T("%s\t%d\n"),LANG_CS("Audio BitPerSample"), tMediaInfo.lBitPerSample);
		csMediaInfo += csTemp;

		csTemp.Format(_T("%s\t%d\n"),LANG_CS("Audio SamplesPerSec"), tMediaInfo.lSamplesPerSec);
		csMediaInfo += csTemp;
		csMediaInfo += "---------------------------------------------\n";
	}

	GetDlgItem(IDC_STATIC_MEDIAINFO)->SetWindowText(csMediaInfo);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CMediaInfoDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if (pMsg->wParam == VK_RETURN && pMsg->message == WM_KEYDOWN)
		return 1;
	else if(pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_ESCAPE)
		return 1;
	return CDialog::PreTranslateMessage(pMsg);
}

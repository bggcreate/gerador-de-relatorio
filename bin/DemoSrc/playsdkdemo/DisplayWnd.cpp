// DisplayWnd.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DisplayWnd.h"
#include "Player.h"
#include "LanguageConvertor.h"
#include "dhplayEx.h"
#include <math.h>
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDisplayWnd dialog


CDisplayWnd::CDisplayWnd(CWnd* pParent /*=NULL*/)
	: CDialog(CDisplayWnd::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDisplayWnd)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_bLButtonDown = FALSE;

	m_bFirstMove = TRUE;
}


void CDisplayWnd::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDisplayWnd)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDisplayWnd, CDialog)
	//{{AFX_MSG_MAP(CDisplayWnd)
	ON_WM_MOUSEMOVE()
	ON_WM_TIMER()
	ON_WM_RBUTTONDOWN()
	ON_WM_RBUTTONUP()
	ON_MESSAGE(WM_USER_MSG_ENCTYPECHANGED, OnEncTypeChange)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDisplayWnd message handlers
BOOL CDisplayWnd::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	RECT rectParent;
	GetParent()->GetWindowRect(&rectParent);
	RECT rect = {0, 0, rectParent.right-rectParent.left, rectParent.bottom-rectParent.top};
	MoveWindow(&rect, TRUE);

	m_dlgMultPlay.Create(IDD_DIALOG_MULTPLAY);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDisplayWnd::CloseMultiWnd()
{
	m_dlgMultPlay.ShowWindow(SW_HIDE);
}

void CALLBACK DrawCBFun(LONG nPort, HDC hDc, void* nUser)
{
	CDisplayWnd* pDisplayWnd = (CDisplayWnd*)nUser;

	pDisplayWnd->OnDrawCBFun(nPort, hDc);
}

void CDisplayWnd::OnDrawCBFun(LONG nPort, HDC hDc)
{
	CRect rect(m_pointStart, m_pointMove);
	if(IsRectEmpty(rect)) 
		return;
	
	CDC dc;
	if (!dc.Attach(hDc))
	{
		return;
	}

	dc.Draw3dRect(rect,RGB(255,0,0),RGB(255,0,0));
	dc.Detach();
}

void CDisplayWnd::OnRButtonDown(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	m_pointStart = point;
	CRect rect;
	GetWindowRect(rect);
	ClipCursor(rect);
	//SetTimer(1, 100, NULL);
	CPlayer::Instance()->RegistDrawCallback(DrawCBFun, (void*)this);

	CDialog::OnLButtonDown(nFlags, point);
}

CRect CalcDisplayRect(CRect& winRect, CRect& displayRect, int nPicWidht, int nPicHeight)
{
	CRect retRect;
	retRect.left = displayRect.left*nPicWidht/winRect.Width();
	retRect.right = displayRect.right*nPicWidht/winRect.Width();
	retRect.top = displayRect.top*nPicHeight/winRect.Height();
	retRect.bottom = displayRect.bottom*nPicHeight/winRect.Height();

	return retRect;
}

void CDisplayWnd::OnRButtonUp(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	m_pointEnd = point;
	KillTimer(1);
	ClipCursor(NULL);

	/*coordinate convert*/
	CRect displayRect(m_pointStart, m_pointEnd);
	if(IsRectEmpty(displayRect))
		return;
	CRect winRect;
	GetWindowRect(winRect);
	int nWidth=0,nHeight=0;
	CPlayer::Instance()->GetPicture(&nWidth, &nHeight);
	CRect destRect = CalcDisplayRect(winRect, displayRect, nWidth, nHeight);
	if(IsRectEmpty(destRect))
		return;

	/*multiwindow display*/
	CPlayer::Instance()->SetDisplayRegion(1, NULL, m_dlgMultPlay.m_hWnd, FALSE);
	CPlayer::Instance()->SetDisplayRegion(1, destRect, m_dlgMultPlay.m_hWnd, TRUE);
	m_dlgMultPlay.ShowWindow(SW_NORMAL);

	/*change heading info*/
	CString strTitle;
	strTitle.Format(_T("%s(%d %d, %d %d)"), LANG_CS("Multi-play"), destRect.left, destRect.top, destRect.right, destRect.bottom);
	m_dlgMultPlay.SetWindowText(strTitle);

	CPlayer::Instance()->RegistDrawCallback(NULL, 0);

	CDialog::OnLButtonUp(nFlags, point);
}

void CDisplayWnd::OnMouseMove(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	if (m_bFirstMove)
	{
		m_pointMove = point;
		m_pointMoveSumX = 0;
		m_pointMoveSumY = 0;
		m_bFirstMove = FALSE;
	}
	float sensitivity = 0.05f;
	float xoffset = (point.x - m_pointMove.x) * sensitivity;
	float yoffset = (point.y - m_pointMove.y) * sensitivity;
	if (m_bLButtonDown)
	{
		//m_pointMoveSumX -= point.x - m_pointMove.x;
		//m_pointMoveSumY -= point.y - m_pointMove.y;
		PLAY_GetDoubleRegion(0, 0, RENDER_STEREO_ROTATE_X, &m_dRotateSumX);
		PLAY_GetDoubleRegion(0, 0, RENDER_STEREO_ROTATE_Y, &m_dRotateSumY);

		double dXOffset = (double)(point.x - m_pointMove.x) * sensitivity;
		double dYOffset = (double)(point.y - m_pointMove.y) * sensitivity;
		if (fabs(dXOffset) >= fabs(dYOffset))
		{
			m_dRotateSumY += (double)(point.x - m_pointMove.x) * sensitivity;
		}
		else
		{
			m_dRotateSumX += (double)(point.y - m_pointMove.y) * sensitivity;
		}


		PLAY_SetStereoRotate(0, 0, m_dRotateSumX, m_dRotateSumY, 0);
	}

	CPlayer::Instance()->UpdateDragFE(point);
	m_pointMove = point;
	CDialog::OnMouseMove(nFlags, point);
}

void CDisplayWnd::OnTimer(UINT_PTR nIDEvent) 
{
	// TODO: Add your message handler code here and/or call default
	CRect rect(m_pointStart, m_pointMove);
	if(IsRectEmpty(rect)) 
		return;

	/*easy draw*/
	CDC *pdc = GetDC();	
	pdc->Draw3dRect(rect,RGB(255,0,0),RGB(255,0,0));
	ReleaseDC(pdc);
	CDialog::OnTimer(nIDEvent);
}


BOOL CDisplayWnd::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if (pMsg->wParam == VK_RETURN)
	{
		return 1;
	}
	return CDialog::PreTranslateMessage(pMsg);
}

LRESULT CDisplayWnd::OnEncTypeChange(WPARAM wParam, LPARAM lParam)
{
    //此段逻辑去掉，会触发异常。
	///*recalculate destRect when resolution is changed*/
	//CRect displayRect(m_pointStart, m_pointEnd);
	//if(IsRectEmpty(displayRect))
	//{
	//	return 0;
	//}

	//CRect winRect;
	//GetWindowRect(winRect);
	//int nWidth=0,nHeight=0;
	//CPlayer::Instance()->GetPicture(&nWidth, &nHeight);
	//CRect destRect = CalcDisplayRect(winRect, displayRect, nWidth, nHeight);
	//if(IsRectEmpty(destRect))
	//{
	//	return 0;
	//}

	///*refresh destRect when resolution is changed*/
	//CPlayer::Instance()->SetDisplayRegion(1, destRect, m_dlgMultPlay.m_hWnd, TRUE);

	return 1;
}

void CDisplayWnd::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	m_bLButtonDown = TRUE;

	CPlayer::Instance()->StartDragFE(point);

	CDialog::OnLButtonDown(nFlags, point);
}

void CDisplayWnd::OnLButtonUp(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	m_bLButtonDown = FALSE;

	CPlayer::Instance()->StopDragFE();

	CDialog::OnLButtonUp(nFlags, point);
}

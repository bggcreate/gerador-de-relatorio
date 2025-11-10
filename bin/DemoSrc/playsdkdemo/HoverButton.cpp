// ColorButton.cpp : implementation file
//
// Copyright (c) 2009.
//

#include "stdafx.h"
#include "HoverButton.h"

#include <afxtempl.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

BEGIN_MESSAGE_MAP(CHoverButton, CBitmapButton)
	//{{AFX_MSG_MAP(CHoverButton)
	ON_WM_MOUSEMOVE()
	ON_MESSAGE(WM_MOUSELEAVE,OnMouseLeave)
	ON_MESSAGE(WM_MOUSEHOVER,OnMouseHover)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////////////////////////////
CHoverButton::CHoverButton()
{
    m_bHover = FALSE;       
    m_bTracking = FALSE;
	m_bButtonEnable = TRUE;
}

CHoverButton::~CHoverButton()
{

}

BOOL CHoverButton::LoadBitmap(UINT bitmapid)
{
    //load picture
    mybitmap.Attach(::LoadImage(::AfxGetInstanceHandle(),MAKEINTRESOURCE(bitmapid), IMAGE_BITMAP,0,0,LR_LOADMAP3DCOLORS));
    BITMAP    bitmapbits;
    //receive bitmap info and save it to bitmapbits struct
    mybitmap.GetBitmap(&bitmapbits);
    
    //receive the height and quarter of the width of the bitmap.
    m_ButtonSize.cy=bitmapbits.bmHeight;
    m_ButtonSize.cx=bitmapbits.bmWidth/4;
    
    SetWindowPos(NULL, 0,0, m_ButtonSize.cx,m_ButtonSize.cy,SWP_NOMOVE |SWP_NOOWNERZORDER);
    return TRUE;
}

void CHoverButton::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    //receive absolutely necessarily context saved in the DRAWITEMSTRUCT struct when drawing the button.
    CDC* mydc=CDC::FromHandle(lpDrawItemStruct->hDC);
	
    //create compatible context 
    CDC* pMemDC = new CDC;
    pMemDC->CreateCompatibleDC(mydc);
	
    //save the old object.
    CBitmap * pOldBitmap;
    pOldBitmap = pMemDC->SelectObject(&mybitmap);
    
    CPoint point(0,0);    
    
    //verdict whether the button is selected, and draw the right bitmap of the button,the second bitmap provided represent selected.
    if(lpDrawItemStruct->itemState & ODS_SELECTED)
    {
        mydc->BitBlt(0,0,m_ButtonSize.cx,m_ButtonSize.cy,pMemDC,m_ButtonSize.cx*2,0,SRCCOPY);
    }
    else
    {   //verdict whether the mouse is on or off the button,draw the right bitmap.
        if(m_bHover)
        {
            mydc->BitBlt(0,0,m_ButtonSize.cx,m_ButtonSize.cy,pMemDC,m_ButtonSize.cx,0,SRCCOPY);
        }else
        {
            mydc->BitBlt(0,0,m_ButtonSize.cx,m_ButtonSize.cy,pMemDC,0,0,SRCCOPY);
        }    
    }

	if (!m_bButtonEnable)
		mydc->BitBlt(0,0,m_ButtonSize.cx,m_ButtonSize.cy,pMemDC,m_ButtonSize.cx*3,0,SRCCOPY);
	
    // clean up
    pMemDC->SelectObject(pOldBitmap);
    delete pMemDC;
}

void CHoverButton::OnMouseMove(UINT nFlags, CPoint point)
{
    if (!m_bTracking)
    {
        TRACKMOUSEEVENT tme;
        tme.cbSize = sizeof(tme);
        tme.hwndTrack = m_hWnd;
        tme.dwFlags = TME_LEAVE|TME_HOVER;
        tme.dwHoverTime = 1;
        m_bTracking = _TrackMouseEvent(&tme);
    }
    CBitmapButton::OnMouseMove(nFlags, point);
}

LRESULT CHoverButton::OnMouseLeave(WPARAM wparam, LPARAM lparam)
{
    m_bTracking = FALSE;
    m_bHover=FALSE;
    //redraw the button
    Invalidate(TRUE);
    return 0;
}

LRESULT CHoverButton::OnMouseHover(WPARAM wparam, LPARAM lparam) 
{	
    m_bHover=TRUE;
    Invalidate(TRUE);
    return 0;
}

BOOL CHoverButton::EnableWindow(BOOL bEnable)
{
	m_bButtonEnable = bEnable;
	Invalidate(TRUE);
    return TRUE;
}

BOOL CHoverButton::EnableWindowEx(BOOL bEnable)
{
	m_bButtonEnable = bEnable;
	Invalidate(TRUE);
    return CBitmapButton::EnableWindow(bEnable);
}
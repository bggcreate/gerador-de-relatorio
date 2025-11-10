#if !defined(AFX_DISPLAYWND_H__FF1C5B3B_4B6D_45CA_BBDD_04C5AA786D31__INCLUDED_)
#define AFX_DISPLAYWND_H__FF1C5B3B_4B6D_45CA_BBDD_04C5AA786D31__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DisplayWnd.h : header file
//

#include "DlgMultPlay.h"

/////////////////////////////////////////////////////////////////////////////
// CDisplayWnd dialog
class CDisplayWnd : public CDialog
{
// Construction
public:
	CDisplayWnd(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDisplayWnd)
	enum { IDD = IDD_DIALOG_DISPLAY };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDisplayWnd)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
public:
	void CloseMultiWnd();
	void OnDrawCBFun(LONG nPort, HDC hDc);

protected:

	// Generated message map functions
	//{{AFX_MSG(CDisplayWnd)
	virtual BOOL OnInitDialog();
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg LRESULT OnEncTypeChange(WPARAM wParam, LPARAM lParam);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	/*Variables for multiarea display*/
	CPoint m_pointStart;
	CPoint m_pointMove;
	CPoint m_pointEnd;
	CDlgMultPlay m_dlgMultPlay;
	BOOL m_bFirstMove;
	BOOL m_bLButtonDown;

	int m_pointMoveSumX;
	int m_pointMoveSumY;
	double m_dRotateSumX;
	double m_dRotateSumY;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DISPLAYWND_H__FF1C5B3B_4B6D_45CA_BBDD_04C5AA786D31__INCLUDED_)

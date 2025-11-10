#if !defined(AFX_DLGSETCOLOR_H__5009847F_CBCC_4668_9D46_621470A20FA0__INCLUDED_)
#define AFX_DLGSETCOLOR_H__5009847F_CBCC_4668_9D46_621470A20FA0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgSetColor.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDlgSetColor dialog

class CDlgSetColor : public CDialog
{
// Construction
public:
	CDlgSetColor(CWnd* pParent = NULL);   // standard constructor
	
// Dialog Data
	//{{AFX_DATA(CDlgSetColor)
	enum { IDD = IDD_DIALOG_SETCOLOR };
	CSliderCtrl	m_procSaturation;
	CSliderCtrl	m_procHue;
	CSliderCtrl	m_procContrast;
	CSliderCtrl	m_procBrightness;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgSetColor)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDlgSetColor)
	virtual BOOL OnInitDialog();
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGSETCOLOR_H__5009847F_CBCC_4668_9D46_621470A20FA0__INCLUDED_)

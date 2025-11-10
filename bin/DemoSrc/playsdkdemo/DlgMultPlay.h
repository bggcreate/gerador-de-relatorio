#if !defined(AFX_DLGMULTPLAY_H__FDC83973_11CE_49F7_BA32_BB672BFDD223__INCLUDED_)
#define AFX_DLGMULTPLAY_H__FDC83973_11CE_49F7_BA32_BB672BFDD223__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgMultPlay.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDlgMultPlay dialog

class CDlgMultPlay : public CDialog
{
// Construction
public:
	CDlgMultPlay(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgMultPlay)
	enum { IDD = IDD_DIALOG_MULTPLAY };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgMultPlay)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDlgMultPlay)
	afx_msg void OnClose();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGMULTPLAY_H__FDC83973_11CE_49F7_BA32_BB672BFDD223__INCLUDED_)

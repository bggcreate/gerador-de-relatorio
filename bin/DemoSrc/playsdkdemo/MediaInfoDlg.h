#if !defined(AFX_MEDIAINFODLG_H__AE60FEAC_5F58_4F42_821E_AE344B048AAF__INCLUDED_)
#define AFX_MEDIAINFODLG_H__AE60FEAC_5F58_4F42_821E_AE344B048AAF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MediaInfoDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMediaInfoDlg dialog

class CMediaInfoDlg : public CDialog
{
// Construction
public:
	CMediaInfoDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CMediaInfoDlg)
	enum { IDD = IDD_DIALOG_MEDIAINFO };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMediaInfoDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CMediaInfoDlg)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MEDIAINFODLG_H__AE60FEAC_5F58_4F42_821E_AE344B048AAF__INCLUDED_)

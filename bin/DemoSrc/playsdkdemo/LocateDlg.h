#if !defined(AFX_LOCATEDLG_H__A8E2215F_4B91_4EDF_8C6D_300BFF6AF5C7__INCLUDED_)
#define AFX_LOCATEDLG_H__A8E2215F_4B91_4EDF_8C6D_300BFF6AF5C7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// LocateDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CLocateDlg dialog

class CLocateDlg : public CDialog
{
// Construction
public:
	CLocateDlg(CWnd* pParent = NULL);   // standard constructor
	typedef enum{
		TYPEBYFRAME, TYPEBYTIME
	}LOCATETYPE;
// Dialog Data
	//{{AFX_DATA(CLocateDlg)
	enum { IDD = IDD_DIALOG_LOCATE };
	int m_locatevalue;
	int	m_locateType;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CLocateDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CLocateDlg)
	afx_msg void OnButtonLocate();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LOCATEDLG_H__A8E2215F_4B91_4EDF_8C6D_300BFF6AF5C7__INCLUDED_)

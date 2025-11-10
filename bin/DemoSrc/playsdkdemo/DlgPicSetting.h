#if !defined(AFX_DLGPICSETTING_H__2B0B336A_C8D5_4970_8F51_ED49869262F2__INCLUDED_)
#define AFX_DLGPICSETTING_H__2B0B336A_C8D5_4970_8F51_ED49869262F2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgPicSetting.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDlgPicSetting dialog

class CDlgPicSetting : public CDialog
{
// Construction
public:
	CDlgPicSetting(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgPicSetting)
	enum { IDD = IDD_DIALOG_PICTURE };
	CString	m_strPath;
	int m_nType;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgPicSetting)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation

protected:

	// Generated message map functions
	//{{AFX_MSG(CDlgPicSetting)
	virtual void OnOK();
	afx_msg void OnButtonPath();
	afx_msg void OnRadioBmp();
	afx_msg void OnRadioJpg();
	virtual BOOL OnInitDialog();
	afx_msg void OnClose();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGPICSETTING_H__2B0B336A_C8D5_4970_8F51_ED49869262F2__INCLUDED_)

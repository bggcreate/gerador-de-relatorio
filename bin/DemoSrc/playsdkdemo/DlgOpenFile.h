#if !defined(AFX_DLGOPENFILE_H__2E83E05F_17E4_4C8D_B3D4_95A861ABE3DA__INCLUDED_)
#define AFX_DLGOPENFILE_H__2E83E05F_17E4_4C8D_B3D4_95A861ABE3DA__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgOpenFile.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDlgOpenFile dialog

class CDlgOpenFile : public CDialog
{
// Construction
public:
	CDlgOpenFile(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgOpenFile)
	enum { IDD = IDD_OPENFILE };
	int		m_nType;
	CString	m_strFile;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgOpenFile)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDlgOpenFile)
	afx_msg void OnButtonFile();
	afx_msg void OnRadioFile();
	afx_msg void OnRadioFilestream();
	virtual BOOL OnInitDialog();
	afx_msg void OnButtonOk();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGOPENFILE_H__2E83E05F_17E4_4C8D_B3D4_95A861ABE3DA__INCLUDED_)

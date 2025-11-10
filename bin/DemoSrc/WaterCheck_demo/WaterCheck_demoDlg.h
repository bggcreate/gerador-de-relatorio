// WaterCheck_demoDlg.h : header file
//

#if !defined(AFX_WATERCHECK_DEMODLG_H__97A6C342_3785_4FE7_B76A_EFE67B965871__INCLUDED_)
#define AFX_WATERCHECK_DEMODLG_H__97A6C342_3785_4FE7_B76A_EFE67B965871__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CWaterCheck_demoDlg dialog

class CWaterCheck_demoDlg : public CDialog
{
// Construction
public:
	CWaterCheck_demoDlg(CWnd* pParent = NULL);	// standard constructor
	~CWaterCheck_demoDlg();
	typedef enum
	{
		INIT,
		OPENFILE,
		CHECK,
		STOP,
		COMPLETE
	};
public:
	void watermarkCheck();
	void ChangeUIstate(int nState);

	CFile m_checkFile;

// Dialog Data
	//{{AFX_DATA(CWaterCheck_demoDlg)
	enum { IDD = IDD_WATERCHECK_DEMO_DIALOG };
	CListCtrl	m_lscheckInfoList;
	CString	m_csfilePath;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CWaterCheck_demoDlg)
	protected:
	BOOL PreTranslateMessage(MSG* pMsg) ;
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CWaterCheck_demoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonCheck();
	afx_msg void OnButtonFilepath();
	afx_msg void OnClose();
	afx_msg void OnButtonStop();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	HANDLE m_hThread;
	HANDLE m_hExit;

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_WATERCHECK_DEMODLG_H__97A6C342_3785_4FE7_B76A_EFE67B965871__INCLUDED_)

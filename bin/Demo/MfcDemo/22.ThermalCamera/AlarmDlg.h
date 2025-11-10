#if !defined(AFX_ALARMDLG_H__CF21D93E_FD45_4D00_9D11_A984CFEE0937__INCLUDED_)
#define AFX_ALARMDLG_H__CF21D93E_FD45_4D00_9D11_A984CFEE0937__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// AlarmDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CAlarmDlg dialog
#define WM_ALARM_INFO	(WM_USER+199)

#define MAX_MSG_SHOW	(10000)

class CAlarmDlg : public CDialog
{
// Construction
public:
	CAlarmDlg(CWnd* pParent = NULL,LLONG iLoginId = 0);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CAlarmDlg)
	enum { IDD = IDD_DLG_ALARM };
	CListCtrl	m_List;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAlarmDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
    LLONG m_iLoginID;
    int	m_nAlarmIndex;
	// Generated message map functions
	//{{AFX_MSG(CAlarmDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnStartlisten();
    afx_msg LRESULT OnAlarmInfo(WPARAM wParam, LPARAM lParam);
	afx_msg void OnStoplisten();
	afx_msg void OnDestroy();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ALARMDLG_H__CF21D93E_FD45_4D00_9D11_A984CFEE0937__INCLUDED_)

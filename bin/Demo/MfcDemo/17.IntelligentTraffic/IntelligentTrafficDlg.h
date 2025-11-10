// IntelligentTrafficDlg.h : header file
//

#pragma once
#include "afxcmn.h"
#include "IntelligentEventDlg.h"
#include "BWListDlg.h"
#include "TrafficFlowDlg.h"
#include "QueryPictureAndRecordDlg.h"

// CIntelligentTrafficDlg dialog
class CIntelligentTrafficDlg : public CDialog
{
// Construction
public:
	CIntelligentTrafficDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_IntelligentTraffic_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	virtual BOOL PreTranslateMessage(MSG* pMsg);
    afx_msg void OnDestroy();
    afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
    afx_msg HCURSOR OnQueryDragIcon();
    afx_msg void OnTcnSelchangeTab(NMHDR *pNMHDR, LRESULT *pResult);
    afx_msg void OnBnClickedBtnLoginAndLogout();
	DECLARE_MESSAGE_MAP()

private:
    // Init
    void    InitDialogContorl(void);
    void    InitNetSdk(void);

    // Show child dialog of Tab control
    void    DoTab(int nTab);
    void    SetDlgState(CWnd *pWnd, BOOL bShow);

    // Init or Cleanup child dialog of Tab
    void    InitChildDlgOfTab(int nChannel, LLONG lLoginIHandle);
    void    CleanUpChildDlgOfTab();

    // Self define function
    LRESULT OnDeviceDisConnect(WPARAM wParam, LPARAM lParam);
    LRESULT OnDeviceReconnect(WPARAM wParam, LPARAM lParam);

private:
    CTabCtrl                m_ctrTab;
    
    // Dialog class
    CBWListDlg              m_dlgBWList;
    CTrafficFlowDlg         m_dlgTrafficFlow;
    CIntelligentEventDlg    m_dlgIntelligentEvent;
    CQueryTrafficPictureDlg m_dlgQueryPictureAndRecord;
    
    // Device login handle
    LLONG                   m_lLoginHandle;
};

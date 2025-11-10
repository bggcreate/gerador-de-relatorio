// DevInitDlg.h : header file
//

#if !defined(AFX_DEVINITDLG_H__F66F4C26_59B0_494F_B2D2_A32DEB38495B__INCLUDED_)
#define AFX_DEVINITDLG_H__F66F4C26_59B0_494F_B2D2_A32DEB38495B__INCLUDED_
#include "./include/dhnetsdk.h"
#include "ModifyIPDlg.h"
#include "GetIPDlg.h"
#include <vector>
#include <afxmt.h>
#include "MylistCtrl.h"
#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define MAX_DEV_INFO_COUNT      (1024*32)

#define WM_SEARCHDEVICEBYIPS	(WM_USER + 200)

/////////////////////////////////////////////////////////////////////////////
// CDevInitDlg dialog

class CDevInitDlg : public CDialog
{
// Construction
public:
	friend void CALLBACK DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser);
	friend void CALLBACK ReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser);
	friend void CALLBACK cbSearchDevices(DEVICE_NET_INFO_EX *pDevNetInfo, void* pUserData);
	void ChangeDevNetIp(CString strIP,int nIPversion,CString strUserName,CString strPassWord,CString strSubnetMask,CString strGateWay);
	CDevInitDlg(CWnd* pParent = NULL);	// standard constructor	
// Dialog Data
	//{{AFX_DATA(CSearchDeviceDlg)
	enum { IDD = IDD_DEVINIT_DIALOG };
	int		m_nSearchTime;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDevInitDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CDevInitDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonSearch();
	afx_msg void OnDestroy();
	afx_msg void OnButtonSearchdevicesbyips();
	afx_msg LRESULT OnSearchDevice(WPARAM w, LPARAM l);
	afx_msg void OnButtonInitDevice();
	afx_msg void OnButtonResetPassword();
	afx_msg LRESULT BeginSearchdevicesbyips(WPARAM dwStartIP, LPARAM dwEndIP);
	afx_msg void OnButtonModifyIp();
	afx_msg LRESULT OnDisConnect(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnReConnect(WPARAM wParam, LPARAM lParam);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	int  m_nDeviceCount;           //当前vector中的元素的个数
	int  m_nSelected;              //当前选中的第几个元素
	LLONG m_lpSearch;              //是否在搜索
	CString m_strPwdResetWay;      //密码重置方式
    CMylistCtrl	m_ctlListInfo;      
	//vector to store  information of devices
	std::vector<DEVICE_NET_INFO_EX> m_DevNetInfo; 
	CCriticalSection m_criticalSection;

private:
	void InitListView();
	void InitNetSDK();
	BOOL GetInitStatus(BYTE initStatus);
	void AddDeviceInfoToListView(DEVICE_NET_INFO_EX *pDevNetInfo);
	void IPtoStr(DWORD ip, char* buf, unsigned int nBufferSize);
	void GetPwdRestWay(BYTE pwdRestWay);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DEVINITDLG_H__F66F4C26_59B0_494F_B2D2_A32DEB38495B__INCLUDED_)

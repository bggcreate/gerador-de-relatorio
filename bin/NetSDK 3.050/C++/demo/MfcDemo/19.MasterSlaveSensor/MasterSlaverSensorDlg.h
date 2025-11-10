// MasterSlaver_watcherDlg.h : header file
//

#if !defined(AFX_MASTERSLAVER_WATCHERDLG_H__E0B0E7DC_BDDD_4D28_8013_7A73BEB0521F__INCLUDED_)
#define AFX_MASTERSLAVER_WATCHERDLG_H__E0B0E7DC_BDDD_4D28_8013_7A73BEB0521F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "dhnetsdk.h"
#include "dhconfigsdk.h"
#include "ptzScreen.h"
#include "resource.h"
#include "ExButton.h"

/////////////////////////////////////////////////////////////////////////////
// CMasterSlaver_watcherDlg dialog

class CMasterSlaver_watcherDlg : public CDialog
{
// Construction
public:
	CMasterSlaver_watcherDlg(CWnd* pParent = NULL);	// standard constructor
	~CMasterSlaver_watcherDlg();
	friend void CALLBACK  RectEventFunc(int nWndID,RECT WinRect,CPoint &pointStart,CPoint &pointEnd,DWORD dwUser, int nID);

	enum { IDD = IDD_MASTERSLAVERSENSOR_DIALOG };
	CComboBox	m_cmbSelectGroup;
	CComboBox	m_cmbSelectNumber;
	CComboBox	m_comboPTZData;
	CComboBox	m_comboDispNum;
	CComboBox	m_ctlChn;
	CIPAddressCtrl	m_DvrIPAddr;
	CIPAddressCtrl	m_PTZIPAddr;
	CString	m_csPassWord;
	CString	m_csUser;
	CString	m_csRemotePassword;
	CString	m_csRemoteUse;
	/* 主从坐标系*/
	int		m_Ym1;
	int		m_Xm1;
	float		m_Xs1;
	float		m_Ys1;
	int		m_Xm2;
	int		m_Ym2;
	float		m_Xs2;
	float		m_Ys2;
	int		m_Xm3;
	int		m_Ym3;
	float		m_Xs3;
	float		m_Ys3;
	int		m_Xm4;
	int		m_Ym4;
	float		m_Xs4;
	float		m_Ys4;
	/* 球机云台控制方向*/
	CExButton	m_ptz_rightdown;
	CExButton	m_ptz_rightup;
	CExButton	m_ptz_leftdown;
	CExButton	m_ptz_leftup;
	CExButton	m_ptz_right;
	CExButton	m_ptz_left;
	CExButton	m_ptz_down;
	CExButton	m_ptz_up;
	CExButton	m_zoom_tele;
	CExButton	m_zoom_wide;

protected:
	HICON m_hIcon;
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg void OnDestroy();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonLogin();
	afx_msg void OnButtonPlay();
	afx_msg void OnButtonLogout();
	afx_msg void OnButtonStopplay();
	afx_msg void OnButtonAddRemote();
	afx_msg void OnButtonBond();
	afx_msg void OnButtonLoginremote();
	afx_msg void OnButtonRemotelogout();
	afx_msg void OnButtonRemoteplay();
	afx_msg void OnButtonRemotestop();
	afx_msg void OnButtonCalibrateEachsensor();
	afx_msg void OnButtonDone();
	afx_msg void OnBUTTONnew();
	afx_msg void OnBnClickedGetSlavepositon1();
	afx_msg void OnBnClickedGetSlavepositon2();
	afx_msg void OnBnClickedGetSlavepositon3();
	afx_msg void OnBnClickedGetSlavepositon4();
	afx_msg void OnSelchangeChannel();
	afx_msg void OnBnClickedButtonStartcalibrate();
	LRESULT OnMasterSlaverPoint(WPARAM wParam, LPARAM lParam);
	LRESULT OnDeviceReconnect(WPARAM wParam, LPARAM lParam);
	LRESULT OnDeviceDisConnect(WPARAM wParam, LPARAM lParam);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void UpdataScreenPos(void);
	void RectEvent(int nWndID,RECT WinRect,CPoint &pointStart,CPoint &pointEnd,int nID,HWND hwnd);
	void InitComboBox();
	void CloseDispVideo(int iDispNum);
	void SetCalibrateValue(int nGroup, int nID); // 配置第几目，第几组
	void ShowCalibrate(CFG_CALIBRATE_MATRIX_ELEMENT  stuCalibrate); // 显示每组标定数据
	void SetCalibrateBySensor(int nSensor); // 设置每组的标定
	void ClearEdit(); // 清空所有标定edit控件
	void ShowCalibrateBySensor(int nSensor);// 显示每组标定数据
	void InitPTZControl();
	BOOL CheckPoint();
	void DlgEnableWindow(BOOL bFlag);
	void UnInitNetSDK();

private:
	LLONG m_lLoginID;//登陆句柄
	LLONG m_lLoginID_REMOTE;//远程设备登陆句柄
	LONG    m_lPlayID;//实时监控返回句柄
	LONG    m_lPlayID_REMOTE;//远程设备实时监控返回句柄
	void ShowLoginErrorReason(int nError);//提示错误原因
	CPtzScreen m_ptzScreen;
	CPtzScreen m_ptzScreen1;
	CRect	m_clientRect;
	CRect	m_screenRect;
	int		m_CurScreen;
	long m_DispHanle[16];
	int m_DispChn[16];
	long m_Group[4];
	int     m_left;
	int     m_right;
	int     m_top;
	int     m_buttom;
	int     m_method; // 1代表框选联动方式，0代表点击联动方式
	int		m_ChnCount;
	int		m_nGroupCount;
	int		m_Zoomchn;// 球机通道
	int		m_nCalibrateDone; // 标定完成标志
	char m_szSlaveSerial[MASTERSLAVE_DEV_SERIALNO_LEN];      // 球机序列号
	char m_szMasterSerial[MASTERSLAVE_DEV_SERIALNO_LEN];     // IPC序列号
	NET_POINT_PAIR_EX m_stuPointPairEx[8]; // 标定对
	NET_MULTI_SENSOR_INFO m_stuOutCalibrateConfig; // 获取的配置信息
	int m_nSensorNum;							   // 守望者有几目
	int m_nGroupNum;							   // 每目标定几组
	int m_nSelceCout[8];
	int m_nCalibrateNum;						   // 标定了几组
	int m_nRectDirection;
	
public:
	void PtzControl(int type, BOOL stop);

public:
	float m_Zs1;
	float m_Zs2;
	float m_Zs4;
	float m_Zs3;	
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MASTERSLAVER_WATCHERDLG_H__E0B0E7DC_BDDD_4D28_8013_7A73BEB0521F__INCLUDED_)

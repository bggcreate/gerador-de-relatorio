// PlayDemoDlg.h : header file
//

#if !defined(AFX_PLAYDEMODLG_H__BA041EC9_0353_436D_A78A_C94792269777__INCLUDED_)
#define AFX_PLAYDEMODLG_H__BA041EC9_0353_436D_A78A_C94792269777__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "DlgOpenFile.h"
#include "HoverButton.h"
#include "NiceSlider.h"
#include "DisplayWnd.h"
#include "PlayStateText.h"
#include "DlgPicSetting.h"
#include "DlgSetColor.h"
#include "CutFileDlg.h"
#include "DecodeEngineDlg.h"
#include "MediaInfoDlg.h"
#include "Player.h"
#include "EasySize.h"

enum MENU_STATE{FILEOPEN, FILECLOSE, FILEINDEXCREATED, STOPPLAY, STARTPLAY,ENABLEFISHEYE,DISABLEFISHEYE};
/////////////////////////////////////////////////////////////////////////////
// CPlayDemoDlg dialog

class CPlayDemoDlg : public CDialog
{
// Construction
public:
	CPlayDemoDlg(CWnd* pParent = NULL);	// standard constructor

	DECLARE_EASYSIZE

// Dialog Data
	//{{AFX_DATA(CPlayDemoDlg)
	enum { IDD = IDD_PLAYDEMO_DIALOG };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPlayDemoDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;
	HACCEL m_hotKey;
	// Generated message map functions
	//{{AFX_MSG(CPlayDemoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg void OnSize(UINT nType, int cx, int cy); 
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnFileOpen();
	afx_msg void OnFileClose();
	afx_msg void OnLocate();
	afx_msg void OnCutFile();
	afx_msg void OnMediaInfo();
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnClose();
	afx_msg void OnHelpAbout();
	afx_msg void OnSettingSnappic();
	afx_msg void OnSettingPrimodialsize();
	afx_msg void OnVerticalSync();
	afx_msg void OnIVS();
	afx_msg LRESULT OnEnctypeChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnSrcArea(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnIndexCreated(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnFisheyeDetect(WPARAM wParam, LPARAM lParam);
	afx_msg void OnOperator(UINT uID);
	afx_msg void OnButtonAudio();
	afx_msg void OnButtonPicture();
	afx_msg void OnButtonSetcolor();
	afx_msg void OnButtonBackword();
	afx_msg void OnButtonForword();
	afx_msg void OnDropFiles(HDROP hDropInfo);
	afx_msg void OnButtonStop();
	afx_msg void OnDevCase();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
protected:
	void OpenFile();
	int ChangeUIState(PLAY_STATE nState);
	int ChangeSingleUIState(PLAY_STATE nState, BOOL bEnable);
	int ChangeMenuState(MENU_STATE nState);
	int MediaInit();
	int OpenAudio();
	int CloseAudio();

private:
	
	
	CNiceSliderCtrl m_sdProc;
	CNiceSliderCtrl	m_sdAudioWave;
	CNiceSliderCtrl	m_sdAuidoVolume;

	
	CHoverButton	m_bnPlay;
	CHoverButton	m_bnPause;
	CHoverButton	m_bnStop;
	CHoverButton	m_bnToEnd;
	CHoverButton	m_bnToBegin;
	CHoverButton	m_bnBackOne;
	CHoverButton	m_bnStepOne;
	CHoverButton	m_bnSlow;
	CHoverButton	m_bnFast;
	CHoverButton	m_bnPicCatch;
	CHoverButton	m_bnSetColor;
	CHoverButton	m_bnAudio;
	CHoverButton	m_bnBackword;
	CHoverButton	m_bnForword;

	
	CDlgSetColor m_dlgSetColor;
	

	CDlgOpenFile m_dlgOpenFile;

	
	CDisplayWnd m_dlgDisplay;


	CPlayStateText m_dlgStateText;


	CDlgPicSetting m_dlgPicSetting;

	DecodeEngineDlg m_dlgDecodeEngine;

	PLAY_STATE m_lastState;

	UINT m_lastFisheyeMode;
	CToolTipCtrl m_ContentTip;
public:
	afx_msg void OnFisheyeviewWall1pplus8();
	afx_msg void OnFisheyeviewFloor1pplus6();
	afx_msg void OnFisheyeviewFloor1plus2();
	afx_msg void OnFisheyeviewCeil1pplus1();
	afx_msg void OnFisheyeviewCeil1plus3();
	afx_msg void OnFisheyeviewFloor2p();
	afx_msg void OnSettingStartFisheye();
	afx_msg void OnSettingDecodeEngine();
	int m_nMoveFrontDistance;
	int m_nMoveBackDistance;
	int m_nMoveLeftDistance;
	int m_nMoveRightDistance;
	int m_nMoveTopDistance;
	int m_nMoveBottomDistance;
	double m_dTotalDistanceZ;
	double m_dTotalDistanceX;
	double m_dTotalDistanceY;

	double m_nLastTime;
	double m_nNowTime;

	int m_nThrowMode;
private:
	int m_DisplayAngle;
public:
	void ChangeAngle(int type);
	afx_msg void OnRotateAngle(UINT ID);
	afx_msg void OnSettingThrowframe();
	afx_msg void OnFisheyeviewSemi();
	afx_msg void OnFisheyeviewCylinder();
	afx_msg void OnFisheyeviewLittle();
	afx_msg void OnFisheyeview360Sphere();
	afx_msg void OnFisheyeview360Cylinder();
	afx_msg void OnAntiAliasing();

	afx_msg void OnSemiCeil();
	afx_msg void OnSemiFloor();
	afx_msg void OnSemiWall();
	afx_msg void OnCylinderCeil();
	afx_msg void OnCylinderFloor();
	afx_msg void OnCylinderWall();
	afx_msg void OnLittleCeil();
	afx_msg void OnLittleFloor();
	afx_msg void OnLittleWall();
	afx_msg void OnSetThrowMode(UINT ID);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PLAYDEMODLG_H__BA041EC9_0353_436D_A78A_C94792269777__INCLUDED_)

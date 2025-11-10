#pragma once
#include "afxwin.h"
#include "afxcmn.h"
#include "PictureCtrl.h"
#include <atlimage.h>

struct TRAFFIC_INTELLIGENT_EVENT_INFO; 

// CIntelligentEventDlg Dialog

class CIntelligentEventDlg : public CDialog
{
	DECLARE_DYNAMIC(CIntelligentEventDlg)

public:
	CIntelligentEventDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CIntelligentEventDlg();

// Dialog Data
	enum { IDD = IDD_INTELLIGENT_EVENT_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    afx_msg void OnBnClickedBtnStartPlayAndStopPlay();
    afx_msg void OnBnClickedBtnSubscirbeAndUnsubsicribe();
    afx_msg void OnBnClickedBtnManalSnap();
    DECLARE_MESSAGE_MAP()
    

public:
    void Init(unsigned int nChannel, LLONG lLoginHandle);
    void CleanUp();

private:
    // Self-define function WM_INTELLIGENT_EVENT
    LRESULT OnIntelligentEvent(WPARAM wParam, LPARAM lParam);

    // Show intelligent info in list control
    void ShowIntelligentEvnetInfo(TRAFFIC_INTELLIGENT_EVENT_INFO* pTrafficEvent);

private:
    CButton m_btnPlay;
    CButton m_btnSubscribe;
    CListCtrl m_ctrEventList;
    CComboBox m_cmbChannel;
    PictureCtrl m_ctrEventPicture;
    PictureCtrl m_ctrPlatePicture;

    LLONG m_lLoginHandle;
    LLONG m_lPlayHandle;
    LLONG m_lRealLoadHandle;
    int	  m_nIndexOfEvent;           
};

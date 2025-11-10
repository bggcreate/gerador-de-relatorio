// AviConvert_DemoDlg.h : header file
//

#if !defined(AFX_AVICONVERT_DEMODLG_H__30DD6708_8613_4399_817C_C3D97C7E468C__INCLUDED_)
#define AFX_AVICONVERT_DEMODLG_H__30DD6708_8613_4399_817C_C3D97C7E468C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include "dhplay.h"

/////////////////////////////////////////////////////////////////////////////
// CAviConvert_DemoDlg dialog

#define UM_MESSAGEPRESENT WM_USER+1
#define UM_MESSAGECOMPLETE WM_USER+2

class CAviConvert_DemoDlg : public CDialog
{
// Construction
public:
	CAviConvert_DemoDlg(CWnd* pParent = NULL);	// standard constructor
	typedef enum
	{
		AVI_CONVERT,
		ASF_CONVERT,
		MP4_CONVERT,
        PS_CONVERT,
        TS_CONVERT,
	}ConvertType;
	typedef enum
	{
		CONVERT,
		CANCEL
	}STATE;
// Dialog Data
	//{{AFX_DATA(CAviConvert_DemoDlg)
	enum { IDD = IDD_AVICONVERT_DEMO_DIALOG };
	int		m_nConvertType;
	long	m_lwidth;
	long	m_lheight;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAviConvert_DemoDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CAviConvert_DemoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonSrcpath();
	afx_msg void OnButtonDestpath();
	afx_msg void OnButtonConvert();
	afx_msg void OnButtonCancel();
	afx_msg void OnRadioAsf();
	afx_msg void OnRadioAvi();
	afx_msg void OnClose();
	afx_msg void OnRadioMp4();
    afx_msg void OnBnClickedRadioPs();
    afx_msg void OnBnClickedRadioTs();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	BOOL m_bConverting;
	HANDLE m_hconvertThread;
public:
	CString m_csSourceFile;
	CString m_csTargetFile;
	int m_nChangeCount;

	CFile m_fSrcFile;
	void AviConvert();
	void ChangeUIState(int nState);
	bool StartConvert();
	void StopConvert();
    int ConvertType(int nDataType);

	LRESULT OnPresent(WPARAM mParam, LPARAM lParam);
	LRESULT OnComplete(WPARAM mParam, LPARAM lParam);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_AVICONVERT_DEMODLG_H__30DD6708_8613_4399_817C_C3D97C7E468C__INCLUDED_)

// DecCB_demoDlg.h : header file
//

#if !defined(AFX_DECCB_DEMODLG_H__BC21A5C1_38FD_4CF6_B089_A7210591EC5D__INCLUDED_)
#define AFX_DECCB_DEMODLG_H__BC21A5C1_38FD_4CF6_B089_A7210591EC5D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CDecCB_demoDlg dialog
#include "DlgDisplay.h"

class CDecCB_demoDlg : public CDialog
{
// Construction
public:
	CDecCB_demoDlg(CWnd* pParent = NULL);	// standard constructor

	typedef enum
	{
		STARTDEC,
		STOPDEC
	}DECSTATE;

	typedef enum
	{
		DEC_VIDEO,
		DEC_AUDIO,
		DEC_COMPLEX,
	}DECTYPE;
// Dialog Data
	//{{AFX_DATA(CDecCB_demoDlg)
	enum { IDD = IDD_DECCB_DEMO_DIALOG };
	CString	m_csSrcFilePath;
	CString	m_csAudiofilePath;
	CString	m_csVideofilePath;
	FILE	*m_pOutAudioFile;
	FILE	*m_pOutVideoFile;
	int		m_nDecType;
	int		m_nInterfaceType;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDecCB_demoDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CDecCB_demoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonFilepath();
	afx_msg void OnButtonDecode();
	afx_msg void OnButtonStop();
	afx_msg void OnRadioAudio();
	afx_msg void OnRadioComplex();
	afx_msg void OnRadioVideo();
	afx_msg void OnClose();
	afx_msg void OnButtonAudiofile();
	afx_msg void OnButtonVideofile();
	afx_msg void OnInterfaceNew();
	afx_msg void OnInterfaceOld();
	afx_msg void OnInterfaceNew2();
	afx_msg void OnInterfaceOld2();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
protected:
	void ChangeUIState(int nState);
private:
	CDlgDisplay m_displayDlg;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DECCB_DEMODLG_H__BC21A5C1_38FD_4CF6_B089_A7210591EC5D__INCLUDED_)

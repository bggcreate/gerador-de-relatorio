// SoundCapture_demoDlg.h : header file
//

#if !defined(AFX_SOUNDCAPTURE_DEMODLG_H__52E01F2C_8C45_4FA8_85E6_27B8870C69B2__INCLUDED_)
#define AFX_SOUNDCAPTURE_DEMODLG_H__52E01F2C_8C45_4FA8_85E6_27B8870C69B2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "dhplay.h"

/////////////////////////////////////////////////////////////////////////////
// CSoundCapture_demoDlg dialog

class CSoundCapture_demoDlg : public CDialog
{
// Construction
public:
	CSoundCapture_demoDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CSoundCapture_demoDlg)
	enum { IDD = IDD_SOUNDCAPTURE_DEMO_DIALOG };
	CComboBox	m_cSamplePerSecondBox;
	CComboBox	m_cBitPerSampleBox;
	CString	m_csFileName;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSoundCapture_demoDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CSoundCapture_demoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButtonRecord();
	afx_msg void OnButtonStop();
	afx_msg void OnButtonFilepath();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	void InitCombox();

	//CString m_csFileName;
private:	
	int	m_nBitPerSample;
	int	m_nSamplePerSecond;
	FILE *m_fPCM;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SOUNDCAPTURE_DEMODLG_H__52E01F2C_8C45_4FA8_85E6_27B8870C69B2__INCLUDED_)

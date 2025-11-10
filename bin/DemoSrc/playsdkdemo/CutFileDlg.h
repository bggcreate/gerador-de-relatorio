#if !defined(AFX_CUTFILEDLG_H__4DAC3A35_7D38_41C8_854A_36FBAA1E6C76__INCLUDED_)
#define AFX_CUTFILEDLG_H__4DAC3A35_7D38_41C8_854A_36FBAA1E6C76__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CutFileDlg.h : header file
//

#include "dhplay.h"

/////////////////////////////////////////////////////////////////////////////
// CCutFileDlg dialog

class CCutFileDlg : public CDialog
{
// Construction
public:
	CCutFileDlg(CString originfile, CWnd* pParent = NULL);   // standard constructor

	typedef enum{
		CUTBYFRAMENUM, CUTBYTIME
	}CUTTYPE;
	FRAME_POS m_RealBegin;
	FRAME_POS m_RealEnd;
	DWORD m_nMaxTime ;
	DWORD m_nMaxFrameNum ;
	CString m_originfile ;
// Dialog Data
	//{{AFX_DATA(CCutFileDlg)
	enum { IDD = IDD_DIALOG_CUTFILE };
	int		m_cutType;
	int		m_startpos;
	int		m_endpos;
	int		m_realStartPos;
	int		m_realEndPos;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCutFileDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCutFileDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnButtonSave();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CUTFILEDLG_H__4DAC3A35_7D38_41C8_854A_36FBAA1E6C76__INCLUDED_)

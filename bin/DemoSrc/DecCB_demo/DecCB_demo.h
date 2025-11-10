// DecCB_demo.h : main header file for the DECCB_DEMO application
//

#if !defined(AFX_DECCB_DEMO_H__1AD8FE70_C18F_4382_9B27_58BCF0DD1451__INCLUDED_)
#define AFX_DECCB_DEMO_H__1AD8FE70_C18F_4382_9B27_58BCF0DD1451__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CDecCB_demoApp:
// See DecCB_demo.cpp for the implementation of this class
//

class CDecCB_demoApp : public CWinApp
{
public:
	CDecCB_demoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDecCB_demoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CDecCB_demoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DECCB_DEMO_H__1AD8FE70_C18F_4382_9B27_58BCF0DD1451__INCLUDED_)

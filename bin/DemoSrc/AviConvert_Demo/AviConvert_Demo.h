// AviConvert_Demo.h : main header file for the AVICONVERT_DEMO application
//

#if !defined(AFX_AVICONVERT_DEMO_H__F48E9796_15CE_41F6_ABA9_6D3C3F0A2A79__INCLUDED_)
#define AFX_AVICONVERT_DEMO_H__F48E9796_15CE_41F6_ABA9_6D3C3F0A2A79__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CAviConvert_DemoApp:
// See AviConvert_Demo.cpp for the implementation of this class
//

class CAviConvert_DemoApp : public CWinApp
{
public:
	CAviConvert_DemoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAviConvert_DemoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CAviConvert_DemoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_AVICONVERT_DEMO_H__F48E9796_15CE_41F6_ABA9_6D3C3F0A2A79__INCLUDED_)

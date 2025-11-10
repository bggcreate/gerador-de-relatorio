// WaterCheck_demo.h : main header file for the WATERCHECK_DEMO application
//

#if !defined(AFX_WATERCHECK_DEMO_H__E84C3F95_DD01_41E6_9638_B5071602FC49__INCLUDED_)
#define AFX_WATERCHECK_DEMO_H__E84C3F95_DD01_41E6_9638_B5071602FC49__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CWaterCheck_demoApp:
// See WaterCheck_demo.cpp for the implementation of this class
//

class CWaterCheck_demoApp : public CWinApp
{
public:
	CWaterCheck_demoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CWaterCheck_demoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CWaterCheck_demoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_WATERCHECK_DEMO_H__E84C3F95_DD01_41E6_9638_B5071602FC49__INCLUDED_)

// SoundCapture_demo.h : main header file for the SOUNDCAPTURE_DEMO application
//

#if !defined(AFX_SOUNDCAPTURE_DEMO_H__67EC26A7_CA21_4E15_B234_D17C22B4FADF__INCLUDED_)
#define AFX_SOUNDCAPTURE_DEMO_H__67EC26A7_CA21_4E15_B234_D17C22B4FADF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CSoundCapture_demoApp:
// See SoundCapture_demo.cpp for the implementation of this class
//

class CSoundCapture_demoApp : public CWinApp
{
public:
	CSoundCapture_demoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSoundCapture_demoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CSoundCapture_demoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SOUNDCAPTURE_DEMO_H__67EC26A7_CA21_4E15_B234_D17C22B4FADF__INCLUDED_)

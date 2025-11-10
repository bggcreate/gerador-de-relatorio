// FaceRecognition.h : main header file for the FACERECOGNITION application
//

#if !defined(AFX_FACERECOGNITION_H__2026399B_215E_4E6B_97FC_56A3F02A3127__INCLUDED_)
#define AFX_FACERECOGNITION_H__2026399B_215E_4E6B_97FC_56A3F02A3127__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CFaceRecognitionApp:
// See FaceRecognition.cpp for the implementation of this class
//


class CFaceRecognitionApp : public CWinApp
{
public:
	CFaceRecognitionApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFaceRecognitionApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CFaceRecognitionApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FACERECOGNITION_H__2026399B_215E_4E6B_97FC_56A3F02A3127__INCLUDED_)

// MasterSlaver_watcher.h : main header file for the MASTERSLAVER_WATCHER application
//

#if !defined(AFX_MASTERSLAVER_WATCHER_H__E484A288_24D5_41DE_B65A_70EFD3F67A71__INCLUDED_)
#define AFX_MASTERSLAVER_WATCHER_H__E484A288_24D5_41DE_B65A_70EFD3F67A71__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CMasterSlaver_watcherApp:
// See MasterSlaver_watcher.cpp for the implementation of this class
//

class CMasterSlaver_watcherApp : public CWinApp
{
public:
	CMasterSlaver_watcherApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMasterSlaver_watcherApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CMasterSlaver_watcherApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MASTERSLAVER_WATCHER_H__E484A288_24D5_41DE_B65A_70EFD3F67A71__INCLUDED_)

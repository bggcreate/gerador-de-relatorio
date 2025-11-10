// IntelligentTraffic.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CIntelligentTrafficApp:
// See IntelligentTraffic.cpp for the implementation of this class
//

class CIntelligentTrafficApp : public CWinApp
{
public:
	CIntelligentTrafficApp();

// Overrides
	public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CIntelligentTrafficApp theApp;

void g_SetWndStaticText(CWnd * pWnd);
CString ConvertString(CString strText);


// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if !defined(AFX_STDAFX_H__7BC137AB_7831_42D4_BEB2_29B92529FD3F__INCLUDED_)
#define AFX_STDAFX_H__7BC137AB_7831_42D4_BEB2_29B92529FD3F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define VC_EXTRALEAN		// Exclude rarely-used stuff from Windows headers

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#include <afxdisp.h>        // MFC Automation classes
#include <afxdtctl.h>		// MFC support for Internet Explorer 4 Common Controls
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>			// MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT


//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

/*return module path*/
CString GetMoudlePath();

#define WM_USER_MSD_INDEXCREATED   (WM_USER + 1)
#define WM_USER_MSD_FISHEYEDEVICE_DETECT   (WM_USER + 2)
#define WM_USER_MSG_ENCTYPECHANGED (WM_USER + 3)

#endif // !defined(AFX_STDAFX_H__7BC137AB_7831_42D4_BEB2_29B92529FD3F__INCLUDED_)

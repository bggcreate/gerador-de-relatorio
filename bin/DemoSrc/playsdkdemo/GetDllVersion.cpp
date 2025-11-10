// GetDllVersion.cpp: implementation of the CGetDllVersion class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "PlayDemo.h"
#include "GetDllVersion.h"
#include <MEMORY>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

BOOL GetDllVersion(TCHAR *pszDllName, CString& strVer)
{
	DWORD dwInfoSize = GetFileVersionInfoSize(pszDllName, NULL);
	if(!dwInfoSize)
		return FALSE;
	
	/* auto_ptr prevents multiple calls at time of return */
	std::auto_ptr<BYTE> lpData(new BYTE[dwInfoSize]);
	if( !GetFileVersionInfo(pszDllName, NULL, dwInfoSize, (void*)lpData.get()) )
		return FALSE;
	
	LPVOID	lpInfo;
	UINT nInfoLen;
	if( !VerQueryValue(lpData.get(), _T("\\"), &lpInfo, &nInfoLen) )
		return FALSE;
	
	VS_FIXEDFILEINFO fileInfo = {0};
	if(nInfoLen!=sizeof(fileInfo))
		return FALSE;
	
	memcpy(&fileInfo, lpInfo, nInfoLen);
	
	strVer.Format(_T("%d.%d.%d.%d"), 
		(fileInfo.dwFileVersionMS & 0xFFFF0000) >> 16,
		fileInfo.dwFileVersionMS & 0x0000FFFF,
		(fileInfo.dwFileVersionLS & 0xFFFF0000) >> 16,
		fileInfo.dwFileVersionLS & 0x0000FFFF);
	
	return TRUE;
}

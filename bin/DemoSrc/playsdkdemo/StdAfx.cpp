// stdafx.cpp : source file that includes just the standard includes
//	PlayDemo.pch will be the pre-compiled header
//	stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"
#include "CharactorTansfer.h"

CString GetMoudlePath()
{
	TCHAR szAppName[MAX_PATH];
	TCHAR szDir[MAX_PATH];
	TCHAR szDrive[MAX_PATH];
	GetModuleFileName(GetModuleHandle(NULL), szAppName, MAX_PATH);
	_tsplitpath(szAppName, szDrive, szDir, NULL, NULL);

	TCHAR szPath[MAX_PATH];
	_tmakepath(szPath, szDrive, szDir, NULL, NULL);

	return szPath;
}


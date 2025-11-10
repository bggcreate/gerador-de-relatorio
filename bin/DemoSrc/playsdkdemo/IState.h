// IState.h: interface for the IState class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ISTATE_H__E213B858_4FDB_4C00_AE3A_DE11C4E3D8D8__INCLUDED_)
#define AFX_ISTATE_H__E213B858_4FDB_4C00_AE3A_DE11C4E3D8D8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "FileManipulate.h"

enum PLAY_STATE{ NONE=-3, Open, Close,
Play = 0, Pause, Stop, ToBegin, ToEnd, BackOne, OneByOne, Slow, Fast, 
STATE_SIZE };
enum SPEED{SLOW4, SLOW3, SLOW2, SLOW1, NORMAL, FAST1, FAST2, FAST3, FAST4};

struct PlayParam
{
	
	long nPort;
	/*wnd for display*/
	HWND hWnd;

	float fSpeedCoff;
	/*is audio on*/
	BOOL bOpenAudio;
	BOOL bOverlayMode;
	COLORREF OverlayColor;
	HWND pMainWnd;
	BOOL bIndexCreated;

	/*used for file stream*/
	HANDLE lpfDav;
	__int64 nDavSize;
};

/*
* abstrate play state
* state translate according to state translate table
*/
interface IPlayState  
{
	/*operation on every state*/
	virtual BOOL Do(PlayParam* lpPlayParam) = 0;

	virtual TCHAR* Description(PlayParam* lpPlayParam) = 0;
};


#endif // !defined(AFX_ISTATE_H__E213B858_4FDB_4C00_AE3A_DE11C4E3D8D8__INCLUDED_)

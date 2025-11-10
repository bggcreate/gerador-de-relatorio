// PlayStateText.cpp: implementation of the CPlayStateText class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "PlayDemo.h"
#include "PlayStateText.h"
#include "CharactorTansfer.h"

#include <string>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CPlayStateText::CPlayStateText()
{
	Clear();
}

CPlayStateText::~CPlayStateText()
{

}

void CPlayStateText::SetState(CString szState)
{
	if(szState == "")
		return;
	m_szState = szState;
}

void CPlayStateText::SetPlayedTime(int nPlayedTime , int nTotalTime)
{
	m_nTotalTime = nTotalTime;
	m_nPlayedTime = nPlayedTime;
}

void CPlayStateText::SetPlayedFrame(int nPlayedFrame, int nTotoalFrame)
{
	m_nTotalFrame = nTotoalFrame;
	m_nPlayedFrame = nPlayedFrame;
}

void CPlayStateText::SetBitrate(double dbBitrate)
{
	m_dbBitrate = dbBitrate;
}

void CPlayStateText::SetRate(int nRate)
{
	m_nRate = nRate;
}

void CPlayStateText::SetPictureSize(int nWidth, int nHeight)
{
	m_nWidth = nWidth;
	m_nHeight = nHeight;
}

void CPlayStateText::Init(CWnd* lpWnd)
{
	m_lpWnd = lpWnd;
	CFont font;
	font.CreateFont(12,
		0,0,0,
		FW_NORMAL,
		FALSE,
		FALSE,
		0,
		ANSI_CHARSET,
		OUT_DEFAULT_PRECIS,
		CLIP_DEFAULT_PRECIS,
		DEFAULT_QUALITY,
		DEFAULT_PITCH | FF_SWISS,
		_T("Arial"));
	m_lpWnd->SetFont(&font, TRUE);
}

void CPlayStateText::Clear()
{
	m_szState = CString("");
	m_nPlayedTime = 0;
	m_nTotalTime = 0;
	m_nPlayedFrame = 0;
	m_nTotalFrame = 0;
	m_dbBitrate = 0;
	m_nWidth = 0;
	m_nHeight = 0;
	m_nRate = 0;
}

void ChangeChar(char* szBuf, int nSize, char cSrc, char cDest)
{
	for(int i = 0; i < nSize; i++)
	{
		if(szBuf[i] == cSrc) szBuf[i] = cDest;
	}
}

void CPlayStateText::Show()
{
	if(!m_nTotalFrame || !m_nTotalTime)
	{
		int nHour = (m_nPlayedTime/3600) % 24;
		int nMinute = (m_nPlayedTime%3600) /60;
		int nSecond = (m_nPlayedTime%60);
		TCHAR szText[256] = {0};
		swprintf(szText, L"%-85s%4d*%4d%8df/s%8dkb/s", m_szState.GetBuffer(m_szState.GetLength()), m_nWidth, m_nHeight, m_nRate, (int)m_dbBitrate);
	//	ChangeChar(szText, 255, 0, ' ');
		m_lpWnd->SetWindowText(szText);
	}
	else
	{
		int nHour = (m_nPlayedTime/3600) % 24;
		int nMinute = (m_nPlayedTime%3600) /60;
		int nSecond = (m_nPlayedTime%60);
		int nHour2 = (m_nTotalTime/3600) % 24;
		int nMinute2 = (m_nTotalTime%3600) /60;
		int nSecond2= (m_nTotalTime%60);

		TCHAR szText[256] = {0};
		swprintf(szText, L"%-40s%4d*%4d%4df/s%8dkb/s%6d/%-7d%02d:%02d:%02d/%02d:%02d:%02d", 
			m_szState.GetBuffer(m_szState.GetLength()), m_nWidth, m_nHeight, m_nRate, (int)m_dbBitrate, m_nPlayedFrame, m_nTotalFrame,
			nHour, nMinute, nSecond, nHour2, nMinute2, nSecond2);
		//ChangeChar(szText, 255, 0, ' ');

		m_lpWnd->SetWindowText(szText);
	}
}
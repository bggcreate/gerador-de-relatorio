// FileStreamPlayer.cpp: implementation of the CFileStreamPlayer class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "PlayDemo.h"
#include "FileStreamPlayer.h"
#include "dhplay.h"
#include "dhplayEx.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFileStreamPlayer::CFileStreamPlayer()
{
	m_lpPlayParam = NULL;
	m_hThread = NULL;
	m_hExit = NULL;
	m_bBegin = false;
}

CFileStreamPlayer::~CFileStreamPlayer()
{

}

int WINAPI ThreadProc(LPVOID lpvData)
{
	CFileStreamPlayer* lpPlayer = (CFileStreamPlayer*)lpvData;
	return lpPlayer->OnProc();
}

void CALLBACK EncTypeChangeFunC(long nPort, void* nUser)
{
	TRACE(_T("change\n"));

	PlayParam* lpPlayParam = (PlayParam*)nUser;

	PostMessage(lpPlayParam->hWnd, WM_USER_MSG_ENCTYPECHANGED, NULL, NULL);

}

__int64 SeekFile(HANDLE hf, __int64 distance, DWORD MoveMethod)
{
	LARGE_INTEGER li;

	li.QuadPart = distance;

	li.LowPart = SetFilePointer(hf, li.LowPart, &li.HighPart, MoveMethod);

	if(li.LowPart == INVALID_FILE_SIZE && GetLastError() != NO_ERROR)
		li.QuadPart = -1;

	return li.QuadPart;
}

int CFileStreamPlayer::Open(TCHAR* lpszFile, PlayParam* lpPlayParam)
{
	HANDLE fDav = CreateFile(lpszFile,GENERIC_READ,FILE_SHARE_READ,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_READONLY,NULL);
	if(fDav == INVALID_HANDLE_VALUE)
		return -1;

	__int64 fPos = SeekFile(fDav,0,FILE_END);
	if(fPos == -1)
		return -1;

	SeekFile(fDav,0,FILE_BEGIN);
	lpPlayParam->lpfDav = fDav;
	lpPlayParam->nDavSize = fPos;
	 
	m_lpPlayParam = lpPlayParam;
	
	PLAY_SetEncTypeChangeCallBack(lpPlayParam->nPort, EncTypeChangeFunC, (void*)lpPlayParam);
	PLAY_SetStreamOpenMode(lpPlayParam->nPort, STREAME_FILE);
	PLAY_OpenStream(lpPlayParam->nPort, NULL, 0, 800*1024);

	m_hExit = CreateEvent(NULL, FALSE, FALSE, NULL);
	return 1;
}

int CFileStreamPlayer::Close(PlayParam* lpPlayParam)
{
	SetEvent(m_hExit);
	if(WaitForSingleObject(m_hThread, 5000) != WAIT_OBJECT_0)
	{
		TerminateThread(m_hThread, -1);
	}

	PLAY_Stop(lpPlayParam->nPort);
	PLAY_CloseStream(lpPlayParam->nPort);

	if(lpPlayParam->lpfDav != INVALID_HANDLE_VALUE)
	{
		CloseHandle(lpPlayParam->lpfDav);
		lpPlayParam->lpfDav = INVALID_HANDLE_VALUE;
	}

	if (NULL != m_hExit)
	{
		CloseHandle(m_hExit);
		m_hExit = NULL;
	}

	if (NULL != m_hThread)
	{
		CloseHandle(m_hThread);
		m_hThread = NULL;
	}

	return 1;
}

int CFileStreamPlayer::Do(PlayParam* lpPlayParam)
{
	TCHAR * pDes = m_lpState->Description(lpPlayParam);
	if(pDes && _tcscmp(pDes,_T("Stop")) == 0)
	{
		SetEvent(m_hExit);
		if(WaitForSingleObject(m_hThread, 5000) != WAIT_OBJECT_0)
		{
			TerminateThread(m_hThread, -1);
		}
		CloseHandle(m_hThread);
		m_hThread = NULL;
	}
	else if(pDes && !m_hThread  && _tcscmp(pDes,_T("Play")) == 0)
	{
		DWORD dwThreadID;
		m_hThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ThreadProc, this, NULL, &dwThreadID);
	}
	else if (pDes && _tcscmp(pDes,_T("Begin")) == 0)
	{
		m_bBegin = true;
	}
	return m_lpState->Do(lpPlayParam);
}

int CFileStreamPlayer::OpenSound(PlayParam* lpPlayParam, BOOL bOpen)
{
	lpPlayParam->bOpenAudio = bOpen;
	if(bOpen)
		return PLAY_PlaySoundShare(lpPlayParam->nPort);
	else
		return PLAY_StopSoundShare(lpPlayParam->nPort);
}

int CFileStreamPlayer::SetAudioVolume(PlayParam* lpPlayParam, int nVolume)
{
	return PLAY_SetVolume(lpPlayParam->nPort, nVolume);
}

int CFileStreamPlayer::SetAudioWave(PlayParam* lpPlayParam, int nWave)
{
	return PLAY_AdjustWaveAudio(lpPlayParam->nPort, nWave);
}

int CFileStreamPlayer::GetAudioVolume(PlayParam* lpPlayParam)
{
	return PLAY_GetVolume(lpPlayParam->nPort);
}

TCHAR* CFileStreamPlayer::Description(PlayParam* lpPlayParam)
{
	return m_lpState->Description(lpPlayParam);
}

int CFileStreamPlayer::ChangeState(IPlayState* lpState)
{
	if(lpState==NULL)
		return -1;

	m_lpState = lpState;
	return 1;
}

int CFileStreamPlayer::GetProcess(PlayParam* lpPlayParam)
{
	if (0 == lpPlayParam->nDavSize)
	{
		return 0;
	}

	__int64 fPos = SeekFile(lpPlayParam->lpfDav,0,FILE_CURRENT);
	return int(fPos*100/lpPlayParam->nDavSize);
}

int CFileStreamPlayer::Seek(PlayParam* lpPlayParam, int nPos)
{
	if(nPos<0 || nPos>100)
		return -1;

	SeekFile(lpPlayParam->lpfDav,nPos*lpPlayParam->nDavSize/100,FILE_BEGIN);
	return 1;
}

int CFileStreamPlayer::SetColor(PlayParam* lpPlayParam, int nSaturation, int nBrightness, int nContrast, int nHue)
{
	return PLAY_SetColor(lpPlayParam->nPort, 0, nBrightness, nContrast, nSaturation, nHue);
}

int CFileStreamPlayer::GetColor(PlayParam* lpPlayParam, int* nSaturation, int* nBrightness, int* nContrast, int* nHue)
{
	return PLAY_GetColor(lpPlayParam->nPort, 0, nBrightness, nContrast, nSaturation, nHue);
}

int CFileStreamPlayer::GetPlayedTime(PlayParam* lpPlayParam)
{
	return 0;
}

int CFileStreamPlayer::GetTotalTime(PlayParam* lpPlayParam)
{
	return 0;
}

int CFileStreamPlayer::GetPlayedFrame(PlayParam* lpPlayParam)
{
	return 0;
}

int CFileStreamPlayer::GetTotalFrame(PlayParam* lpPlayParam)
{
	return 0;
}

int CFileStreamPlayer::GetRate(PlayParam* lpPlayParam, int* nRate)
{
	DWORD dwRate = PLAY_GetCurrentFrameRate(lpPlayParam->nPort);
	*nRate = dwRate;
	return dwRate;
}

int CFileStreamPlayer::GetBitrate(PlayParam* lpPlayParam, double* dbBitrate)
{
	return PLAY_GetRealFrameBitRate(lpPlayParam->nPort, dbBitrate);
}

BOOL CFileStreamPlayer::SnapPicture(PlayParam* lpPlayParam, char* szFileName, int nType)/*nType,0 bmp 1 jpg*/
{
	return PLAY_CatchPicEx(lpPlayParam->nPort, szFileName, tPicFormats(nType));
}

int CFileStreamPlayer::SetDisplayRegion(PlayParam* lpPlayParam, DWORD nRegionNum, DISPLAYRECT *pSrcRect, HWND hDestWnd, BOOL bEnable)
{
	return PLAY_SetDisplayRegion(lpPlayParam->nPort, nRegionNum, pSrcRect, hDestWnd, bEnable);
}

int CFileStreamPlayer::GetPicSize(PlayParam* lpPlayParam, long *width, long *height)
{
	return PLAY_GetPictureSize(lpPlayParam->nPort, width, height);
}

BOOL CFileStreamPlayer::SetVerticalSync(PlayParam *lpPlayParam, BOOL bVerticalSync)
{
	return PLAY_VerticalSyncEnable(lpPlayParam->nPort, bVerticalSync);
}

/*Receive video info. cmdType: PLAY_CMD_GetTime  Encoding time info;
							   PLAY_CMD_GetFileRate  Frame rate info;
							   PLAY_CMD_GetMediaInfo  Media info*/
BOOL CFileStreamPlayer::GetQueryInfo(PlayParam *lpPlayParam, int cmdType, char* buf, int buflen, int* returnlen)
{
	return PLAY_QueryInfo(lpPlayParam->nPort, cmdType, buf, buflen, returnlen);
}

BOOL CFileStreamPlayer::RegistDrawCallback(PlayParam *lpPlayParam, fDrawCBFun DrawCBFun, void* nUser)
{
	return PLAY_RigisterDrawFun(lpPlayParam->nPort, DrawCBFun, nUser);
}

BOOL CFileStreamPlayer::SetPlayDirection(PlayParam* lpPlayParam, DWORD emDirection)
{
	return PLAY_SetPlayDirection(lpPlayParam->nPort, emDirection);
}

BOOL CFileStreamPlayer::RenderPrivateData(PlayParam* lpPlayParam, BOOL bTrue)
{
	return PLAY_RenderPrivateData(lpPlayParam->nPort, bTrue, 0);
}

BOOL CFileStreamPlayer::StartFisheye(PlayParam* lpPlayParam)
{
	return PLAY_StartFisheye(lpPlayParam->nPort);
}

BOOL CFileStreamPlayer::OptFisheyeParams(PlayParam* lpPlayParam, FISHEYE_OPERATETYPE operatetype, FISHEYE_OPTPARAM* pOptParam)
{
	return PLAY_OptFisheyeParams(lpPlayParam->nPort, operatetype, pOptParam);
}

BOOL CFileStreamPlayer::FisheyeEptzUpdate(PlayParam* lpPlayParam, FISHEYE_EPTZPARAM* pEptzParam, BOOL bSecondRegion)
{
	return PLAY_FisheyeEptzUpdate(lpPlayParam->nPort, pEptzParam, bSecondRegion);

}

BOOL CFileStreamPlayer::StopFisheye(PlayParam* lpPlayParam)
{
	return PLAY_StopFisheye(lpPlayParam->nPort);
}

BOOL CFileStreamPlayer::SetFishEyeInfoCallBack(PlayParam* lpPlayParam, fFishEyeInfoFun pFishEyeInfoFun, void* pUserData)
{
	return PLAY_SetFishEyeInfoCallBack(lpPlayParam->nPort, pFishEyeInfoFun, pUserData);
}

BOOL CFileStreamPlayer::EnableLargePicAdjustment(PlayParam* lpPlayParam, BOOL bEnable)
{
	return PLAY_EnableLargePicAdjustment(lpPlayParam->nPort, bEnable);
}

BOOL CFileStreamPlayer::SetAntiAliasing(PlayParam *lpPlayParam, BOOL bAntiAliasing)
{
	return PLAY_AntiAliasEnable(lpPlayParam->nPort, bAntiAliasing);
}

int CFileStreamPlayer::OnProc()
{
	const int READ_SIZE = 1024*128;
	unsigned char* lpReadBuf = new unsigned char[READ_SIZE];
	/*Add bInput to prevent long-time loop*/
	BOOL bInput = TRUE;
	DWORD nReadSize = 0;
	while(WaitForSingleObject(m_hExit, 0) != WAIT_OBJECT_0)/*Set short time interval to prevent slow data sending during quick play*/
	{
		if (m_bBegin)
		{
			m_bBegin = false;
			bInput = true;
			Sleep(500);
		}

		if(bInput!=FALSE)
		{
			nReadSize = READ_SIZE;
			::ReadFile(m_lpPlayParam->lpfDav, lpReadBuf, READ_SIZE,&nReadSize,NULL);
			if(nReadSize <= 0)
			{
				Sleep(2);
				continue;
			}
		}

		/*Continue sending data if failed*/
		bInput = PLAY_InputData(m_lpPlayParam->nPort, lpReadBuf, nReadSize);
		if(bInput==FALSE)
		{
			Sleep(2);
		}
	}

	delete []lpReadBuf;

	return 1;
}

namespace FileStreamState
{
	/*CStop*/
	int CStop::Do(PlayParam* lpPlayParam)
	{
		PLAY_ResetBuffer(lpPlayParam->nPort, BUF_VIDEO_RENDER);
		PLAY_CleanScreen(lpPlayParam->nPort, 240, 240, 240, 0, 0);
		PLAY_Stop(lpPlayParam->nPort);
		SeekFile(lpPlayParam->lpfDav, 0, FILE_BEGIN);
		return 1;
	}
	TCHAR* CStop::Description(PlayParam* lpPlayParam)
	{
		return _T("Stop");
	}

	/*CToBegin*/
	int CToBegin::Do(PlayParam* lpPlayParam)
	{
		PLAY_ResetBuffer(lpPlayParam->nPort, BUF_VIDEO_SRC);
		PLAY_ResetBuffer(lpPlayParam->nPort, BUF_AUDIO_SRC);
		PLAY_ResetBuffer(lpPlayParam->nPort, BUF_VIDEO_RENDER);
		PLAY_ResetBuffer(lpPlayParam->nPort, BUF_AUDIO_RENDER);
		SeekFile(lpPlayParam->lpfDav, 0, FILE_BEGIN);
		return 1;
	}
	TCHAR* CToBegin::Description(PlayParam* lpPlayParam)
	{
		return _T("Begin");
	}

	/*CToEnd*/
	int CToEnd::Do(PlayParam* lpPlayParam)
	{
		SeekFile(lpPlayParam->lpfDav, 0, FILE_END);
		return 1;
	}
	TCHAR* CToEnd::Description(PlayParam* lpPlayParam)
	{
		return _T("End");
	}
}
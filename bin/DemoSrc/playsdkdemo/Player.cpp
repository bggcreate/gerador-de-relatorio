// Player.cpp: implementation of the CPlayer class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "PlayDemo.h"
#include "Player.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#include "FilePlayer.h"
#include "FileStreamPlayer.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CPlayer::CPlayer()
{	
	memset(m_lpState, 0, sizeof(m_lpState));
	m_lpPlay = NULL;

	m_playParam.nPort = 0;
	m_playParam.hWnd = 0;
	m_playParam.bOverlayMode = FALSE;
	m_playParam.OverlayColor = RGB(0, 0, 0);
	m_playParam.bOpenAudio = 0;

	memset(stateTable, 0, sizeof(stateTable));
	memset(openTable, 0, sizeof(openTable));
	memset(closeTable, 0, sizeof(closeTable));
	
	m_bStartFishEye = false;

	memset(&m_feOptParams, 0x00, sizeof(m_feOptParams));
	m_feOptParams.mainStreamSize.h = 4000;
	m_feOptParams.mainStreamSize.w = 3000;
	m_feOptParams.originX = 4096;
	m_feOptParams.originY = 4096;
	m_feOptParams.radius = 4096;
	m_feOptParams.mainMountMode = FISHEYEMOUNT_MODE_CEIL;

	FISHEYE_OUTPUTFORMAT* pFormat = new FISHEYE_OUTPUTFORMAT;
	memset(pFormat, 0x00, sizeof(FISHEYE_OUTPUTFORMAT));

	FISHEYE_SUBMODE* pSubMode = new FISHEYE_SUBMODE[9];
	memset(pSubMode, 0x00, sizeof(FISHEYE_SUBMODE)*9);
	//get screen resolution
	pFormat->mainShowSize.w = GetSystemMetrics(SM_CXSCREEN);
	pFormat->mainShowSize.h = GetSystemMetrics(SM_CYSCREEN);
	pFormat->subMode = pSubMode;
	m_feOptParams.outputFormat = pFormat;

	m_bNeedDrawFishEyeLines = false;
	
}

CPlayer::~CPlayer()
{
	memset(m_lpState, 0, sizeof(m_lpState));
	m_lpPlay = NULL;

	delete [] m_feOptParams.outputFormat->subMode;
	delete m_feOptParams.outputFormat;
}

CPlayer* CPlayer::Instance()
{
	static CPlayer splayer;
	return &splayer;
}


int CPlayer::Open(TYPE eType, TCHAR* lpszFile, HWND playWnd, HWND MainDlgWnd)
{
	if(eType == FILE)
		CreateFile();
	else if(eType == FILESTREAM)
		CreateFileStream();
	else
		return -1;
	
	m_playParam.bIndexCreated = FALSE;
	m_playParam.hWnd = playWnd;
	m_playParam.pMainWnd = MainDlgWnd;
	return m_lpPlay->Open(lpszFile, &m_playParam);
}

int CPlayer::Close()
{
	if(m_bStartFishEye)
	{
		m_bStartFishEye = false;
		m_bNeedDrawFishEyeLines = false;
		RegistDrawCallback(0, 0);
		m_lpPlay->StopFisheye(&m_playParam);
	}

	if(m_lpPlay)
		m_lpPlay->Close(&m_playParam);
	Destroy();

	return 1;
}

int CPlayer::GetProcess()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetProcess(&m_playParam);
}

int CPlayer::Seek(int nPos)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->Seek(&m_playParam, nPos);
}

int CPlayer::SetColor(int nSaturation, int nBrightness, int nContrast, int nHue)
{
	if(!m_lpPlay)
		return -1;
	
	return m_lpPlay->SetColor(&m_playParam, nSaturation, nBrightness, nContrast, nHue);
}

int CPlayer::GetColor(int* nSaturation, int* nBrightness, int* nContrast, int* nHue)
{
	if(!m_lpPlay)
		return -1;
	
	return m_lpPlay->GetColor(&m_playParam, nSaturation, nBrightness, nContrast, nHue);
}

int CPlayer::GetPlayedTime()
{
	if(!m_lpPlay)
		return -1;
	
	return m_lpPlay->GetPlayedTime(&m_playParam);
}

int CPlayer::GetTotalTime()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetTotalTime(&m_playParam);
}

int CPlayer::GetPlayedFrame()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetPlayedFrame(&m_playParam);
}

int CPlayer::GetTotalFrame()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetTotalFrame(&m_playParam);
}

int CPlayer::GetPicture(int* nWidth, int* nHeight)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetPicSize(&m_playParam, (long *)nWidth, (long *)nHeight);
}

int CPlayer::GetRate(int* nRate)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetRate(&m_playParam, nRate);
}

int CPlayer::GetBitrate(double* dbBitrate)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetBitrate(&m_playParam, dbBitrate);
}

int CPlayer::SnapPicture(char* szFileName, int nType)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->SnapPicture(&m_playParam, szFileName, nType);
}

BOOL CPlayer::GetPicSize(long *width, long *height)
{
	if (m_lpPlay == NULL)
		return FALSE;
	return m_lpPlay->GetPicSize(&m_playParam, width, height);
}

BOOL CPlayer::SetCurrentFrameNum(int frameNum)
{
	return m_lpPlay->SetCurrentFrameNum(&m_playParam, frameNum);
}

BOOL CPlayer::SetPlayedTiemEx(int time)
{
	return m_lpPlay->SetPlayedTimeEx(&m_playParam, time);
}

int CPlayer::SetDisplayRegion(DWORD nRegionNum, RECT *pSrcRect, HWND hDestWnd, BOOL bEnable)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->SetDisplayRegion(&m_playParam, nRegionNum, (DISPLAYRECT*)pSrcRect, hDestWnd, bEnable);
}

BOOL CPlayer::GetNextKeyFramePos(DWORD nValue, int nType, PFRAME_POS pFramePos) 
{
	return m_lpPlay->GetNextKeyFramePos(&m_playParam, nValue, nType, pFramePos);
}

BOOL CPlayer::GetKeyFramePos(DWORD nValue, int nType, PFRAME_POS pFramePos) 
{
	return m_lpPlay->GetKeyFramePos(&m_playParam, nValue, nType, pFramePos);
}

BOOL CPlayer::SetVerticalSync(bool bVerticalSync)
{
	return m_lpPlay->SetVerticalSync(&m_playParam, bVerticalSync);
}

BOOL CPlayer::SetAntiAliasing(bool bAntiAliasing)
{
	return m_lpPlay->SetAntiAliasing(&m_playParam, bAntiAliasing);
}

BOOL CPlayer::GetQueryInfo(int cmdType, char* buf, int buflen, int* returnlen)
{
	return m_lpPlay->GetQueryInfo(&m_playParam, cmdType, buf, buflen, returnlen);
}

BOOL CPlayer::IsIndexCreated()
{
	return m_playParam.bIndexCreated;
}

int CPlayer::CreateFile()
{
	m_lpState[Play] = new FileState::CPlay;
	m_lpState[Pause] = new FileState::CPause;
	m_lpState[Stop] = new FileState::CStop;
	m_lpState[ToBegin] = new FileState::CToBegin;
	m_lpState[ToEnd] = new FileState::CToEnd;
	m_lpState[BackOne] = new FileState::CBackOne;
	m_lpState[OneByOne] = new FileState::COneByeOne;
	m_lpState[Slow] = new FileState::CSlow;
	m_lpState[Fast] = new FileState::CFast;

	memcpy(stateTable, FileState::stateTable, sizeof(stateTable));
	memcpy(openTable, FileState::openTable, sizeof(openTable));
	memcpy(closeTable, FileState::closeTable, sizeof(closeTable));

	m_lpPlay = new CFilePlayer;

	return 1;
}

int CPlayer::CreateFileStream()
{
	m_lpState[Play] = new FileStreamState::CPlay;
	m_lpState[Pause] = new FileStreamState::CPause;
	m_lpState[Stop] = new FileStreamState::CStop;
	m_lpState[ToBegin] = new FileStreamState::CToBegin;
	m_lpState[ToEnd] = new FileStreamState::CToEnd;
	m_lpState[BackOne] = new FileStreamState::CBackOne;
	m_lpState[OneByOne] = new FileStreamState::COneByeOne;
	m_lpState[Slow] = new FileStreamState::CSlow;
	m_lpState[Fast] = new FileStreamState::CFast;

	memcpy(stateTable, FileStreamState::stateTable, sizeof(stateTable));
	memcpy(openTable, FileStreamState::openTable, sizeof(openTable));
	memcpy(closeTable, FileStreamState::closeTable, sizeof(closeTable));

	m_lpPlay = new CFileStreamPlayer;
	
	return 1;
}

int CPlayer::Destroy()
{
	for(int i = 0 ; i < STATE_SIZE; i++)
	{
		if(m_lpState[i]) 
			delete m_lpState[i];
	}

	if(m_lpPlay)
		delete m_lpPlay;

	memset(m_lpState, 0, sizeof(m_lpState));
	m_lpPlay = NULL;

	return 1;
}

int CPlayer::ChangeState(PLAY_STATE nState)
{
	if(!m_lpPlay)
		return -1;

	if (Play == nState)
	{
		m_lpPlay->SetFishEyeInfoCallBack(&m_playParam, OnFisheyeInfo, this);
	}
	else if(Stop == nState && m_bStartFishEye)
	{
		m_bStartFishEye = false;
		m_bNeedDrawFishEyeLines = false;
		RegistDrawCallback(0, 0);
		m_lpPlay->StopFisheye(&m_playParam);
	}

	return m_lpPlay->ChangeState(m_lpState[nState]);
}

int CPlayer::Do()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->Do(&m_playParam);
}

TCHAR* CPlayer::Description()
{
	if(!m_lpPlay)
		return NULL;

	return m_lpPlay->Description(&m_playParam);
}

int CPlayer::OpenSound(BOOL bOpen)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->OpenSound(&m_playParam, bOpen);
}

int CPlayer::SetAudioVolume(int nVolume)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->SetAudioVolume(&m_playParam, nVolume);
}

int CPlayer::GetAudioVolume()
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->GetAudioVolume(&m_playParam);
}

int CPlayer::SetAudioWave(int nWave)
{
	if(!m_lpPlay)
		return -1;

	return m_lpPlay->SetAudioWave(&m_playParam, nWave);
}

BOOL CPlayer::RegistDrawCallback(fDrawCBFun cbDrawCBFun,void* nUser)
{
	if (!m_lpPlay)
		return -1;

	return m_lpPlay->RegistDrawCallback(&m_playParam, cbDrawCBFun, nUser);
}

BOOL CPlayer::SetPlayDirection(DWORD emDirection)
{
	if (!m_lpPlay)
		return -1;

	return m_lpPlay->SetPlayDirection(&m_playParam, emDirection);
}

BOOL CPlayer::RenderPrivateData(BOOL bTrue)
{
	if (!m_lpPlay)
		return -1;
	
	return m_lpPlay->RenderPrivateData(&m_playParam, bTrue);
}

void CALLBACK CPlayer::OnFisheyeInfo(LONG nPort,
									 BYTE byCorrectMode,
									 WORD wRadius,
									 WORD wCircleX,
									 WORD wCircleY,
									 UINT widthRatio,
									 UINT heightRatio,
									 BYTE gain,
									 BYTE denoiseLevel,
									 BYTE installStyle,
									 void* pUserData)
{
	CPlayer* pPlayer = (CPlayer*)pUserData;
	if (pPlayer)
	{
		pPlayer->SetFisheyeInfo(wCircleX, wCircleY, wRadius, widthRatio, heightRatio, installStyle);
	}
}

void CPlayer::SetFisheyeInfo(int x, int y, int radius, DWORD widthRatio, DWORD heightRatio, BYTE installStyle)
{
	if (x && y && radius)
	{
		m_feOptParams.originX = x;
		m_feOptParams.originY = y;
		m_feOptParams.radius = radius;

		if (m_feOptParams.mainStreamSize.w != widthRatio || m_feOptParams.mainStreamSize.h != heightRatio)
		{
			m_feOptParams.mainStreamSize.w = widthRatio;
			m_feOptParams.mainStreamSize.h = heightRatio;
		}

		PostMessage(m_playParam.pMainWnd, WM_USER_MSD_FISHEYEDEVICE_DETECT, true, NULL);
	}
	else
	{
		PostMessage(m_playParam.pMainWnd, WM_USER_MSD_FISHEYEDEVICE_DETECT, false, NULL);
	}
}

void CALLBACK fDrawCBFunFishEye(LONG nPort,HDC hDc, void* pUserData)
{
	CPlayer::Instance()->DrawFishEyeLines(hDc);
}

BOOL CPlayer::ControlFishEye()
{
	if (!m_lpPlay)
	{
		return -1;
	}

	if (m_bStartFishEye)
	{
		m_bStartFishEye = false;
		m_bNeedDrawFishEyeLines = false;
		RegistDrawCallback(0, 0);
		return m_lpPlay->StopFisheye(&m_playParam);
	}

	m_bStartFishEye = true;
	RegistDrawCallback(fDrawCBFunFishEye, 0);
	return m_lpPlay->StartFisheye(&m_playParam);
}

BOOL CPlayer::OptFisheyeParams(FISHEYE_OPERATETYPE operatetype, FISHEYE_OPTPARAM* pOptParam)
{
	if (!m_lpPlay)
	{
		return -1;
	}

	return m_lpPlay->OptFisheyeParams(&m_playParam, operatetype, pOptParam);
}

BOOL CPlayer::FisheyeEptzUpdate(FISHEYE_EPTZPARAM* eptzParam, BOOL bSecondRegion)
{
	if (!m_lpPlay)
	{
		return -1;
	}

	return m_lpPlay->FisheyeEptzUpdate(&m_playParam, eptzParam, bSecondRegion);
}

void CPlayer::SetFishEyeMode(FISHEYE_MOUNTMODE nMountMode, FISHEYE_CALIBRATMODE nCalibrateMode)
{
	if (!m_bStartFishEye)
	{
		return;
	}

	m_feOptParams.mainMountMode = (FISHEYE_MOUNTMODE)nMountMode;
	m_feOptParams.mainCalibrateMode = (FISHEYE_CALIBRATMODE)nCalibrateMode;

	BOOL nRet = m_lpPlay->OptFisheyeParams(&m_playParam, FISHEYE_SETPARAM, &m_feOptParams);
	if (nRet <= 0)
	{
		OutputDebugString(_T("Set Fisheye Params failed.\n"));
	}
	if(nCalibrateMode == FISHEYECALIBRATE_MODE_ORIGINAL_PLUS_THREE_EPTZ_REGION)
	{
		m_bNeedDrawFishEyeLines = true;
	}
	else
	{
		m_bNeedDrawFishEyeLines = false;
	}
}

BOOL CPlayer::EnableLargePicAdjustment(int bEnable)
{
	if (!m_lpPlay)
	{
		return -1;
	}

	return m_lpPlay->EnableLargePicAdjustment(&m_playParam, bEnable);
}


void CPlayer::DrawFishEyeLines(HDC hdc)
{
	if(m_bNeedDrawFishEyeLines)
	m_feHandler.Draw(m_playParam.hWnd, hdc);
}

void CPlayer::StartDragFE(CPoint point)
{
	if(m_bNeedDrawFishEyeLines)
	m_feHandler.StartDrag(point);
}

void CPlayer::StopDragFE()
{
	if(m_bNeedDrawFishEyeLines)
	m_feHandler.StopDrag();
}

void CPlayer::UpdateDragFE(CPoint point)
{
	if(m_bNeedDrawFishEyeLines)
	m_feHandler.UpdateDrag(point);
}

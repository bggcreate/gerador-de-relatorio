// FileStreamPlayer.h: interface for the CFileStreamPlayer class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FILESTREAMPLAYER_H__7F04C817_EA84_45C6_8052_E69BBCF9A3B3__INCLUDED_)
#define AFX_FILESTREAMPLAYER_H__7F04C817_EA84_45C6_8052_E69BBCF9A3B3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "IPlay.h"
#include "FilePlayer.h" 
#include "FileManipulate.h"

namespace FileStreamState
{
	//State translate table 0 Close£¬1  Open£¬2 not Change
	static int stateTable[STATE_SIZE][STATE_SIZE] = {
		/*				Play	Pause	Stop	ToBegin	ToEnd	BackOne	One		Slow	Fast */
		/*Play*/		{0,		1,		1,		1,		1,		0,		1,		1,		1},
		/*Pause*/		{1,		0,		1,		1,		1,		0,		1,		0,		0},
		/*Stop*/		{1,		0,		0,		0,		0,		0,		0,		0,		0},
		/*ToBegin*/		{2,		2,		2,		2,		2,		0,		2,		2,		2},
		/*ToEnd*/		{2,		2,		2,		2,		2,		0,		2,		2,		2},
		/*BackOne*/		{2,		2,		2,		2,		2,		0,		2,		2,		2},
		/*OneByOne*/	{1,		0,		1,		1,		1,		0,		1,		0,		0},
		/*Slow*/		{1,		0,		1,		1,		1,		0,		0,		1,		1},
		/*Fast*/		{1,		0,		1,		1,		1,		0,		0,		1,		1}
	};
	
	static int openTable[STATE_SIZE] = {1, 0, 0, 0, 0, 0, 0, 0, 0};
	static int closeTable[STATE_SIZE] = {0, 0, 0, 0, 0, 0, 0, 0, 0};

	/*Use the same method with FilePlayer*/
	typedef FileState::CPlay CPlay;
	typedef FileState::CPause CPause;
	typedef FileState::CBackOne CBackOne;
	typedef FileState::COneByeOne COneByeOne;
	typedef FileState::CSlow CSlow;
	typedef FileState::CFast CFast;

	class CStop : public IPlayState
	{
		virtual int Do(PlayParam* lpPlayParam);
		virtual TCHAR* Description(PlayParam* lpPlayParam);
	};

	class CToBegin : public IPlayState
	{
		virtual int Do(PlayParam* lpPlayParam);
		virtual TCHAR* Description(PlayParam* lpPlayParam);
	};

	class CToEnd : public IPlayState
	{
		virtual int Do(PlayParam* lpPlayParam);
		virtual TCHAR* Description(PlayParam* lpPlayParam);
	};
}

class CFileStreamPlayer  : public IPlay
{
public:
	CFileStreamPlayer();
	virtual ~CFileStreamPlayer();

	virtual int Open(TCHAR* lpszFile, PlayParam* lpPlayParam);
	virtual int Close(PlayParam* lpPlayParam);
	virtual int Do(PlayParam* lpPlayParam);
	virtual int OpenSound(PlayParam* lpPlayParam, BOOL bOpen);
	virtual int SetAudioVolume(PlayParam* lpPlayParam, int nVolume);
	virtual int GetAudioVolume(PlayParam* lpPlayParam);
	virtual int SetAudioWave(PlayParam* lpPlayParam, int nWave);
	virtual TCHAR* Description(PlayParam* lpPlayParam);
	virtual int ChangeState(IPlayState* lpState);
	virtual int GetProcess(PlayParam* lpPlayParam);
	virtual int Seek(PlayParam* lpPlayParam, int nPos);
	virtual int SetColor(PlayParam* lpPlayParam, int nSaturation, int nBrightness, int nContrast, int nHue);
	virtual int GetColor(PlayParam* lpPlayParam, int* nSaturation, int* nBrightness, int* nContrast, int* nHue);
	virtual int GetPlayedTime(PlayParam* lpPlayParam);
	virtual int GetTotalTime(PlayParam* lpPlayParam);
	virtual int GetPlayedFrame(PlayParam* lpPlayParam);
	virtual int GetTotalFrame(PlayParam* lpPlayParam);
	virtual int GetRate(PlayParam* lpPlayParam, int* nRate);
	virtual int GetBitrate(PlayParam* lpPlayParam, double* dbBitrate);
	virtual BOOL SnapPicture(PlayParam* lpPlayParam, char* szFileName, int nType);/*nType,0 bmp 1 jpg*/
	virtual int SetDisplayRegion(PlayParam* lpPlayParam, DWORD nRegionNum, DISPLAYRECT *pSrcRect, HWND hDestWnd, BOOL bEnable);
	virtual int GetPicSize(PlayParam* lpPlayParam, long *width, long *height);
	virtual BOOL SetCurrentFrameNum(PlayParam *lpPlayParam, int frameNum){return true;};
	virtual BOOL SetPlayedTimeEx(PlayParam *lpPlayParam, int nTime){return true;};
	virtual BOOL GetNextKeyFramePos(PlayParam *lpPlayParam, DWORD nValue, int nType, PFRAME_POS pFramePos){return true;};
	virtual BOOL GetKeyFramePos(PlayParam *lpPlayParam, DWORD nValue, int nType, PFRAME_POS pFramePos){return true;}
	virtual BOOL SetVerticalSync(PlayParam *lpPlayParam, BOOL bVerticalSync);
	virtual BOOL GetQueryInfo(PlayParam *lpPlayParam, int cmdType, char* buf, int buflen, int* returnlen);
	virtual BOOL RegistDrawCallback(PlayParam *lpPlayParam, fDrawCBFun DrawCBFun, void* nUser);
	virtual BOOL SetPlayDirection(PlayParam* lpPlayParam, DWORD emDirection);
	virtual BOOL RenderPrivateData(PlayParam* lpPlayParam, BOOL bTrue);
	virtual BOOL StartFisheye(PlayParam* lpPlayParam);
	virtual BOOL OptFisheyeParams(PlayParam* lpPlayParam, FISHEYE_OPERATETYPE operatetype, FISHEYE_OPTPARAM* pOptParam);
	virtual BOOL FisheyeEptzUpdate(PlayParam* lpPlayParam, FISHEYE_EPTZPARAM* pEptzParam, BOOL bSecondRegion);
	virtual BOOL StopFisheye(PlayParam* lpPlayParam);
	virtual BOOL SetFishEyeInfoCallBack(PlayParam* lpPlayParam, fFishEyeInfoFun pFishEyeInfoFun, void* pUserData);
	virtual BOOL EnableLargePicAdjustment(PlayParam* lpPlayParam, BOOL bEnable);
	virtual	BOOL SetAntiAliasing(PlayParam *lpPlayParam, BOOL bAntiAliasing);
	int OnProc();
private:
	IPlayState* m_lpState;

	/*Thread parameter*/
	PlayParam* m_lpPlayParam; 
	
	HANDLE m_hExit;
	HANDLE m_hThread;
	bool m_bBegin;
};

#endif // !defined(AFX_FILESTREAMPLAYER_H__7F04C817_EA84_45C6_8052_E69BBCF9A3B3__INCLUDED_)

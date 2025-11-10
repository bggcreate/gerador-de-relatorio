// Player.h: interface for the CPlayer class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_PLAYER_H__9DC81D4F_E754_4C5F_8C69_B621C8D09651__INCLUDED_)
#define AFX_PLAYER_H__9DC81D4F_E754_4C5F_8C69_B621C8D09651__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "IPlay.h"
#include "FishEyePTZHandler.h"

class CPlayer  
{
public:
	//state translate table
	int stateTable[STATE_SIZE][STATE_SIZE];
	int openTable[STATE_SIZE];
	int closeTable[STATE_SIZE];
public:
	/*
	* singleton pattern ,used for different UI
	*/
	static CPlayer* Instance();

	enum TYPE {FILE, FILESTREAM};
	int Open(TYPE eType, TCHAR* lpszFile, HWND hWnd, HWND wnd);
	int Close();
	int ChangeState(PLAY_STATE nState);
	int Do();
	int OpenSound(BOOL bOpen);
	int SetAudioVolume(int nVolume);
	int GetAudioVolume();
	int SetAudioWave(int nWave);
	TCHAR* Description(); 
	int GetProcess();
	int Seek(int nPos);
	int SetColor(int nSaturation, int nBrightness, int nContrast, int nHue);
	int GetColor(int* nSaturation, int* nBrightness, int* nContrast, int* nHue);
	int GetPlayedTime();
	int GetTotalTime();
	int GetPlayedFrame();
	int GetTotalFrame();
	int GetPicture(int* nWidth, int* nHeight);
	int GetRate(int* nRate);
	int GetBitrate(double* dbBitrate);
	int SnapPicture(char* szFileName, int nType);
	int SetDisplayRegion(DWORD nRegionNum, RECT *pSrcRect, HWND hDestWnd, BOOL bEnable);
	BOOL GetPicSize(long *width, long *height);
	BOOL SetCurrentFrameNum(int frameNum);
	BOOL SetPlayedTiemEx(int time);
	BOOL GetNextKeyFramePos(DWORD nValue, int nType, PFRAME_POS pFramePos);
	BOOL GetKeyFramePos(DWORD nValue, int nType, PFRAME_POS pFramePos);
	BOOL SetVerticalSync(bool bVerticalSync);
	BOOL SetAntiAliasing(bool bAntiAliasing);
	BOOL GetQueryInfo(int cmdType, char* buf, int buflen, int* returnlen);
	BOOL IsIndexCreated();
	BOOL RegistDrawCallback(fDrawCBFun cbDrawCBFun,void* nUser);
	BOOL SetPlayDirection(DWORD emDirection);
	BOOL RenderPrivateData(BOOL bTrue);
	BOOL ControlFishEye();
	BOOL OptFisheyeParams(FISHEYE_OPERATETYPE operatetype, FISHEYE_OPTPARAM* pOptParam);
	BOOL FisheyeEptzUpdate(FISHEYE_EPTZPARAM* , BOOL bSecondRegion);	
	void SetFishEyeMode(FISHEYE_MOUNTMODE nMountMode, FISHEYE_CALIBRATMODE nCalibrateMode);
	static void CALLBACK OnFisheyeInfo(LONG nPort, 
										BYTE byCorrectMode,
										WORD wRadius,
										WORD wCircleX,
										WORD wCircleY,
										UINT widthRatio,
										UINT heightRatio,
										BYTE gain,
										BYTE denoiseLevel,
										BYTE installStyle,
										void* pUserData);
	void SetFisheyeInfo(int x, int y, int radius, DWORD widthRatio, DWORD heightRatio, BYTE installStyle);
	BOOL EnableLargePicAdjustment(int bEnable);

	void DrawFishEyeLines(HDC hdc);
	void StartDragFE(CPoint point);
	void StopDragFE();
	void UpdateDragFE(CPoint point);

protected:
	int CreateFile();
	int CreateFileStream();
	int Destroy();
	//void DrawFishEyeOneWin(HWND hwin, HDC hdc, int winid, int red, int green, int blue);

private:
	/*Created here*/
	CPlayer();
public:
	virtual ~CPlayer();
private:
	IPlayState* m_lpState[STATE_SIZE];
	PlayParam m_playParam;
	IPlay* m_lpPlay;
	bool m_bStartFishEye;			
	FISHEYE_OPTPARAM m_feOptParams; //fisheye params
	volatile bool m_bNeedDrawFishEyeLines;
	
	CFishEyePTZHandler m_feHandler;
};

#endif // !defined(AFX_PLAYER_H__9DC81D4F_E754_4C5F_8C69_B621C8D09651__INCLUDED_)

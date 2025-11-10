// PlayStateText.h: interface for the CPlayStateText class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_PLAYSTATETEXT_H__6868F88B_5D4B_4581_8F02_77FA1D466F1A__INCLUDED_)
#define AFX_PLAYSTATETEXT_H__6868F88B_5D4B_4581_8F02_77FA1D466F1A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


class CPlayStateText  
{
public:
	CPlayStateText();
	virtual ~CPlayStateText();

public:
	void SetState(CString szState);
	void SetPlayedTime(int nPlayedTime , int nTotalTime /*0  not change*/);
	void SetPlayedFrame(int nPlayedFrame, int nTotalFrame /*0 not change*/);
	void SetBitrate(double dbBitrate);
	void SetRate(int nRate);
	void SetPictureSize(int nWidth, int nHeight);
	/*reset*/
	void Init(CWnd* lpWnd);
	void Show();
	void Clear();
private:
	CWnd*	m_lpWnd;
	
	CString m_szState;
	int m_nPlayedTime;
	int m_nTotalTime;
	int m_nPlayedFrame;
	int m_nTotalFrame;
	double m_dbBitrate;
	int m_nRate;
	int m_nWidth;
	int m_nHeight;
};

#endif // !defined(AFX_PLAYSTATETEXT_H__6868F88B_5D4B_4581_8F02_77FA1D466F1A__INCLUDED_)
